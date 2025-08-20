using Smart.Maui.Animations;
using Smart.Maui.Interactivity;
using Smart.Maui.ViewModels;

using System.Globalization;

namespace WorkDesign;

public partial class AnimeSamplePage : ContentPage
{
	public AnimeSamplePage()
	{
		InitializeComponent();
	}
}

public sealed class AnimeSamplePageViewModel : ExtendViewModelBase
{
}

public sealed class FadeToAnimation2 : AnimationBase
{
    public static readonly BindableProperty OpacityProperty = BindableProperty.Create(
        nameof(Opacity),
        typeof(double),
        typeof(FadeToAnimation),
        0.0d,
        BindingMode.TwoWay);

    public double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    protected override async Task BeginAnimation(VisualElement target)
    {
        await target.FadeTo(Opacity, Convert.ToUInt32(Duration, CultureInfo.InvariantCulture), Easing);
    }
}
[ContentProperty("Animations")]
public sealed class SequentialAnimation2 : AnimationBase
{
#pragma warning disable CA2227
    public List<AnimationBase> Animations { get; }
#pragma warning restore CA2227

    public SequentialAnimation2()
    {
        Animations = [];
    }

#pragma warning disable CA1002
    public SequentialAnimation2(List<AnimationBase> animations)
    {
        Animations = animations;
    }
#pragma warning restore CA1002

    protected override async Task BeginAnimation(VisualElement target)
    {
        foreach (var animation in Animations)
        {
            await animation.Begin();
        }
    }
}

[ContentProperty("Animation")]
public sealed class BeginAnimationAction2 : BindableObject, IAction
{
    public static readonly BindableProperty AnimationProperty = BindableProperty.Create(
        nameof(Animation),
        typeof(AnimationBase),
        typeof(BeginAnimationAction2));

    public AnimationBase? Animation
    {
        get => (AnimationBase)GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    // ReSharper disable once AsyncVoidMethod
    public async void Execute(BindableObject associatedObject, object? parameter)
    {
        if ((associatedObject is VisualElement visual) && (Animation is not null))
        {
            Animation.Target ??= visual;

            await Animation.Begin();
        }
    }
}
