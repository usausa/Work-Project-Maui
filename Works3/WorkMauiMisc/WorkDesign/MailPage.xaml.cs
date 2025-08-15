namespace WorkDesign;

using Smart.Maui.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel.Design;

public partial class MailPage : ContentPage
{
	public MailPage()
	{
		InitializeComponent();
	}
}

public partial class MailPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<MailMessage> Messages { get; } = [];

    public MailPageViewModel()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        Messages.Add(new MailMessage
        {
        });
        Messages.Add(new MailMessage
        {
        });
        Messages.Add(new MailMessage
        {
        });
        Messages.Add(new MailMessage
        {
        });
        Messages.Add(new MailMessage
        {
        });
    }
}

public sealed class MailMessage
{
    public DateTime DateTime { get; set; }

    // TODO
}
