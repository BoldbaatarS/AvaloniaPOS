namespace Shared.Abstractions;

public interface IImageStorage
{
    Task<string> SaveFromStreamAsync(Stream source, string subfolder, string? fileName = null, CancellationToken ct = default);
    Task<string> SaveFromPathAsync(string sourcePath, string subfolder, string? fileName = null, CancellationToken ct = default);
    Task<string> SaveFromUrlAsync(Uri url, string subfolder, string? fileName = null, CancellationToken ct = default);

    Task<bool> DeleteAsync(string absolutePath, CancellationToken ct = default);
    string GetSafeImagePath(string? absoluteOrNull, string fallbackRelative = "Assets/Images/placeholder.png");
}
