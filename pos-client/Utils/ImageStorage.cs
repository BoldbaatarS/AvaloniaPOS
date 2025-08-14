using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Threading.Tasks;


namespace RestaurantPOS.Utils;
[Obsolete("Энэ методыг ашиглах боломжгүй. NewMethod ашиглана уу.", true)]
public static class ImageStorage
{
    private static readonly string RootFolder = Path.Combine(AppContext.BaseDirectory, "Images");

    public static async Task<string?> SaveImageAsync(IStorageFile file, string subfolder)
    {
        try
        {
            var folder = Path.Combine(RootFolder, subfolder);
            Directory.CreateDirectory(folder);

            var newFileName = Guid.NewGuid() + Path.GetExtension(file.Name);
            var destPath = Path.Combine(folder, newFileName);

            await using var sourceStream = await file.OpenReadAsync();
            await using var destinationStream = File.Create(destPath);
            await sourceStream.CopyToAsync(destinationStream);

            return destPath;
        }
        catch
        {
            return null;
        }
    }

    public static string GetSafeImagePath(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            // Default зураг
            return Path.Combine(AppContext.BaseDirectory, "Assets", "Images", "placeholder.png");
        }
        return path;
    }
}
