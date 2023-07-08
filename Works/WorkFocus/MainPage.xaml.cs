namespace WorkFocus;

using System.Diagnostics;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        DescendantAdded += (sender, args) =>
        {
            if (args.Element is VisualElement visual)
            {
                visual.Focused += OnFocused;
                visual.Unfocused += OnUnfocused;
            }
        };

        InitializeComponent();
    }

    private void OnFocused(object sender, FocusEventArgs e)
    {
        Debug.WriteLine($"*Focused {e.VisualElement.ClassId}");
    }

    private void OnUnfocused(object sender, FocusEventArgs e)
    {
        Debug.WriteLine($"*Unfocused {e.VisualElement.ClassId}");
    }
}

