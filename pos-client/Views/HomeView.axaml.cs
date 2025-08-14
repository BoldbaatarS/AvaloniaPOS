using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Microsoft.EntityFrameworkCore;           // <-- FirstOrDefaultAsync / SaveChangesAsync
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Sqlite;
using Shared.Models;
using RestaurantPOS.ViewModels;

namespace RestaurantPOS.Views;

public partial class HomeView : UserControl
{
    private bool _isDragging = false;
    private Point _lastPoint;
    private TableModel? _draggingTable;

    private HomeViewModel? ViewModel => DataContext as HomeViewModel;

    public HomeView()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;

                if (ViewModel.Tables != null)
                {
                    ViewModel.Tables.CollectionChanged += (_, _) => DrawTables();
                    DrawTables();
                }
            }
        };
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Tables))
        {
            if (ViewModel?.Tables != null)
            {
                ViewModel.Tables.CollectionChanged += (_, _) => DrawTables();
                DrawTables();
            }
        }
    }

    private void DrawTables()
    {
        HallCanvas.Children.Clear();

        foreach (var table in ViewModel?.Tables ?? Enumerable.Empty<TableModel>())
        {
            var border = new Border
            {
                Width = 100,
                Height = 100,
                Background = Brushes.Transparent,
                Tag = table
            };

            border.PointerPressed += OnTablePointerPressed;
            border.PointerMoved += OnTablePointerMoved;
            border.PointerReleased += OnTablePointerReleased;

            var panel = new StackPanel { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };

            var path = table.SafeImagePath; // string path
            var image = new Image
            {
                Source = File.Exists(path)
                    ? new Bitmap(path)
                    : new Bitmap(Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Table.png")),
                Width = 80,
                Height = 80
            };
            panel.Children.Add(image);

            panel.Children.Add(new TextBlock
            {
                Text = table.Name,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            });

            border.Child = panel;

            Canvas.SetLeft(border, table.PositionX);
            Canvas.SetTop(border, table.PositionY);

            HallCanvas.Children.Add(border);
        }
    }

    private void OnTablePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border element && element.Tag is TableModel table)
        {
            _draggingTable = table;
            _lastPoint = e.GetPosition(HallCanvas);
            _isDragging = true;

            e.Pointer.Capture(element);

            // Bring to front (optional)
            element.SetValue(Panel.ZIndexProperty, 999);
        }
    }

    private void OnTablePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || _draggingTable is null || sender is not Border border)
            return;

        var current = e.GetPosition(HallCanvas);
        var dx = current.X - _lastPoint.X;
        var dy = current.Y - _lastPoint.Y;

        // Proposed new position
        var newX = _draggingTable.PositionX + dx;
        var newY = _draggingTable.PositionY + dy;

        // Canvas & item sizes
        var canvasW = GetActualWidth(HallCanvas);
        var canvasH = GetActualHeight(HallCanvas);
        var itemW = GetActualWidth(border);
        var itemH = GetActualHeight(border);

        // Clamp inside bounds: [0 .. canvas - item]
        newX = Clamp(newX, 0, Math.Max(0, canvasW - itemW));
        newY = Clamp(newY, 0, Math.Max(0, canvasH - itemH));

        // Apply to model + UI
        _draggingTable.PositionX = newX;
        _draggingTable.PositionY = newY;

        Canvas.SetLeft(border, newX);
        Canvas.SetTop(border, newY);

        _lastPoint = current;
    }

    private async void OnTablePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggingTable != null)
        {
            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbTable = await db.Tables.FirstOrDefaultAsync(t => t.Id == _draggingTable.Id);
            if (dbTable != null)
            {
                dbTable.PositionX = _draggingTable.PositionX;
                dbTable.PositionY = _draggingTable.PositionY;
                await db.SaveChangesAsync();
            }
        }

        e.Pointer.Capture(null);
        _isDragging = false;
        _draggingTable = null;
    }

    // ---- helpers -------------------------------------------------------------

    private static double Clamp(double val, double min, double max)
        => val < min ? min : (val > max ? max : val);

    private static double GetActualWidth(Control c)
        => double.IsFinite(c.Bounds.Width) && c.Bounds.Width > 0 ? c.Bounds.Width :
           double.IsFinite(c.Width) && c.Width > 0 ? c.Width : 0;

    private static double GetActualHeight(Control c)
        => double.IsFinite(c.Bounds.Height) && c.Bounds.Height > 0 ? c.Bounds.Height :
           double.IsFinite(c.Height) && c.Height > 0 ? c.Height : 0;
}
