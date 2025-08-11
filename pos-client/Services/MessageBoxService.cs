using System.Threading.Tasks;
using Avalonia.Controls;
using RestaurantPOS.Views.Dialogs;

namespace RestaurantPOS.Services;

public static class MessageBoxService
{
    public static async Task<bool> ShowAsync(Window? owner, string title, string message, MessageDialog.DialogType type = MessageDialog.DialogType.Info)
    {
        var dialog = new MessageDialog(title, message, type);
        return await dialog.ShowDialog<bool>(owner!);
    }
}
