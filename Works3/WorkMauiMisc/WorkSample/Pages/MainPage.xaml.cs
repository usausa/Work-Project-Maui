namespace WorkSample.Pages;
using WorkSample.Models;
using WorkSample.PageModels;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}