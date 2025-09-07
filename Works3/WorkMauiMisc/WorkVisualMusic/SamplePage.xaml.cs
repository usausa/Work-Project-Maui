using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkVisualMusic;

public partial class SamplePage : ContentPage
{
	public SamplePage()
	{
		InitializeComponent();
	}
}

public partial class SamplePageViewModel : ExtendViewModelBase
{
    private Random random = new();

    private int[] currentValues;
    private int[] previousValues;

	[ObservableProperty]
	public partial double KnobValue1 { get; set; } = 50;

    [ObservableProperty]
    public partial double KnobValue3 { get; set; } = 50;

    [ObservableProperty]
    public partial double KnobValue2 { get; set; } = 50;

    [ObservableProperty]
    public partial double SliderValue1 { get; set; } = 50;

    [ObservableProperty]
    public partial double SliderValue3 { get; set; } = 50;

    [ObservableProperty]
    public partial double SliderValue2 { get; set; } = 50;

    [ObservableProperty]
    public partial int[] Values { get; set; }

    public SamplePageViewModel()
	{
        currentValues = new int[16];
        previousValues = new int[16];
        Values = currentValues;

        _ = StartAnimation();
    }

    private async Task StartAnimation()
    {
        while (true)
        {
            (currentValues, previousValues) = (previousValues, currentValues);

            for (var i = 0; i < previousValues.Length; i++)
            {
                currentValues[i] = Math.Clamp((int)((previousValues[i] * 0.3) + (random.Next(0, 16 + 2) * 0.7)), 0, 16);
            }

            Values = currentValues;

            await Task.Delay(50);
        }
    }
}