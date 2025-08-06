namespace WorkSample.Pages;
using WorkSample.Models;

public partial class ProjectDetailPage : ContentPage
{
    public ProjectDetailPage(ProjectDetailPageModel model)
    {
        InitializeComponent();

        BindingContext = model;
    }
}
