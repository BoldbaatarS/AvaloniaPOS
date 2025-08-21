using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models;
using Shared.Abstractions;
using Infrastructure.Sqlite;



namespace RestaurantPOS.ViewModels;

public partial class AdminHallsViewModel : ViewModelBase
{
    private readonly AppDbContext _context;

    [ObservableProperty] private ObservableCollection<HallModel> halls = new();
    [ObservableProperty] private HallModel? selectedHall;
    [ObservableProperty] private string hallName = string.Empty;
    [ObservableProperty] private string? hallImagePath;       // DB-д хадгалагдсан зам
    [ObservableProperty] private string? editHallImagePath;   // Түр сонгосон зураг (preview)

    private readonly IImageStorage _imageStorage;

    public AdminHallsViewModel(IImageStorage imageStorage, AppDbContext context)
    {
        _imageStorage = imageStorage;
        _context = context;
        LoadHalls();
    }

    private void LoadHalls()
    {
        Halls = new ObservableCollection<HallModel>(_context.Halls.ToList());
       
    }
    public bool IsHallSelectionVisible => Halls.Count > 1;

    [RelayCommand]
    private async Task BrowseImage()  // Preview сонгох
    {
        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;
        if (mainWindow == null) return;

        var files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Зураг сонгох",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("Зураг") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg" } } }
        });

        var file = files?.FirstOrDefault();
        if (file is null) return;

        await using var stream = await file.OpenReadAsync(); // Avalonia -> Stream
        // subfolder: "Halls", файл нэрийг эх нэрээр нь өгөх (сонголттой)
        var savedPath = await _imageStorage.SaveFromStreamAsync(stream, subfolder: "Halls", fileName: file.Name);
        EditHallImagePath = savedPath; // preview-д шууд хэрэглэнэ
    }

    [RelayCommand]
    private void AddOrUpdateHall()
    {
        if (string.IsNullOrWhiteSpace(HallName)) return;

        // Preview (EditHallImagePath) байгаа бол commit хийж HallImagePath болгоно
        if (!string.IsNullOrWhiteSpace(EditHallImagePath))
            HallImagePath = EditHallImagePath;

        if (SelectedHall == null) // Шинэ
        {
            var hall = new HallModel { Name = HallName, ImagePath = HallImagePath };
            _context.Halls.Add(hall);
            _context.SaveChanges();
            Halls.Add(hall);
        }
        else // Засвар
        {
            SelectedHall.Name = HallName;
            SelectedHall.ImagePath = HallImagePath;
            _context.Halls.Update(SelectedHall);
            _context.SaveChanges();
        }

        ClearForm();
    }

    [RelayCommand]
    private void DeleteHall()
    {
        if (SelectedHall != null)
        {
            _context.Halls.Remove(SelectedHall);
            _context.SaveChanges();
            Halls.Remove(SelectedHall);
            ClearForm();
        }
    }

    private void ClearForm()
    {
        SelectedHall = null;
        HallName = string.Empty;
        HallImagePath = null;
        EditHallImagePath = null;
    }

    partial void OnSelectedHallChanged(HallModel? value)
    {
        if (value != null)
        {
            HallName = value.Name;
            HallImagePath = value.ImagePath;
            EditHallImagePath = null; // Preview байхгүй бол
            OnPropertyChanged(nameof(SafePreviewImage)); // **Шинэ зураг үзүүлэхийн тулд preview-г сэргээе**
            
        }
        else
        {
            HallName = string.Empty;
            HallImagePath = null;
            EditHallImagePath = null;
        }
    }

    public Bitmap SafePreviewImage =>
    !string.IsNullOrEmpty(EditHallImagePath) && File.Exists(EditHallImagePath)
        ? new Bitmap(EditHallImagePath)
        : !string.IsNullOrEmpty(HallImagePath) && File.Exists(HallImagePath)
            ? new Bitmap(HallImagePath)
            : new Bitmap(Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Hall.png"));

    partial void OnHallImagePathChanged(string? value) => OnPropertyChanged(nameof(SafePreviewImage));
    partial void OnEditHallImagePathChanged(string? value) => OnPropertyChanged(nameof(SafePreviewImage));
}
