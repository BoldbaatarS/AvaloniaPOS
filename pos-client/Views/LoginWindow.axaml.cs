using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RestaurantPOS.Services;

namespace RestaurantPOS.Views;

public partial class LoginWindow : Window
{
    private readonly IAuthService _authService;

    public LoginWindow()
    {
        InitializeComponent();
        _authService = App.AuthService; // App доторх AuthService-г ашиглаж байна
    }

    // Товчлуур дарахад TextBox дээр тэмдэгт нэмэх
    private void PinButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && PinDisplay != null)
        {
            PinDisplay.Text += btn.Content?.ToString();
        }
    }

    // Сүүлийн тэмдэгт устгах
    private void Backspace_Click(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(PinDisplay.Text))
            PinDisplay.Text = PinDisplay.Text.Substring(0, PinDisplay.Text.Length - 1);
    }

    // Цэвэрлэх
    private void Clear_Click(object? sender, RoutedEventArgs e)
    {
        PinDisplay.Text = string.Empty;
    }

    // Нэвтрэх
    // private async Task Login_Click(object? sender, RoutedEventArgs e)
    // {
    //     var pin = PinDisplay.Text ?? string.Empty;

    //     // AuthService ашиглаж шалгах
    //     if (_authService.LoginByPin(pin))
    //     {
    //         Close(true); // Dialog амжилттай хаах
    //     }
    //     else
    //     {
    //        await MessageBoxService.ShowAsync(this, "Алдаа", "Буруу PINfghf!", MessageDialog.DialogType.Error);
    //         PinDisplay.Text = string.Empty;
    //     }
    // }

    // Цонх чирч зөөх
    private void Window_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
}
