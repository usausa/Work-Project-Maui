namespace Template.MobileApp.Controls;

public sealed partial class SideDrawer
{
    private partial void InitializePanelGesture()
    {
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        panel.GestureRecognizers.Add(pan);
    }
}
