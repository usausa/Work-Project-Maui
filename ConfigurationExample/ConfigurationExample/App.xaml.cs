using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace ConfigurationExample;

public partial class App : Application
{
	public App(IConfiguration config)
	{
		InitializeComponent();

		MainPage = new MainPage();

        var settings = config.GetSection("Settings").Get<Settings>();
		Debug.WriteLine(settings.Value);
	}
}
