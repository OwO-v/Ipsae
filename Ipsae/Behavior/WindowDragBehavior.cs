using System.Windows;
using System.Windows.Input;

namespace Ipsae.Behavior;

public class WindowDragBehavior
{
    public static bool GetIsEnabled(UIElement obj)
    {
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(UIElement obj, bool value)
    {
        obj.SetValue(IsEnabledProperty, value);
    }

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(WindowDragBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element && (bool)e.NewValue)
        {
            element.MouseDown += OnMouseDown;
        }
        else if (d is UIElement elem)
        {
            elem.MouseDown -= OnMouseDown;
        }
    }

    private static void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var window = Window.GetWindow((DependencyObject)sender);
            window?.DragMove();
        }
    }
}
