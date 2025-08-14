using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.CompilerServices;

namespace Shared.Models
{
    /// <summary>
    /// UI-ээс хараат бус ширээний модель (POCO).
    /// Зургийг UI талд (converter/viewmodel) Bitmap болгоно.
    /// </summary>
    public class TableModel : INotifyPropertyChanged
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        private string? _name = string.Empty;
        public string? Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private double _positionX;
        public double PositionX
        {
            get => _positionX;
            set { if (Math.Abs(_positionX - value) > double.Epsilon) { _positionX = value; OnPropertyChanged(); } }
        }

        private double _positionY;
        public double PositionY
        {
            get => _positionY;
            set { if (Math.Abs(_positionY - value) > double.Epsilon) { _positionY = value; OnPropertyChanged(); } }
        }

        // Зургийн эх зам (оригинал)
        private string? _imagePath = string.Empty;
        public string? ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SafeImagePath));
                }
            }
        }

        /// <summary>
        /// UI-д ашиглах найдвартай зам (fallback default зурагтай).
        /// </summary>
        [NotMapped]
        public string SafeImagePath =>
            !string.IsNullOrWhiteSpace(ImagePath) && File.Exists(ImagePath)
                ? ImagePath!
                : Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Table.png");

        // Статус (шийдвэл enum болгож болно)
        private string _status = "Available"; // Available / Reserved / Occupied
        public string Status
        {
            get => _status;
            set { if (_status != value) { _status = value; OnPropertyChanged(); } }
        }

        // Харьяалах заал
        public Guid HallId { get; set; }
        public HallModel? Hall { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
