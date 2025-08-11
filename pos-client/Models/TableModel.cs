using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RestaurantPOS.Models
{
    public partial class TableModel : ObservableObject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private string? name = string.Empty;

        // Байршил
        [ObservableProperty]
        private double positionX;

        [ObservableProperty]
        private double positionY;

        // Ширээний зураг
        [ObservableProperty]
        private string? imagePath = string.Empty;
        public string Status { get; set; } = "Available"; // Available / Reserved / Occupied

        // Хамаарах заал
        public Guid HallId { get; set; }
        public HallModel? Hall { get; set; }

        // Default зураг
        [NotMapped]
        public Bitmap SafeTableImagePath =>
            !string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath)
            ? new Bitmap(ImagePath)
            : new Bitmap(Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Table.png"));

    }
}

