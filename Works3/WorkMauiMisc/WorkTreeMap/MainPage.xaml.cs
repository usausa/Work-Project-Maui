namespace WorkTreeMap;

using Microsoft.Maui.Graphics;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // サンプルデータ（ラベルと重み）
        var data = new Dictionary<string, double>
        {
            { "Category A", 6 },
            { "Category B", 4 },
            { "Category C", 3 },
            { "Category D", 2 },
            { "Category E", 1 },
        };

        // 重みを元に二分木を構築
        var root = TreeMapBuilder.Build(data);

        // Drawableに渡す
        TreeMapView.Drawable = new TreeMapDrawable(root);
    }
}

public class TreeMapDrawable : IDrawable
{
    private readonly TreeMapNode _root;
    private readonly Color[] _palette = new[]
    {
        Colors.SkyBlue, Colors.MediumSeaGreen, Colors.Orange,
        Colors.Tomato, Colors.Goldenrod, Colors.Plum
    };

    public TreeMapDrawable(TreeMapNode root)
    {
        _root = root;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        DrawNode(canvas, _root, dirtyRect, 0);

        canvas.RestoreState();
    }

    private void DrawNode(ICanvas canvas, TreeMapNode node, RectF rect, int depth)
    {
        if (node.IsLeaf)
        {
            // 葉ノード：矩形とラベルを描画
            var color = _palette[depth % _palette.Length];
            canvas.FillColor = color;
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 1;
            canvas.FillRectangle(rect);
            canvas.DrawRectangle(rect);

            canvas.FontColor = Colors.Black;
            canvas.FontSize = 14;
            canvas.DrawString(node.Label, rect.X + 4, rect.Y + 4, HorizontalAlignment.Left);
        }
        else
        {
            // 内部ノード：向きを決め、比率で分割
            bool splitVertically = rect.Width >= rect.Height;
            double ratio = node.Left.Value / node.Value;

            if (splitVertically)
            {
                float splitX = rect.X + (float)(rect.Width * ratio);
                var leftRect = new RectF(rect.X, rect.Y, splitX - rect.X, rect.Height);
                var rightRect = new RectF(splitX, rect.Y, rect.Right - splitX, rect.Height);
                DrawNode(canvas, node.Left, leftRect, depth + 1);
                DrawNode(canvas, node.Right, rightRect, depth + 1);
            }
            else
            {
                float splitY = rect.Y + (float)(rect.Height * ratio);
                var topRect = new RectF(rect.X, rect.Y, rect.Width, splitY - rect.Y);
                var bottomRect = new RectF(rect.X, splitY, rect.Width, rect.Bottom - splitY);
                DrawNode(canvas, node.Left, topRect, depth + 1);
                DrawNode(canvas, node.Right, bottomRect, depth + 1);
            }
        }
    }
}

public class TreeMapNode
{
    public double Value { get; }
    public string Label { get; }
    public TreeMapNode Left { get; }
    public TreeMapNode Right { get; }

    // 葉ノード用コンストラクタ
    public TreeMapNode(string label, double value)
    {
        Label = label;
        Value = value;
    }

    // 内部ノード用コンストラクタ
    public TreeMapNode(TreeMapNode left, TreeMapNode right)
    {
        Left = left;
        Right = right;
        Value = left.Value + right.Value;
    }

    public bool IsLeaf => Left == null && Right == null;
}

public static class TreeMapBuilder
{
    public static TreeMapNode Build(IDictionary<string, double> data)
    {
        // 葉ノードリストを作成
        var leaves = data.Select(kv => new TreeMapNode(kv.Key, kv.Value))
            .OrderByDescending(n => n.Value)
            .ToList();

        return BuildRecursive(leaves);
    }

    private static TreeMapNode BuildRecursive(List<TreeMapNode> nodes)
    {
        if (nodes.Count == 1)
            return nodes[0];

        // 合計の半分を目指して分割
        double total = nodes.Sum(n => n.Value);
        double acc = 0;
        int split = 0;
        for (int i = 0; i < nodes.Count; i++)
        {
            acc += nodes[i].Value;
            if (acc >= total / 2)
            {
                split = i + 1;
                break;
            }
        }
        // 片方が0要素にならないように保護
        split = Math.Clamp(split, 1, nodes.Count - 1);

        var leftList = nodes.Take(split).ToList();
        var rightList = nodes.Skip(split).ToList();

        var leftNode = BuildRecursive(leftList);
        var rightNode = BuildRecursive(rightList);

        return new TreeMapNode(leftNode, rightNode);
    }
}