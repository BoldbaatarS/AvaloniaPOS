using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantPOS.Services;
using RestaurantPOS.Views;
using RestaurantPOS.Views.Dialogs;


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
        _navigation.NavigateTo(new HomeViewModel());
    }

    [RelayCommand]
    public void Home() => _navigation.NavigateTo(new HomeViewModel());

    [RelayCommand]
    public async Task Admin(Window owner)
    {
        if (App.AuthService.CurrentUser?.IsAdmin == true)
        {
            // Админ бол нэвтрэх
            _navigation.NavigateTo(new AdminPanelViewModel());
        }
        else
        {
            await MessageBoxService.ShowAsync(owner, "Хандалт хориглогдсон", "Таны эрх хүрэхгүй байна.", MessageDialog.DialogType.Warning);
        }
        // Хэрэглэгч байхгүй эсвэл админ биш бол алдаа харуул
        // if (string.IsNullOrEmpty(CurrentUser) || !IsAdmin)
        // {
        //     Console.WriteLine($"Admin access denied. Current user: {CurrentUser ?? "null"}, IsAdmin: {IsAdmin}");


        //     await MessageBoxService.ShowAsync(owner, "Алдаа", "Зөвхөн админ нэвтэрсэн хэрэглэгчид админ хэсэгт нэвтрэх боломжтой.", MessageDialog.DialogType.Error);
        //     return;
        // }

        
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