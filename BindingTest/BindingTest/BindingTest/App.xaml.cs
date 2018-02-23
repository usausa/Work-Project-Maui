[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace BindingTest
{
    using Smart.Forms.Resolver;
    using Smart.Resolver;

    public partial class App
	{
		public App ()
		{
			InitializeComponent();

		    var resolver = CreateResolver();
		    ResolveProvider.Default.UseSmartResolver(resolver);

            MainPage = new MainPage();
		}

	    private SmartResolver CreateResolver()
	    {
	        var config = new ResolverConfig()
	            .UseAutoBinding()
	            .UseArrayBinding()
	            //.UseAssignableBinding()
	            .UsePropertyInjector();

	        return config.ToResolver();
	    }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
