using System.Collections.ObjectModel;
using System.Windows.Input;
using Ipsae.Model;
using Ipsae.Service;

namespace Ipsae.ViewModel;

public class IpListViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    private string _pageTitle;
    private string _ipInput = "";

    public IpListViewModel(INavigationService navigationService, string listType)
    {
        _navigationService = navigationService;
        _pageTitle = listType == "white" ? "Whitelist IP" : "Blacklist IP";

        NavigateHomeCommand = new RelayCommand(_ => _navigationService.NavigateHome());
        AddIpCommand = new RelayCommand(_ => AddIp(), _ => !string.IsNullOrWhiteSpace(IpInput));
        DeleteIpCommand = new RelayCommand(param => DeleteIp(param as IpEntry));
    }

    public ICommand NavigateHomeCommand { get; }
    public ICommand AddIpCommand { get; }
    public ICommand DeleteIpCommand { get; }

    public ObservableCollection<IpEntry> IpEntries { get; } = new();

    public string PageTitle
    {
        get => _pageTitle;
        set => SetProperty(ref _pageTitle, value);
    }

    public string IpInput
    {
        get => _ipInput;
        set => SetProperty(ref _ipInput, value);
    }

    private void AddIp()
    {
        var ip = IpInput.Trim();
        if (string.IsNullOrEmpty(ip)) return;

        IpEntries.Add(new IpEntry { IpAddress = ip });
        IpInput = "";
    }

    private void DeleteIp(IpEntry? entry)
    {
        if (entry != null)
            IpEntries.Remove(entry);
    }
}
