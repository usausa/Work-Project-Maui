namespace Template.MobileApp.Messaging;

using CommunityToolkit.Maui.Core;

using Template.MobileApp.Helpers;

public sealed class GetImageStreamEventArgs : ValueTaskEventArgs<Stream?>
{
    public CancellationToken Token { get; set; } = CancellationToken.None;

    public Stream? Stream { get; set; }
}

public interface IDrawingController
{
    event EventHandler<GetImageStreamEventArgs>? GetImageStreamRequest;

    Color LineColor { get; set; }

    float LineWidth { get; set; }

    ObservableCollection<IDrawingLine> Lines { get; }
}

public sealed partial class DrawingController : ObservableObject, IDrawingController
{
    private EventHandler<GetImageStreamEventArgs>? getImageStreamRequest;

    event EventHandler<GetImageStreamEventArgs>? IDrawingController.GetImageStreamRequest
    {
        add => getImageStreamRequest += value;
        remove => getImageStreamRequest -= value;
    }

    // Property

    [ObservableProperty]
    public partial Color LineColor { get; set; } = Colors.Black;

    [ObservableProperty]
    public partial float LineWidth { get; set; } = 5;

    public ObservableCollection<IDrawingLine> Lines { get; set; } = new();

    // Message

    public ValueTask<Stream?> GetImageStream(CancellationToken token = default)
    {
        var args = new GetImageStreamEventArgs
        {
            Token = token
        };
        getImageStreamRequest?.Invoke(this, args);
        return args.Task;
    }
}
