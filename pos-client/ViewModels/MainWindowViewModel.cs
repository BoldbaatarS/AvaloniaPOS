using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantPOS.Services;
using RestaurantPOS.Views;
using RestaurantPOS.Views.Dialogs;
using Shared.Abstractions;


namespace RestaurantPOS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    public object? CurrentView => _navigation.CurrentView;

    [ObservableProperty]
    private string? currentUser;

    [ObservableProperty]
    private string statusMessage = "Тавтай морил!";

    [ObservableProperty]
    private bool isAdmin;
      
    public MainWindowViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        

        // NavigationService доторх өөрчлөлтийг сонсох
        _navigation.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(NavigationService.CurrentView))
                OnPropertyChanged(nameof(CurrentView));
        };

        // Эхлэхэд HomeView харуулах
       _navigation.NavigateTo<HomeViewModel>();
    }

    [RelayCommand]
    public void Home() => _navigation.NavigateTo<HomeViewModel>();

    [RelayCommand]
    public async Task Admin(Window owner)
    {
        if (App.AuthService.CurrentUser?.IsAdmin == true)
        {
            // Админ бол нэвтрэх
            _navigation.NavigateTo<AdminPanelViewModel>();
        }
        else
        {
            await MessageBoxService.ShowAsync(owner, "Хандалт хориглогдсон", "Таны эрх хүрэхгүй байна.", MessageDialog.DialogType.Warning);
        }
      
    }
    [RelayCommand] public void Reports() => _navigation.NavigateTo(null!);//new ReportsViewModel()
    [RelayCommand] public void Settings() => _navigation.NavigateTo(null!);// new SettingsViewModel()

    [RelayCommand]
    private async Task Logout(Window owner)
    {
        App.AuthService.Logout();

        // Logout → LoginWindow дахин харуулах
        var login = new LoginWindow { DataContext = new LoginWindowViewModel(App.AuthService) };
      
        var success = await login.ShowDialog<bool>(owner);
        if (success && App.AuthService.CurrentUser != null)
        {
            CurrentUser = App.AuthService.CurrentUser.Name;
            StatusMessage = $"Нэвтэрсэн: {CurrentUser}";
            IsAdmin = App.AuthService.CurrentUser.IsAdmin; // Админ төлөвийг шинэчлэх
        }
        else
        {
            owner.Close(); // Хэрэв дахин login амжилтгүй → програм хаах
        }
    }
}