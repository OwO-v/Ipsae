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

        AttachButtonClickHandlers();

        Dispatcher.InvokeAsync(() =>
        {
            LoadPage(0);
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {

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

    private void NavigateTo(System.Windows.Controls.Page page)
    {
        HomeGrid.Visibility = Visibility.Collapsed;
        MainFrame.Visibility = Visibility.Visible;
        MainFrame.Navigate(page);
    }

    public void NavigateHome()
    {
        MainFrame.Content = null;
        MainFrame.Visibility = Visibility.Collapsed;
        HomeGrid.Visibility = Visibility.Visible;
    }

    private void TileStatus_Click(object sender, RoutedEventArgs e)
        => NavigateTo(new ServiceStatusPage());

    private void TileEvents_Click(object sender, RoutedEventArgs e)
        => NavigateTo(new EventListPage());

    private void TileWhitelist_Click(object sender, RoutedEventArgs e)
        => NavigateTo(new IpListPage("white"));

    private void TileBlacklist_Click(object sender, RoutedEventArgs e)
        => NavigateTo(new IpListPage("black"));

    private void TileSettings_Click(object sender, RoutedEventArgs e)
        => NavigateTo(new SettingsPage());

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal : WindowState.Maximized;

    private void CloseButton_Click(object sender, RoutedEventArgs e)
        => this.Hide();
}