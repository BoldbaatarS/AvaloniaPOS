using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using RestaurantPOS.Models;
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
                // ViewModel-д өөрчлөлт орсон үед DrawTables()
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


            panel.Children.Add(new Image
            {
                Source = table.SafeTableImagePath,
                Width = 80,
                Height = 80
            });

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
        }
    }

    private void OnTablePointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && _draggingTable != null && sender is Border border)
        {
            var current = e.GetPosition(HallCanvas);
            var dx = current.X - _lastPoint.X;
            var dy = current.Y - _lastPoint.Y;

            _draggingTable.PositionX += dx;
            _draggingTable.PositionY += dy;

            Canvas.SetLeft(border, _draggingTable.PositionX);
            Canvas.SetTop(border, _draggingTable.PositionY);

            _lastPoint = current;
        }
    }

    private void OnTablePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggingTable != null)
        {
            // Байрлалыг DB-д хадгалах
            using (var db = new Data.AppDbContext())
            {
                var dbTable = db.Tables.FirstOrDefault(t => t.Id == _draggingTable.Id);
                if (dbTable != null)
                {
                    dbTable.PositionX = _draggingTable.PositionX;
                    dbTable.PositionY = _draggingTable.PositionY;
                    db.SaveChanges();
                }
            }
        }

        e.Pointer.Capture(null);
        _isDragging = false;
        _draggingTable = null;
    }
}
