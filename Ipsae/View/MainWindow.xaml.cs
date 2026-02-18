using Ipsae.ViewModel;
using System.Windows;

namespace Ipsae.View;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
