using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace RestaurantPOS.Views.Dialogs;

public partial class MessageDialog : Window
{
    public enum DialogType
    {
        Info,
        Warning,
        Error,
        Confirm
    }

    public MessageDialog()
    {
        InitializeComponent();
    }

    public MessageDialog(string title, string message, DialogType type = DialogType.Info) : this()
    {
        TitleBlock.Text = title;
        MessageBlock.Text = message;

        // Icon path
        var iconPath = type switch
        {
            DialogType.Info => "avares://RestaurantPOS/Assets/info.png",
            DialogType.Warning => "avares://RestaurantPOS/Assets/warning.png",
            DialogType.Error => "avares://RestaurantPOS/Assets/error.png",
            DialogType.Confirm => "avares://RestaurantPOS/Assets/question.png",
            _ => "avares://RestaurantPOS/Assets/info.png"
        };

        using var stream = AssetLoader.Open(new Uri(iconPath));
        IconImage.Source = new Bitmap(stream);

        // Background өнгө төрөл бүрээр
        var iconBackground = type switch
        {
            DialogType.Info => new SolidColorBrush(Color.Parse("#E3F2FD")),     // Цэнхэр
            DialogType.Warning => new SolidColorBrush(Color.Parse("#FFF8E1")),  // Шар
            DialogType.Error => new SolidColorBrush(Color.Parse("#FFEBEE")),    // Улаан
            DialogType.Confirm => new SolidColorBrush(Color.Parse("#E8F5E9")),  // Ногоон
            _ => new SolidColorBrush(Color.Parse("#E3F2FD"))
        };
        this.Resources["IconBackground"] = iconBackground;

        // Cancel товч зөвхөн Confirm үед харагдана
        CancelButton.IsVisible = type == DialogType.Confirm;
    }

    private void Ok_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(true);
    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(false);
}
