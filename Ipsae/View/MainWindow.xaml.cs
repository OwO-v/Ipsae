using System.Windows;
using System.Windows.Controls;
using Ipsae.ViewModel;

namespace Ipsae.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.IsSidebarExpanded))
        {
            if (sender is MainWindowViewModel viewModel)
            {
                // Border의 ColumnSpan 변경
                var border = RootGrid.Children[0] as Border;
                if (border != null)
                {
                    Grid.SetColumnSpan(border, viewModel.IsSidebarExpanded ? 2 : 1);
                }

                // UserControl의 Column 변경
                var userControl = RootGrid.Children[1] as UIElement;
                if (userControl != null)
                {
                    Grid.SetColumn(userControl, viewModel.IsSidebarExpanded ? 2 : 1);
                    Grid.SetColumnSpan(userControl, viewModel.IsSidebarExpanded ? 1 : 2);
                }

                // Sidebar 컨텐츠 업데이트
                if (border?.Child is Grid sidebar)
                {
                    foreach (var child in sidebar.Children)
                    {
                        if (child is StackPanel stackPanel && stackPanel.Name == "LogoTitle")
                        {
                            // LogoTitle의 Image 크기 조정
                            var image = stackPanel.Children[0] as Image;
                            if (image != null)
                            {
                                image.Width = viewModel.IsSidebarExpanded ? 50 : 39;
                                image.Height = viewModel.IsSidebarExpanded ? 50 : 39;
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
                            if (button.Content is StackPanel btnStackPanel)
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
    }
}