namespace Ipsae.ViewModel;

public interface INavigationService
{
    void NavigateTo(string pageKey, object? parameter = null);
    void NavigateHome();
}
