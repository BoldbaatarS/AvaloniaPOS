using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using RestaurantPOS.Models;

public class HallModel : INotifyPropertyChanged
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set { name = value; OnPropertyChanged(); }
    }

    private string? imagePath;
    public string? ImagePath
    {
        get => imagePath;
        set
        {
            imagePath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SafeHallImagePath)); // Зураг өөрчлөгдвөл энэ бас шинэчлэгдэнэ
        }
    }

    public List<TableModel> Tables { get; set; } = new();

    public Bitmap SafeHallImagePath =>
        !string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath)
            ? new Bitmap(ImagePath)
            : new Bitmap(Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Hall.png"));

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
