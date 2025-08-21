using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using Infrastructure.Sqlite;
using Shared.Utils;
using Shared.Models;
using Avalonia.Media.Imaging;


namespace RestaurantPOS.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly AppDbContext _context;

    [ObservableProperty] private ObservableCollection<HallModel> halls = new();
    [ObservableProperty] private HallModel? selectedHall;
    [ObservableProperty] private ObservableCollection<TableModel> tables = new();

    private AppConfig _config = null!;



    public HomeViewModel(AppDbContext context)
    {
        _context = context;
        LoadHalls();
    }

    private void LoadHalls()
    {
        Halls = new ObservableCollection<HallModel>(_context.Halls.ToList());
        _config = ConfigManager.Load();

        if (_config.DefaultHallId.HasValue)
        {
            SelectedHall = Halls.FirstOrDefault(h => h.Id == _config.DefaultHallId.Value);
        }

        Halls.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(IsHallSelectionVisible));
        };
    }
    public bool IsHallSelectionVisible => Halls.Count > 1;

    partial void OnSelectedHallChanged(HallModel? value)
    {
        if (value != null)
        {
            Tables = new ObservableCollection<TableModel>(
                _context.Tables.Where(t => t.HallId == value.Id).ToList()
            );
            // DefaultHall болгон хадгалах
            _config.DefaultHallId = value.Id;
            ConfigManager.Save(_config);
        }
        else
        {
            Tables.Clear();
        }
    }



    public string BackgroundImage =>
        SelectedHall != null && !string.IsNullOrEmpty(SelectedHall.ImagePath) && File.Exists(SelectedHall.ImagePath)
            ? SelectedHall.ImagePath
            : Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Hall.png");
            
     public double HallWidth
    {
        get
        {
            var path = SelectedHall?.SafeImagePath;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                using var bmp = new Bitmap(path);
                return bmp.PixelSize.Width;
            }
            return 800;
        }
    }
    public double HallHeight
{
    get
    {
        var path = SelectedHall?.SafeImagePath;
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            using var bmp = new Bitmap(path);
            return bmp.PixelSize.Height;
        }
        return 600;
    }
}
}