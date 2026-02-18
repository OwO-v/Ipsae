using System.Windows;
using System.Windows.Controls;

namespace Ipsae.View.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        mainWindow?.NavigateHome();
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
    }
}