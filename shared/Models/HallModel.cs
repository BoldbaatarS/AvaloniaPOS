using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;

namespace Shared.Models
{
    /// <summary>
    /// UI-ээс хараат бус (POCO) Hall модель.
    /// Зураг харуулахыг UI давхарга дээр (converter/viewmodel) шийднэ.
    /// </summary>
    public class HallModel : INotifyPropertyChanged
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private string? _imagePath;
        /// <summary>
        /// Хэрэв хоосон биш бол оригинал зурагны абсолют/харьцангуй зам.
        /// </summary>
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
        /// UI-д үзүүлэхэд ашиглах найдвартай зам (оруулах зураг байхгүй бол default fallback).
        /// Анхаар: UI дээрээс энэ string-ийг Bitmap болгоно (converter/viewmodel).
        /// </summary>
        public string SafeImagePath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ImagePath) && File.Exists(ImagePath))
                    return ImagePath!;

                // fallback: portable assets (exe-ний хажуугийн Assets/Default/Hall.png)
                return Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Hall.png");
            }
        }

        /// <summary>
        /// Холбогдох ширээнүүд. (Shared.TableModel нь мөн POCO байх ёстой)
        /// </summary>
        public List<TableModel> Tables { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
