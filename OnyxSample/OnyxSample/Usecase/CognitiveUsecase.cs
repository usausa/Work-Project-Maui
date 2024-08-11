namespace OnyxSample.Usecase;

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

using SkiaSharp;

using Smart.IO;

public class DetectResult
{
    public float Left { get; }

    public float Top { get; }

    public float Right { get; }

    public float Bottom { get; }

    public float Score { get; }

    public string Label { get; }

    public DetectResult(float left, float top, float right, float bottom, float score, string label)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
        Score = score;
        Label = label;
    }
}

public sealed class CognitiveUsecase : IDisposable
{
    private readonly IFileSystem fileSystem;

    private bool initialized;

    private InferenceSession session = default!;

    private string[] labels = default!;

    public CognitiveUsecase(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public void Dispose()
    {
        if (initialized)
        {
            session.Dispose();
        }
    }

    public Task<DetectResult[]> DetectAsync(SKBitmap bitmap) => Task.Run(async () =>
    {
        await PrepareSessionAsync();

        var metadata = session.InputMetadata.First();
        var dimensions = metadata.Value.Dimensions;
        var height = dimensions[2];
        var width = dimensions[3];

        var size = 3 * width * height;
        var buffer = ArrayPool<float>.Shared.Rent(size);
        var inputTensor = new DenseTensor<float>(buffer.AsMemory(0, size), [1, 3, height, width]);
        PrepareTensor(bitmap, inputTensor, width, height);

        var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(metadata.Key, inputTensor) };
        using var values = session.Run(inputs);

        ArrayPool<float>.Shared.Return(buffer);

        var boxes = values.First(x => x.Name == "detected_boxes").AsTensor<float>();
        var classes = values.First(x => x.Name == "detected_classes").AsTensor<long>();
        var scores = values.First(x => x.Name == "detected_scores").AsTensor<float>();

        var results = new List<DetectResult>();
        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < scores.Length; i++)
        {
            results.Add(new DetectResult(boxes[0, i, 0], boxes[0, i, 1], boxes[0, i, 2], boxes[0, i, 3], scores[0, i], labels[classes[0, i]]));
        }
        return results.ToArray();
    });

    private async ValueTask PrepareSessionAsync()
    {
        if (initialized)
        {
            return;
        }

        await using var modelStream = await fileSystem.OpenAppPackageFileAsync("model.onnx");
        session = new InferenceSession(await modelStream.ReadAllBytesAsync());

        await using var labelStream = await fileSystem.OpenAppPackageFileAsync("labels.txt");
        using var reader = new StreamReader(labelStream);
        labels = await reader.ReadLinesAsync().ToArrayAsync();

        initialized = true;
    }

    private static void PrepareTensor(SKBitmap bitmap, DenseTensor<float> tensor, int width, int height)
    {
        var resizedBitmap = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
        for (var y = 0; y < resizedBitmap.Height; y++)
        {
            for (var x = 0; x < resizedBitmap.Width; x++)
            {
                var color = resizedBitmap.GetPixel(x, y);
                tensor[0, 0, y, x] = color.Red - 255.0f;
                tensor[0, 1, y, x] = color.Green - 255.0f;
                tensor[0, 2, y, x] = color.Blue - 255.0f;
            }
        }
    }
}
