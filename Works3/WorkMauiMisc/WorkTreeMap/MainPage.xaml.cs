namespace WorkTreeMap;

using Microsoft.Maui.Graphics;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // サンプルデータ（ラベルと重み）
        var data = new List<TreeMapNode>
        {
            new(Color.FromRgb(55, 54, 57), "1", 147108),
            new(Color.FromRgb(67, 68, 78), "2", 89474),
            new(Color.FromRgb(42, 39, 37), "3", 83509),
            new(Color.FromRgb(82, 88, 104), "4", 44901),
            new(Color.FromRgb(179, 149, 118), "5", 30217),
            new(Color.FromRgb(18, 16, 13), "6", 29373),
            new(Color.FromRgb(104, 118, 139), "7", 28429),
            new(Color.FromRgb(132, 147, 179), "8", 28332),
            new(Color.FromRgb(83, 54, 30), "9", 25446),
            new(Color.FromRgb(116, 88, 58), "10", 22547),
            new(Color.FromRgb(162, 180, 208), "11", 21125),
            new(Color.FromRgb(145, 103, 19), "12", 20988),
            new(Color.FromRgb(202, 218, 232), "13", 19795),
            new(Color.FromRgb(68, 185, 242), "14", 19785),
            new(Color.FromRgb(216, 51, 40), "15", 19641),
            new(Color.FromRgb(236, 190, 111), "16", 19253),
            new(Color.FromRgb(148, 123, 94), "17", 17557),
            new(Color.FromRgb(208, 183, 172), "18", 17426),
            new(Color.FromRgb(172, 126, 60), "19", 14489),
            new(Color.FromRgb(3, 86, 33), "20", 14355),
            new(Color.FromRgb(35, 129, 162), "21", 13003),
            new(Color.FromRgb(216, 172, 5), "22", 11042),
            new(Color.FromRgb(17, 183, 93), "23", 10379),
            new(Color.FromRgb(171, 220, 10), "24", 7724),
            new(Color.FromRgb(144, 22, 17), "25", 6150)
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
    //private readonly Color[] _palette = new[]
    //{
    //    Colors.SkyBlue, Colors.MediumSeaGreen, Colors.Orange,
    //    Colors.Tomato, Colors.Goldenrod, Colors.Plum
    //};

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
            //var color = _palette[depth % _palette.Length];
            //canvas.FillColor = color;
            canvas.FillColor = node.Color;
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 1;
            canvas.FillRectangle(rect);
            //canvas.DrawRectangle(rect);

            //canvas.FontColor = Colors.Black;
            //canvas.FontSize = 14;
            //canvas.DrawString(node.Label, rect.X + 4, rect.Y + 4, HorizontalAlignment.Left);
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
    public Color Color { get; }
    public TreeMapNode Left { get; }
    public TreeMapNode Right { get; }

    // 葉ノード用コンストラクタ
    public TreeMapNode(Color color, string label, double value)
    {
        Color = color;
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
    public static TreeMapNode Build(List<TreeMapNode> leaves)
    {
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