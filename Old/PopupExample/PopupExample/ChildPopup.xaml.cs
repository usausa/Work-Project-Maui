namespace PopupExample;

using System.Diagnostics;

using CommunityToolkit.Maui.Core;

public partial class ChildPopup
{
    public ChildPopup()
    {
        InitializeComponent();
    }

    private void ChildPopup_OnOpened(object? sender, PopupOpenedEventArgs e)
    {
        Debug.WriteLine("**** Opened");
    }

    private void ChildPopup_OnClosed(object? sender, PopupClosedEventArgs e)
    {
        Debug.WriteLine("**** Closed");
    }

    private void Button_OnTestClicked(object? sender, EventArgs e)
    {
        Debug.WriteLine("----------");
        var parent = Parent;
        while (parent is not null)
        {
            Debug.WriteLine($"**** {parent}");
            parent = parent.Parent;
        }

        Debug.WriteLine("----------");
        foreach (var child in Parent.GetVisualTreeDescendants())
        {
            Debug.WriteLine($"**** {child}");
        }
    }

    private void Button_OnCloseClicked(object? sender, EventArgs e)
    {
        Close();
    }
}