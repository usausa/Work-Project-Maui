namespace WorkLifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

public sealed class AppLifecycleBehavior : Behavior<Page>
{
    private Page? page;

    private Window? window;

    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.ParentChanged += BindableOnParentChanged;

        page = bindable;

        if (page?.Parent is Window w)
        {
            AttachEvent(w);
        }
    }

    protected override void OnDetachingFrom(Page bindable)
    {
        if (page is not null)
        {
            page.PropertyChanged -= BindableOnParentChanged;
        }

        page = null;

        DetachEvent();

        base.OnDetachingFrom(bindable);
    }

    private void BindableOnParentChanged(object sender, EventArgs e)
    {
        DetachEvent();
        if (page?.Parent is Window w)
        {
            AttachEvent(w);
        }
    }

    private void AttachEvent(Window? w)
    {
        if (w is null)
        {
            return;
        }

        window = w;
        window.Created += WindowOnCreated;
        window.Activated += WindowOnActivated;
        window.Deactivated += WindowOnDeactivated;
        window.Stopped += WindowOnStopped;
        window.Resumed += WindowOnResumed;
        window.Destroying += WindowOnDestroying;
    }

    private void DetachEvent()
    {
        if (window is null)
        {
            return;
        }

        window.Created -= WindowOnCreated;
        window.Activated -= WindowOnActivated;
        window.Deactivated -= WindowOnDeactivated;
        window.Stopped -= WindowOnStopped;
        window.Resumed -= WindowOnResumed;
        window.Destroying -= WindowOnDestroying;
        window = null;
    }

    private void WindowOnCreated(object sender, EventArgs e)
    {
        if (page is IAppLifecycle lifecycle)
        {
            lifecycle.OnCreated();
        }

        if (page?.BindingContext is IAppLifecycle contextLifecycle)
        {
            contextLifecycle.OnCreated();
        }
    }

    private void WindowOnActivated(object sender, EventArgs e)
    {
        if (page is IAppLifecycle lifecycle)
        {
            lifecycle.OnActivated();
        }

        if (page?.BindingContext is IAppLifecycle contextLifecycle)
        {
            contextLifecycle.OnActivated();
        }
    }

    private void WindowOnDeactivated(object sender, EventArgs e)
    {
        if (page is IAppLifecycle lifecycle)
        {
            lifecycle.OnDeactivated();
        }

        if (page?.BindingContext is IAppLifecycle contextLifecycle)
        {
            contextLifecycle.OnDeactivated();
        }
    }

    private void WindowOnStopped(object sender, EventArgs e)
    {
        if (page is IAppLifecycle lifecycle)
        {
            lifecycle.OnStopped();
        }

        if (page?.BindingContext is IAppLifecycle contextLifecycle)
        {
            contextLifecycle.OnStopped();
        }
    }

    private void WindowOnResumed(object sender, EventArgs e)
    {
        if (page is IAppLifecycle lifecycle)
        {
            lifecycle.OnResumed();
        }

        if (page?.BindingContext is IAppLifecycle contextLifecycle)
        {
            contextLifecycle.OnResumed();
        }
    }

    private void WindowOnDestroying(object sender, EventArgs e)
    {
        if (page is IAppLifecycle lifecycle)
        {
            lifecycle.OnDestroying();
        }

        if (page?.BindingContext is IAppLifecycle contextLifecycle)
        {
            contextLifecycle.OnDestroying();
        }
    }
}
