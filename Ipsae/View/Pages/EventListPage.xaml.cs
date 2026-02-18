using System.Windows;
using System.Windows.Controls;

namespace Ipsae.View.Pages;

public partial class EventListPage : Page
{
    public EventListPage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        mainWindow?.NavigateHome();
    }
}