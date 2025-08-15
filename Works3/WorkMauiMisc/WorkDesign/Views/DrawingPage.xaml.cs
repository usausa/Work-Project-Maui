using CommunityToolkit.Maui.Core;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Collections.ObjectModel;
using Smart.Maui.Input;

namespace WorkDesign;

public partial class DrawingPage : ContentPage
{
	public DrawingPage()
	{
		InitializeComponent();
	}
}

public sealed partial class DrawingPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<IDrawingLine> Lines { get; } = new();

	public IObserveCommand ClearCommand { get; }

    public DrawingPageViewModel()
	{
		ClearCommand = MakeDelegateCommand(() => Lines.Clear());
    }
}