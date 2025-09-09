using System.Collections.ObjectModel;
using System.Windows.Input;
using Smart.Collections.Generic;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkDesign;

public partial class BasicRefreshPage : ContentPage
{
	public BasicRefreshPage()
	{
		InitializeComponent();
	}
}

public partial class BasicRefreshPageViewModel : ExtendViewModelBase
{
	[ObservableProperty]
	public partial bool IsRefreshing { get; set; }

    public ObservableCollection<BasicEntity> Results { get; } = new();

	public ICommand RefreshCommand { get; }

    private int counter = 100;

    public BasicRefreshPageViewModel()
    {
        RefreshCommand = MakeAsyncCommand(async () =>
        {
            IsRefreshing = true;

            await Task.Delay(1000).ConfigureAwait(true);
            counter++;
            Results.Insert(0, new BasicEntity { Id = counter, Group = "x", Name = $"Name-{counter}" });

            IsRefreshing = false;
        });
        Results.AddRange(BasicService.GetData());
    }
}