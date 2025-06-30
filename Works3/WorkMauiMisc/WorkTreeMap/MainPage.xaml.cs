using System.Diagnostics;

namespace WorkTreeMap;

using Microsoft.Maui.Graphics;

using Smart.Linq;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // サンプルデータ（ラベルと重み）
        var data = new List<ColorCount>
        {
            new(Color.FromRgb(55, 54, 57), 147108),
            new(Color.FromRgb(67, 68, 78), 89474),
            new(Color.FromRgb(42, 39, 37), 83509),
            new(Color.FromRgb(82, 88, 104), 44901),
            new(Color.FromRgb(179, 149, 118), 30217),
            new(Color.FromRgb(18, 16, 13), 29373),
            new(Color.FromRgb(104, 118, 139), 28429),
            new(Color.FromRgb(132, 147, 179), 28332),
            new(Color.FromRgb(83, 54, 30), 25446),
            new(Color.FromRgb(116, 88, 58), 22547),
            new(Color.FromRgb(162, 180, 208), 21125),
            new(Color.FromRgb(145, 103, 19), 20988),
            new(Color.FromRgb(202, 218, 232), 19795),
            new(Color.FromRgb(68, 185, 242), 19785),
            new(Color.FromRgb(216, 51, 40), 19641),
            new(Color.FromRgb(236, 190, 111), 19253),
            new(Color.FromRgb(148, 123, 94), 17557),
            new(Color.FromRgb(208, 183, 172), 17426),
            new(Color.FromRgb(172, 126, 60), 14489),
            new(Color.FromRgb(3, 86, 33), 14355),
            new(Color.FromRgb(35, 129, 162), 13003),
            new(Color.FromRgb(216, 172, 5), 11042),
            new(Color.FromRgb(17, 183, 93), 10379),
            new(Color.FromRgb(171, 220, 10), 7724),
            new(Color.FromRgb(144, 22, 17), 6150)
        };

        // 重みを元に二分木を構築
        var root = TreeMapNode<ColorCount>.Build(data, static x => x.Count);

        // Drawableに渡す
        TreeMapView.Drawable = new TreeMapDrawable(root);
    }
}

public class TreeMapDrawable : IDrawable
{
    private readonly TreeMapNode<ColorCount> root;

    public TreeMapDrawable(TreeMapNode<ColorCount> root)
    {
        this.root = root;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        DrawNode(canvas, root, dirtyRect);

        canvas.RestoreState();
    }

    private void DrawNode(ICanvas canvas, TreeMapNode<ColorCount> node, RectF rect)
    {
        if (node.IsLeaf)
        {
            // 葉ノード：矩形とラベルを描画
            //var color = _palette[depth % _palette.Length];
            //canvas.FillColor = color;
            canvas.FillColor = node.Value.Color;
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
            var splitVertically = rect.Width >= rect.Height;
            var ratio = node.Left.Area / node.Area;

            if (splitVertically)
            {
                var splitX = rect.X + (float)(rect.Width * ratio);
                var leftRect = new RectF(rect.X, rect.Y, splitX - rect.X, rect.Height);
                var rightRect = new RectF(splitX, rect.Y, rect.Right - splitX, rect.Height);
                DrawNode(canvas, node.Left, leftRect);
                DrawNode(canvas, node.Right, rightRect);
            }
            else
            {
                var splitY = rect.Y + (float)(rect.Height * ratio);
                var topRect = new RectF(rect.X, rect.Y, rect.Width, splitY - rect.Y);
                var bottomRect = new RectF(rect.X, splitY, rect.Width, rect.Bottom - splitY);
                DrawNode(canvas, node.Left, topRect);
                DrawNode(canvas, node.Right, bottomRect);
            }
        }
    }
}

public record ColorCount(Color Color, int Count);

public sealed class TreeMapNode<T>
{
    public T Value { get; } = default!;

    public double Area { get; }

    public TreeMapNode<T> Left { get; } = default!;

    public TreeMapNode<T> Right { get; } = default!;

    public bool IsLeaf { get; }

    private TreeMapNode(T value, double area)
    {
        Value = value;
        Area = area;
        IsLeaf = true;
    }

    private TreeMapNode(TreeMapNode<T> left, TreeMapNode<T> right)
    {
        Left = left;
        Right = right;
        Area = left.Area + right.Area;
        IsLeaf = false;
    }

#pragma warning disable CA1000
    public static TreeMapNode<T> Build(IEnumerable<T> values, Func<T, double> areaSelector)
    {
        return BuildRecursive(values
            .Select(value => new TreeMapNode<T>(value, areaSelector(value)))
            .OrderByDescending(node => node.Area)
            .ToArray());
    }
#pragma warning restore CA1000

    private static TreeMapNode<T> BuildRecursive(ReadOnlySpan<TreeMapNode<T>> nodes)
    {
        if (nodes.Length == 1)
        {
            return nodes[0];
        }

        var total = 0d;
        foreach (var node in nodes)
        {
            total += node.Area;
        }

        var split = 0;
        var sum = 0d;
        for (var i = 0; i < nodes.Length; i++)
        {
            sum += nodes[i].Area;
            if (sum >= total / 2)
            {
                split = i + 1;
                break;
            }
        }

        split = Math.Clamp(split, 1, nodes.Length - 1);

        var left = BuildRecursive(nodes[..split]);
        var right = BuildRecursive(nodes[split..]);

        return new TreeMapNode<T>(left, right);
    }
}
