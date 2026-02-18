using Ipsae.View.Pages;
using Ipsae.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ipsae.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Button? _activeNavButton;
    private readonly List<Button> _navButtons = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        _navButtons.Add(StatusButton);
        _navButtons.Add(EventButton);
        _navButtons.Add(WhitelistButton);
        _navButtons.Add(BlacklistButton);
        _navButtons.Add(SettingButton);

        AttachButtonClickHandlers();

        Dispatcher.InvokeAsync(() =>
        {
            LoadPage(0);
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.IsSidebarExpanded))
        {
            if (sender is MainWindowViewModel viewModel)
            {
                Grid.SetColumnSpan(SidebarBorder, viewModel.IsSidebarExpanded ? 2 : 1);
                Grid.SetColumn(MainFrame, viewModel.IsSidebarExpanded ? 2 : 1);
                Grid.SetColumnSpan(MainFrame, viewModel.IsSidebarExpanded ? 2 : 3);

                foreach (var child in SidebarGrid.Children)
                {
                    if (child is StackPanel stackPanel && stackPanel.Name == "LogoTitle")
                    {
                        var image = stackPanel.Children[0] as Image;
                        if (image != null)
                        {
                            image.Width = viewModel.IsSidebarExpanded ? 50 : 35;
                            image.Height = viewModel.IsSidebarExpanded ? 50 : 35;
                        }

                        var textBlock = stackPanel.Children[1] as TextBlock;
                        if (textBlock != null)
                        {
                            textBlock.Visibility = viewModel.IsSidebarExpanded
                                ? Visibility.Visible
                                : Visibility.Collapsed;
                        }
                    }
                    else if (child is Button button)
                    {
                        if (button.Content is StackPanel btnStackPanel && btnStackPanel.Children.Count > 1)
                        {
                            var btnTextBlock = btnStackPanel.Children[1] as TextBlock;
                            if (btnTextBlock != null)
                            {
                                btnTextBlock.Visibility = viewModel.IsSidebarExpanded
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }
    }

    private void AttachButtonClickHandlers()
    {
        for (int i = 0; i < _navButtons.Count; i++)
        {
            int pageIndex = i;
            _navButtons[i].Click += (s, e) => LoadPage(pageIndex);
        }
    }

    private void LoadPage(int pageIndex)
    {
        Page? page = pageIndex switch
        {
            0 => new ServiceStatusPage(),
            1 => new ServiceStatusPage(),
            2 => new ServiceStatusPage(),
            3 => new ServiceStatusPage(),
            4 => new ServiceStatusPage(),
            _ => null
        };

        if (page != null)
        {
            MainFrame.Content = null;
            Dispatcher.InvokeAsync(() =>
            {
                MainFrame.Content = page;
            }, System.Windows.Threading.DispatcherPriority.Background);

            SetActiveNavButton(pageIndex);
        }
    }

    private void SetActiveNavButton(int index)
    {
        if (index < 0 || index >= _navButtons.Count)
            return;

        var activeStyle = (Style)FindResource("NavButtonActiveStyle");
        var normalStyle = (Style)FindResource("NavButtonStyle");
        var accentBlueBrush = (SolidColorBrush)FindResource("AccentBlueBrush");
        var textSecondaryBrush = (SolidColorBrush)FindResource("TextSecondaryBrush");

        foreach (var button in _navButtons)
        {
            button.Style = normalStyle;
            
            if (button.Content is StackPanel stackPanel && stackPanel.Children.Count > 1)
            {
                if (stackPanel.Children[1] is TextBlock textBlock)
                {
                    textBlock.Foreground = textSecondaryBrush;
                }
            }
        }

        var selectedButton = _navButtons[index];
        selectedButton.Style = activeStyle;
        
        if (selectedButton.Content is StackPanel selectedStackPanel && selectedStackPanel.Children.Count > 1)
        {
            if (selectedStackPanel.Children[1] is TextBlock selectedTextBlock)
            {
                selectedTextBlock.Foreground = accentBlueBrush;
            }
        }

        _activeNavButton = selectedButton;
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
            this.WindowState = WindowState.Normal;
        else
            this.WindowState = WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Hide();
    }
}