using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantPOS.Models;
using RestaurantPOS.Data;
using RestaurantPOS.Utils;
using Avalonia.Media.Imaging;

namespace RestaurantPOS.ViewModels;

public partial class AdminTablesViewModel : ViewModelBase
{
    private readonly AppDbContext _context = new();

    [ObservableProperty] private ObservableCollection<TableModel> tables = new();
    [ObservableProperty] private TableModel? selectedTable;

    [ObservableProperty] private ObservableCollection<HallModel> halls = new();
    [ObservableProperty] private HallModel? selectedHall;

    [ObservableProperty] private string tableName = string.Empty;
    [ObservableProperty] private double positionX = 0;
    [ObservableProperty] private double positionY = 0;

    [ObservableProperty] private string? tableImagePath; // Ширээний зураг

    [ObservableProperty] private string? editTableImagePath;   // Түр сонгосон зураг (preview)

    public AdminTablesViewModel()
    {
        LoadTables();
        LoadHalls();
    }

    private void LoadTables()
    {
        Tables = new ObservableCollection<TableModel>(_context.Tables.ToList());
    }

    private void LoadHalls()
    {
        Halls = new ObservableCollection<HallModel>(_context.Halls.ToList());
    }

    [RelayCommand]
    private async Task BrowseImage()
    {
        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;
        if (mainWindow == null) return;

        var file = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Ширээний зураг сонгох",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Зураг") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg" } }
            }
        });

        if (file != null && file.Any())
        {
            var savedPath = await ImageStorage.SaveImageAsync(file.First(), "Tables");
            if (savedPath != null)
            {
                EditTableImagePath = savedPath;
            }
        }
    }

    [RelayCommand]
    private void AddOrUpdateTable()
    {
        if (SelectedHall == null || string.IsNullOrWhiteSpace(TableName))
            return;

        if (SelectedTable == null) // Шинэ
        {
            TableImagePath = EditTableImagePath ?? TableImagePath;
            var table = new TableModel
            {
                Name = TableName,
                PositionX = PositionX,
                PositionY = PositionY,
                HallId = SelectedHall.Id,
                ImagePath = TableImagePath!
            };
            _context.Tables.Add(table);
            _context.SaveChanges();
            Tables.Add(table);
        }
        else // Засах
        {
            TableImagePath = EditTableImagePath ?? TableImagePath;
            SelectedTable.Name = TableName;
            SelectedTable.PositionX = PositionX;
            SelectedTable.PositionY = PositionY;
            SelectedTable.HallId = SelectedHall.Id;
            SelectedTable.ImagePath = TableImagePath!;

            _context.Tables.Update(SelectedTable);
            _context.SaveChanges();
        }

        ClearForm();
    }

    [RelayCommand]
    private void DeleteTable()
    {
        if (SelectedTable != null)
        {
            _context.Tables.Remove(SelectedTable);
            _context.SaveChanges();
            Tables.Remove(SelectedTable);
            ClearForm();
        }
    }

    private void ClearForm()
    {
        SelectedTable = null;
        TableName = string.Empty;
        PositionX = 0;
        PositionY = 0;
        TableImagePath = null;
        SelectedHall = null;
    }


    partial void OnSelectedTableChanged(TableModel? value)
    {
        if (value != null)
        {
            TableName = value.Name!;
            PositionX = value.PositionX;
            PositionY = value.PositionY;
            TableImagePath = value.ImagePath;
            SelectedHall = Halls.FirstOrDefault(h => h.Id == value.HallId);
        }
    }
    
    public Bitmap SafePreviewImage =>
    !string.IsNullOrEmpty(EditTableImagePath) && File.Exists(EditTableImagePath)
        ? new Bitmap(EditTableImagePath)
        : !string.IsNullOrEmpty(TableImagePath) && File.Exists(TableImagePath)
            ? new Bitmap(TableImagePath)
            : new Bitmap(Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Table.png"));

    partial void OnTableImagePathChanged(string? value) => OnPropertyChanged(nameof(SafePreviewImage));
    partial void OnEditTableImagePathChanged(string? value) => OnPropertyChanged(nameof(SafePreviewImage));
}
