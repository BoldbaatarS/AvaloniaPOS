using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RestaurantPOS.ViewModels;

public partial class AdminPanelViewModel : ViewModelBase
{
    [ObservableProperty] private double sidebarWidth = 200;
    [ObservableProperty] private bool isCollapsed;
    [ObservableProperty] private object? currentAdminView;

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsCollapsed = !IsCollapsed;
        SidebarWidth = IsCollapsed ? 50 : 200;
      
    }

    [RelayCommand]
    private void OpenUsers() => CurrentAdminView = new AdminUsersViewModel();

    [RelayCommand]
    private void OpenHalls() => CurrentAdminView = new AdminHallsViewModel();

    [RelayCommand]
    private void OpenTables() => CurrentAdminView = new AdminTablesViewModel();

    // [RelayCommand]
    // private void OpenReports() => CurrentAdminView = new ReportsAdminViewModel();
}
