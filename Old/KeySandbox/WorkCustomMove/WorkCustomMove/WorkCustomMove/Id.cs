using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace WorkCustomMove
{
    public static class Debug
    {
        public static readonly BindableProperty IdProperty = BindableProperty.CreateAttached(
            "Id",
            typeof(string),
            typeof(Debug),
            null);

        public static string GetId(BindableObject view)
        {
            return (string)view.GetValue(IdProperty);
        }

        public static void SetId(BindableObject view, string value)
        {
            view.SetValue(IdProperty, value);
        }
    }
}
