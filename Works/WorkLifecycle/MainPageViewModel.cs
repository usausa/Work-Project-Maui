namespace WorkLifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MainPageViewModel : IAppLifecycle
{
    public void OnCreated()
    {
        System.Diagnostics.Debug.WriteLine("* OnCreated");
    }

    public void OnActivated()
    {
        System.Diagnostics.Debug.WriteLine("* OnActivated");
    }

    public void OnDeactivated()
    {
        System.Diagnostics.Debug.WriteLine("* OnDeactivated");
    }

    public void OnStopped()
    {
        System.Diagnostics.Debug.WriteLine("* OnStopped");
    }

    public void OnResumed()
    {
        System.Diagnostics.Debug.WriteLine("* OnResumed");
    }

    public void OnDestroying()
    {
        System.Diagnostics.Debug.WriteLine("* OnDestroying");
    }
}
