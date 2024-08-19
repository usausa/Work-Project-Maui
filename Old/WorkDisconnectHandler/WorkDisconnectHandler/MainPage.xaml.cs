namespace WorkDisconnectHandler;

using System.Diagnostics;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Layouts;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        var child = Container.Children.OfType<ChildView>().FirstOrDefault();
        if (child is not null)
        {
            Container.Children.Remove(child);
            Cleanup(child);
        }

        child = new ChildView();
        AbsoluteLayout.SetLayoutFlags(child, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.HeightProportional);
        AbsoluteLayout.SetLayoutBounds(child, new Rect(0, 0, 1, 1));
        Container.Children.Add(child);
    }

    private static void Cleanup(IVisualTreeElement parent)
    {
        if (parent is VisualElement visualElement)
        {
            Debug.WriteLine($"* {visualElement.GetType()}");
            visualElement.Behaviors.Clear();
            visualElement.Triggers.Clear();
            visualElement.Handler?.DisconnectHandler();
        }

        foreach (var child in parent.GetVisualChildren())
        {
            Cleanup(child);
        }
    }
}

