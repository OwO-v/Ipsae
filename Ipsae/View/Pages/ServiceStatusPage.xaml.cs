using System.Windows;
using System.Windows.Controls;

namespace Ipsae.View.Pages;

public partial class ServiceStatusPage : Page
{
    public ServiceStatusPage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        mainWindow?.NavigateHome();
    }

    private void TabStatusButton_Click(object sender, RoutedEventArgs e)
    {
        TabStatusButton.Style = (Style)FindResource("PanelTabButtonActive");
        TabStatsButton.Style = (Style)FindResource("PanelTabButton");
        StatusPanel.Visibility = Visibility.Visible;
        StatsPanel.Visibility = Visibility.Collapsed;
    }

    private void TabStatsButton_Click(object sender, RoutedEventArgs e)
    {
        TabStatsButton.Style = (Style)FindResource("PanelTabButtonActive");
        TabStatusButton.Style = (Style)FindResource("PanelTabButton");
        StatsPanel.Visibility = Visibility.Visible;
        StatusPanel.Visibility = Visibility.Collapsed;
    }

    private void ToggleServiceButton_Click(object sender, RoutedEventArgs e)
    {
    }
}