using System.Windows;
using System.Windows.Controls;

namespace Ipsae.View.Pages;

public partial class IpListPage : Page
{
    private readonly string _listType;

    public IpListPage(string listType)
    {
        InitializeComponent();
        _listType = listType;
        PageTitle.Text = listType == "white" ? "Whitelist IP" : "Blacklist IP";
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        mainWindow?.NavigateHome();
    }

    private void AddIpButton_Click(object sender, RoutedEventArgs e)
    {
        var ip = IpInputBox.Text.Trim();
        if (string.IsNullOrEmpty(ip)) return;
        IpInputBox.Clear();
    }
}