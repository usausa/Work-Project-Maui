using WorkEffect.Behaviors;

namespace WorkEffect;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void CounterBtn_OnClicked(object sender, EventArgs e)
    {
        //Entry1.IsEnabled = !Entry1.IsEnabled;
        //Entry2.IsEnabled = !Entry2.IsEnabled;

        //if (Entry1.Behaviors.Count > 0)
        //{
        //    Entry1.Behaviors.Clear();
        //}
        //else
        //{
        //    Entry1.Behaviors.Add(new NoBorderBehavior());
        //}

        //var on = NoBorder.GetOn(Entry1);
        //NoBorder.SetOn(Entry1, !on);

        var on = NoBorder2.GetOn(Entry1);
        NoBorder2.SetOn(Entry1, !on);
    }
}

