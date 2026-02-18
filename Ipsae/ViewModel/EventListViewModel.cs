using System.Collections.ObjectModel;
using System.Windows.Input;
using Ipsae.Model;
using Ipsae.Service;

namespace Ipsae.ViewModel;

public class EventListViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    private int _totalCount;

    public EventListViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        NavigateHomeCommand = new RelayCommand(_ => _navigationService.NavigateHome());
    }

    public ICommand NavigateHomeCommand { get; }

    public ObservableCollection<DetectionEvent> Events { get; } = new();

    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }
}
