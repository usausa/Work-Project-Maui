namespace KeySample.FormsApp.Input
{
    using System.Collections.Generic;

    using Smart.Forms;

    using Xamarin.Forms;

    public static class FocusHelper
    {
        public static bool MoveFocus(VisualElement parent, bool forward)
        {
            var find = false;
            var first = default(VisualElement);
            var previous = default(VisualElement);
            foreach (var visual in EnumerateFocusable(parent))
            {
                if (visual.IsFocused)
                {
                    if (forward)
                    {
                        find = true;
                    }
                    else
                    {
                        return previous?.Focus() ?? false;
                    }
                }
                else if (find)
                {
                    return visual.Focus();
                }

                previous = visual;
                first ??= visual;
            }

            if (!find)
            {
                return first?.Focus() ?? false;
            }

            return false;
        }

        public static bool MoveFocusInPage(VisualElement element, bool forward)
        {
            var page = element.FindParent<Page>();
            if (page is null)
            {
                return false;
            }

            var find = false;
            var first = default(VisualElement);
            var previous = default(VisualElement);
            foreach (var visual in EnumerateFocusable(page))
            {
                if (visual == element)
                {
                    if (forward)
                    {
                        find = true;
                    }
                    else
                    {
                        return previous?.Focus() ?? false;
                    }
                }
                else if (find)
                {
                    return visual.Focus();
                }

                previous = visual;
                first ??= visual;
            }

            if (!find)
            {
                return first?.Focus() ?? false;
            }

            return false;
        }

        private static IEnumerable<VisualElement> EnumerateFocusable(Element parent)
        {
            foreach (var child in parent.LogicalChildren)
            {
                if (child is VisualElement visualElement)
                {
                    if (!visualElement.IsEnabled || !visualElement.IsVisible)
                    {
                        continue;
                    }

                    if (visualElement.IsTabStop && (visualElement is not Layout))
                    {
                        yield return visualElement;
                    }
                }

                foreach (var descendant in EnumerateFocusable(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
