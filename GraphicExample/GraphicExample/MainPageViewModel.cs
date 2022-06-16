namespace GraphicExample;

using System.Windows.Input;

using Smart.Maui.ViewModels;

#pragma warning disable CA5394
public class MainPageViewModel : ViewModelBase
{
    private readonly Random random = new();

    public GraphicSource<TestData> Source { get; } = new(new TestData());

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand LeftCommand { get; }
    public ICommand RightCommand { get; }

    public MainPageViewModel()
    {
        AddCommand = MakeDelegateCommand(() =>
        {
            Source.Value.Points.Add(new Point(random.NextDouble(), random.NextDouble()));
            Source.Update();
        });
        DeleteCommand = MakeDelegateCommand(() =>
        {
            if (Source.Value.Points.Count > 0)
            {
                Source.Value.Points.RemoveAt(Source.Value.Points.Count - 1);
            }
            Source.Update();
        });
        LeftCommand = MakeDelegateCommand(() =>
        {
            for (var i = 0; i < Source.Value.Points.Count; i++)
            {
                var point = Source.Value.Points[i];
                Source.Value.Points[i] = new Point(point.X - 0.1, point.Y);
            }
            Source.Update();
        });
        RightCommand = MakeDelegateCommand(() =>
        {
            for (var i = 0; i < Source.Value.Points.Count; i++)
            {
                var point = Source.Value.Points[i];
                Source.Value.Points[i] = new Point(point.X + 0.1, point.Y);
            }
            Source.Update();
        });
    }
}
#pragma warning restore CA5394
