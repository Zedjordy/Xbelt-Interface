using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Windows.Input;

namespace SettingsClone.Views.Controls;

public class PageButton : Button
{
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(PageButton), new PropertyMetadata(""));

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(PageButton), new PropertyMetadata(""));

    public Symbol IconLeft
    {
        get => (Symbol)GetValue(IconLeftProperty);
        set => SetValue(IconLeftProperty, value);
    }
    public static readonly DependencyProperty IconLeftProperty =
        DependencyProperty.Register(nameof(IconLeft), typeof(Symbol), typeof(PageButton), new PropertyMetadata(Symbol.Home));

    public Symbol IconRight
    {
        get => (Symbol)GetValue(IconRightProperty);
        set => SetValue(IconRightProperty, value);
    }
    public static readonly DependencyProperty IconRightProperty =
        DependencyProperty.Register(nameof(IconRight), typeof(Symbol), typeof(PageButton), new PropertyMetadata(Symbol.Forward));
}