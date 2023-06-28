#nullable enable
using Android.Text;
using Java.Lang;
using Microsoft.Maui.Handlers;

namespace WorkEffect.Behaviors;

public static class InputFilter
{
    public static readonly BindableProperty RuleProperty = BindableProperty.CreateAttached(
        "Rule",
        typeof(Func<string, bool>),
        typeof(InputFilter),
        null);

    public static Func<string, bool>? GetRule(BindableObject bindable) => (Func<string, bool>?)bindable.GetValue(RuleProperty);

    public static void SetRule(BindableObject bindable, Func<string, bool>? value) => bindable.SetValue(RuleProperty, value);

    public static void UseCustomMapper()
    {
#if ANDROID
        EntryHandler.Mapper.Add("Rule", (handler, _) =>
        {
            var element = (Entry)handler.VirtualView;
            var rule = GetRule(element);

            handler.PlatformView.SetFilters(rule is null ? Array.Empty<IInputFilter>() : new IInputFilter[] { new RuleInputFilter(rule) });
        });
#endif
    }

#if ANDROID
    private class RuleInputFilter : Java.Lang.Object, IInputFilter
    {
        private readonly Func<string, bool> rule;

        public RuleInputFilter(Func<string, bool> rule)
        {
            this.rule = rule;
        }

        public ICharSequence? FilterFormatted(ICharSequence? source, int start, int end, ISpanned? dest, int dstart, int dend)
        {
            var value = dest!.SubSequence(0, dstart) + source!.SubSequence(start, end) + dest!.SubSequence(dend, dest!.Length());
            return rule(value) ? source : new Java.Lang.String(dest.SubSequence(dstart, dend));
        }
    }
#endif
}
