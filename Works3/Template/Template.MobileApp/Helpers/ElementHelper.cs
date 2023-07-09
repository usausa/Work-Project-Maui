namespace Template.MobileApp.Helpers;

using CommunityToolkit.Maui.Views;

using Microsoft.Maui;

public static class ElementHelper
{
    public static VisualElement? FindFocused(IVisualTreeElement parent)
    {
        foreach (var child in parent.GetVisualChildren())
        {
            if ((child is VisualElement visual) && visual.IsFocused)
            {
                return visual;
            }

            var focused = FindFocused(child);
            if (focused is not null)
            {
                return focused;
            }
        }

        return null;
    }

    public static IEnumerable<VisualElement> EnumerateActive(IVisualTreeElement parent)
    {
        foreach (var child in parent.GetVisualChildren())
        {
            if ((child is not VisualElement visual) || !visual.IsEnabled || !visual.IsVisible)
            {
                continue;
            }

            yield return visual;

            foreach (var descendant in EnumerateActive(child))
            {
                yield return descendant;
            }
        }
    }

    public static IEnumerable<T> EnumerateActive<T>(IVisualTreeElement parent)
    {
        foreach (var child in parent.GetVisualChildren())
        {
            if ((child is not VisualElement visual) || !visual.IsEnabled || !visual.IsVisible)
            {
                continue;
            }

            if (child is T element)
            {
                yield return element;
            }

            foreach (var descendant in EnumerateActive<T>(child))
            {
                yield return descendant;
            }
        }
    }

    // TODO Nativeでやれるか？

    private static bool IsFocusableElement(VisualElement visualElement) =>
        visualElement is Button ||
        visualElement is CheckBox ||
        visualElement is DatePicker ||
        visualElement is Editor ||
        visualElement is Entry ||
        visualElement is ImageButton ||
        visualElement is ListView ||
        visualElement is Picker ||
        visualElement is RadioButton ||
        visualElement is SearchBar ||
        visualElement is Slider ||
        visualElement is Stepper ||
        visualElement is Switch ||
        visualElement is TimePicker;

    public static bool MoveFocus(VisualElement parent, bool forward)
    {
        var find = false;
        var first = default(VisualElement);
        var previous = default(VisualElement);
        foreach (var visual in EnumerateActive(parent))
        {
            if (IsFocusableElement(visual))
            {
                continue;
            }

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

    public static bool MoveFocusInRoot(VisualElement current, bool forward)
    {
        var parent = FindRoot(current);
        if (parent is null)
        {
            return false;
        }

        var find = false;
        var first = default(VisualElement);
        var previous = default(VisualElement);
        foreach (var visual in EnumerateActive(parent))
        {
            if (IsFocusableElement(visual))
            {
                continue;
            }

            if (visual == current)
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

    public static VisualElement? FindRoot(this Element element)
    {
        while (true)
        {
            var parent = element.Parent;
            if (parent is null)
            {
                return null;
            }

            if (element is Page page)
            {
                return page;
            }

            if (element is Popup popup)
            {
                return popup.Content;
            }

            element = parent;
        }
    }
}
