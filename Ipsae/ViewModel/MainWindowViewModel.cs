using System.Windows;
using System.Windows.Input;

namespace Ipsae.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private ICommand? _dragMoveCommand;
    private bool _isSidebarExpanded = true;

    public ICommand DragMoveCommand
    {
        get { return _dragMoveCommand ??= new RelayCommand(DragMove); }
    }

    public bool IsSidebarExpanded
    {
        get { return _isSidebarExpanded; }
        set { SetProperty(ref _isSidebarExpanded, value); }
    }

    private void DragMove(object? parameter)
    {
        if (parameter is MouseButtonEventArgs e && e.LeftButton == MouseButtonState.Pressed)
        {
            if (Application.Current.MainWindow is Window window)
            {
                window.DragMove();
            }
        }
    }
}
