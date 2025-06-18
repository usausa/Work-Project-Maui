using CommunityToolkit.Maui.Extensions;

namespace WorkPopup2;

public partial class InputDialog
{
	public InputDialog()
	{
		InitializeComponent();
	}

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Application.Current!.Windows[0].Page!.ClosePopupAsync(123);
    }
}