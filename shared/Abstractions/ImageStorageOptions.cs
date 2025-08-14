namespace Shared.Abstractions;

public sealed class ImageStorageOptions
{
    public string ImagesRoot { get; set; } = Path.Combine(AppContext.BaseDirectory, "Images");
    public bool Overwrite { get; set; } = false;
}
