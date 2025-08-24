#if ANDROID
namespace Template.MobileApp.Behaviors;

using Android.Content.Res;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Java.Lang;

using Microsoft.Maui.Handlers;

public static partial class EntryOption
{
    public static void UseCustomMapper()
    {
        EntryHandler.Mapper.AppendToMapping(NoBorderProperty.PropertyName, static (handler, _) => UpdateHandleNoBorder(handler.PlatformView, (Entry)handler.VirtualView));
        EditorHandler.Mapper.AppendToMapping(NoBorderProperty.PropertyName, static (handler, _) => UpdateHandleNoBorder(handler.PlatformView, (Editor)handler.VirtualView));
    }

    private static void UpdateHandleNoBorder(TextView editText, BindableObject element)
    {
        var value = GetNoBorder(element);
        editText.BackgroundTintList = value ? ColorStateList.ValueOf(Android.Graphics.Color.Transparent) : null;
    }
}
#endif