using Smart.Maui;
using Smart.Maui.Animations;
using Smart.Maui.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm.Messaging;

namespace WorkDesign;

public partial class AnimationEasingPage : ContentPage
{
	public AnimationEasingPage()
	{
		InitializeComponent();
	}
}

public sealed class AnimationEasingPageViewModel : ExtendViewModelBase
{
	public EventRequest AnimationRequest { get; } = new();

	public IObserveCommand AnimationCommand { get; }

    public AnimationEasingPageViewModel()
    {
        AnimationCommand = MakeDelegateCommand(() => AnimationRequest.Request());
    }
}

public sealed class EasingDemoAnimation : AnimationBase
{
    protected override async Task BeginAnimation(VisualElement target)
    {
        target.TranslationX = 0;
        target.TranslationY = 0;

        var parent = target.Parent.FindParent<VisualElement>();
		if (parent is null)
		{
			return;
        }

        var x = parent.Width - target.Width;
		var y = parent.Height - target.Height;
        await target.TranslateTo(x, y, Duration, Easing);
    }
}