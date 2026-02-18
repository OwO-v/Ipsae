using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ipsae.Behavior;

public class TileHoverBehavior
{
    private static readonly Duration AnimDuration = new(TimeSpan.FromMilliseconds(250));
    private static readonly Color DefaultBorderColor = Color.FromRgb(0x1A, 0x1A, 0x1A);
    private static readonly Color DefaultBgColor = Color.FromRgb(0x11, 0x11, 0x11);
    private static readonly Color HoverBgColor = Color.FromRgb(0x14, 0x14, 0x14);

    public static readonly DependencyProperty HoverBorderColorProperty =
        DependencyProperty.RegisterAttached(
            "HoverBorderColor",
            typeof(Color),
            typeof(TileHoverBehavior),
            new PropertyMetadata(Colors.Transparent, OnHoverBorderColorChanged));

    public static Color GetHoverBorderColor(DependencyObject obj) =>
        (Color)obj.GetValue(HoverBorderColorProperty);

    public static void SetHoverBorderColor(DependencyObject obj, Color value) =>
        obj.SetValue(HoverBorderColorProperty, value);

    private static void OnHoverBorderColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Button button) return;

        button.MouseEnter -= OnMouseEnter;
        button.MouseLeave -= OnMouseLeave;

        if ((Color)e.NewValue != Colors.Transparent)
        {
            button.MouseEnter += OnMouseEnter;
            button.MouseLeave += OnMouseLeave;
        }
    }

    private static void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is not Button button) return;

        var targetColor = GetHoverBorderColor(button);
        var border = FindChildBorder(button);
        if (border == null) return;

        // Replace frozen brushes with mutable ones, preserving current color
        EnsureMutableBrushes(border);

        var borderAnim = new ColorAnimation(targetColor, AnimDuration)
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);

        var bgAnim = new ColorAnimation(HoverBgColor, AnimDuration)
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        border.Background.BeginAnimation(SolidColorBrush.ColorProperty, bgAnim);
    }

    private static void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is not Button button) return;

        var border = FindChildBorder(button);
        if (border == null) return;

        EnsureMutableBrushes(border);

        var borderAnim = new ColorAnimation(DefaultBorderColor, AnimDuration)
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);

        var bgAnim = new ColorAnimation(DefaultBgColor, AnimDuration)
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        border.Background.BeginAnimation(SolidColorBrush.ColorProperty, bgAnim);
    }

    private static void EnsureMutableBrushes(Border border)
    {
        if (border.BorderBrush is not SolidColorBrush borderBrush || borderBrush.IsFrozen)
        {
            var currentColor = (border.BorderBrush as SolidColorBrush)?.Color ?? DefaultBorderColor;
            border.BorderBrush = new SolidColorBrush(currentColor);
        }

        if (border.Background is not SolidColorBrush bgBrush || bgBrush.IsFrozen)
        {
            var currentColor = (border.Background as SolidColorBrush)?.Color ?? DefaultBgColor;
            border.Background = new SolidColorBrush(currentColor);
        }
    }

    private static Border? FindChildBorder(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is Border border)
                return border;
            var result = FindChildBorder(child);
            if (result != null)
                return result;
        }
        return null;
    }
}
