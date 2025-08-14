using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace RestaurantPOS.ViewModels;

public partial class AdminPanelViewModel : ViewModelBase
{

    private readonly IImageStorage _imageStorage;

    public AdminPanelViewModel(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }
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
    private void OpenUsers() => CurrentAdminView = App.Services.GetRequiredService<AdminUsersViewModel>();

    [RelayCommand]
    private void OpenHalls() => CurrentAdminView = App.Services.GetRequiredService<AdminHallsViewModel>();

    [RelayCommand]
    private void OpenTables() => CurrentAdminView = App.Services.GetRequiredService<AdminTablesViewModel>();

    // [RelayCommand]
    // private void OpenReports() => CurrentAdminView = new ReportsAdminViewModel();
}
