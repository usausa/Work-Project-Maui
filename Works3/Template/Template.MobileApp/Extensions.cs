namespace Template.MobileApp;

using System.Reflection;

using Template.MobileApp.Behaviors;
using Template.MobileApp.Components;
using Template.MobileApp.Helpers;

public static class Extensions
{
    //--------------------------------------------------------------------------------
    // Resource
    //--------------------------------------------------------------------------------

    public static T FindResource<T>(this ResourceDictionary resource, string key) =>
        resource.TryGetValue(key, out var value) ? (T)value : default!;

    public static IEnumerable<Type> UnderNamespaceTypes(this Assembly assembly, Type baseNamespaceType)
    {
        var ns = baseNamespaceType.Namespace!;
        return assembly.ExportedTypes.Where(x => x.Namespace?.StartsWith(ns, StringComparison.Ordinal) ?? false);
    }

    //--------------------------------------------------------------------------------
    // Element
    //--------------------------------------------------------------------------------

    public static void SetDefaultFocus(this IVisualTreeElement parent)
    {
        var first = default(VisualElement);
        foreach (var visualElement in ElementHelper.EnumerateFocusable(parent))
        {
            if (Focus.GetDefault(visualElement))
            {
                visualElement.Focus();
                return;
            }

            first ??= visualElement;
        }

        first?.Focus();
    }

    //--------------------------------------------------------------------------------
    // Navigation
    //--------------------------------------------------------------------------------

    // ReSharper disable once AsyncVoidMethod
    public static async ValueTask PostForwardAsync(this INavigator navigator, object viewId, NavigationParameter? parameter = null)
    {
        if (navigator.Executing)
        {
            async void ExecutingChanged(object? sender, EventArgs args)
            {
                if (!navigator.Executing)
                {
                    navigator.ExecutingChanged -= ExecutingChanged;
                    await navigator.ForwardAsync(viewId, parameter);
                }
            }

            navigator.ExecutingChanged += ExecutingChanged;
        }
        else
        {
            await navigator.ForwardAsync(viewId, parameter);
        }
    }

    // ReSharper disable once AsyncVoidMethod
    public static async ValueTask PostActionAsync(this INavigator navigator, Func<Task> task)
    {
        if (navigator.Executing)
        {
            async void ExecutingChanged(object? sender, EventArgs args)
            {
                if (!navigator.Executing)
                {
                    navigator.ExecutingChanged -= ExecutingChanged;
                    await task();
                }
            }

            navigator.ExecutingChanged += ExecutingChanged;
        }
        else
        {
            await task();
        }
    }

    //--------------------------------------------------------------------------------
    // Reactive
    //--------------------------------------------------------------------------------

    // TODO Smartへ移動？
    // TODO これはベースへ移動？、結合は分離、呼び出し箇所修正

    public static IObservable<TSource> WhereNotNull<TSource>(this IObservable<TSource?> source) =>
        source.Where(static x => x is not null)!;

    public static IObservable<TSource> ObserveOnCurrentContext<TSource>(this IObservable<TSource> source) =>
        source.ObserveOn(SynchronizationContext.Current!);

    public static IObservable<ScreenStateEventArgs> ObserveStateChanged(this IScreen screen) =>
        Observable.FromEvent<EventHandler<ScreenStateEventArgs>, ScreenStateEventArgs>(static h => (_, e) => h(e), h => screen.ScreenStateChanged += h, h => screen.ScreenStateChanged -= h);

    public static IObservable<ScreenStateEventArgs> ObserveStateChangedOnCurrentContext(this IScreen screen) =>
        screen.ObserveStateChanged().ObserveOnCurrentContext();

    public static IObservable<LocationEventArgs> ObserveLocationChanged(this ILocationService locationService) =>
        Observable.FromEvent<EventHandler<LocationEventArgs>, LocationEventArgs>(static h => (_, e) => h(e), h => locationService.LocationChanged += h, h => locationService.LocationChanged -= h);

    public static IObservable<LocationEventArgs> ObserveLocationChangedOnCurrentContext(this ILocationService locationService) =>
        locationService.ObserveLocationChanged().ObserveOnCurrentContext();

    public static IObservable<SpeechRecognizeEventArgs> ObserveRecognized(this ISpeechService speechService) =>
        Observable.FromEvent<EventHandler<SpeechRecognizeEventArgs>, SpeechRecognizeEventArgs>(static h => (_, e) => h(e), h => speechService.Recognized += h, h => speechService.Recognized -= h);

    public static IObservable<SpeechRecognizeEventArgs> ObserveRecognizedOnCurrentContext(this ISpeechService speechService) =>
        speechService.ObserveRecognized().ObserveOnCurrentContext();

    public static IObservable<NfcEventArgs> ObserveDetected(this INfcReader nfcReader) =>
        Observable.FromEvent<EventHandler<NfcEventArgs>, NfcEventArgs>(static h => (_, e) => h(e), h => nfcReader.Detected += h, h => nfcReader.Detected -= h);

    public static IObservable<NfcEventArgs> ObserveDetectedOnCurrentContext(this INfcReader nfcReader) =>
        nfcReader.ObserveDetected().ObserveOnCurrentContext();

    public static IObservable<NoiseEventArgs> ObserveMeasured(this INoiseMonitor noiseMonitor) =>
        Observable.FromEvent<EventHandler<NoiseEventArgs>, NoiseEventArgs>(static h => (_, e) => h(e), h => noiseMonitor.Measured += h, h => noiseMonitor.Measured -= h);

    public static IObservable<NoiseEventArgs> ObserveMeasuredOnCurrentContext(this INoiseMonitor noiseMonitor) =>
        noiseMonitor.ObserveMeasured().ObserveOnCurrentContext();
}
