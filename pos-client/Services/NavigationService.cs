using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantPOS.Services;

public interface INavigationService
{
    object? CurrentView { get; }
    void NavigateTo(object viewModel);
    event PropertyChangedEventHandler? PropertyChanged;
}

public class NavigationService : INavigationService, INotifyPropertyChanged
{
    private object? _currentView;
    public object? CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public void NavigateTo(object viewModel) => CurrentView = viewModel;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
