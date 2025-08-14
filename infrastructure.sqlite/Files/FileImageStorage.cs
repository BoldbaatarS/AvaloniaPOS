using System.Net.Http;
using Microsoft.Extensions.Options;
using Shared.Abstractions;

namespace Infrastructure.Files;

public sealed class FileImageStorage : IImageStorage
{
    private readonly ImageStorageOptions _opt;
    private readonly HttpClient _http;

    public FileImageStorage(IOptions<ImageStorageOptions> opt, HttpClient httpClient)
    {
        _opt = opt.Value;
        _http = httpClient;
        Directory.CreateDirectory(_opt.ImagesRoot);
    }

    public async Task<string> SaveFromStreamAsync(Stream source, string subfolder, string? fileName = null, CancellationToken ct = default)
    {
        var folder = EnsureFolder(subfolder);
        var ext = GuessExtension(fileName) ?? ".bin";
        var name = fileName ?? $"{Guid.NewGuid()}{ext}";
        var dest = Path.Combine(folder, name);

        var tmp = dest + ".tmp";
        Directory.CreateDirectory(folder);

        await using (var fs = File.Create(tmp))
            await source.CopyToAsync(fs, ct);

        if (File.Exists(dest) && !_opt.Overwrite)
            dest = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(name)}_{Guid.NewGuid()}{ext}");

        File.Move(tmp, dest, overwrite: true);
        return dest;
    }

    public Task<string> SaveFromPathAsync(string sourcePath, string subfolder, string? fileName = null, CancellationToken ct = default)
    {
        if (!File.Exists(sourcePath)) throw new FileNotFoundException(sourcePath);
        var ext = Path.GetExtension(sourcePath);
        var name = fileName ?? $"{Guid.NewGuid()}{ext}";
        var folder = EnsureFolder(subfolder);
        var dest = Path.Combine(folder, name);
        Directory.CreateDirectory(folder);
        File.Copy(sourcePath, dest, overwrite: _opt.Overwrite);
        return Task.FromResult(dest);
    }

    public async Task<string> SaveFromUrlAsync(Uri url, string subfolder, string? fileName = null, CancellationToken ct = default)
    {
        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        var ext = Path.GetExtension(url.AbsolutePath);
        return await SaveFromStreamAsync(stream, subfolder, fileName ?? (string.IsNullOrWhiteSpace(ext) ? null : $"dl_{Guid.NewGuid()}{ext}"), ct);
    }

    public Task<bool> DeleteAsync(string absolutePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(absolutePath)) return Task.FromResult(false);
        try { if (File.Exists(absolutePath)) File.Delete(absolutePath); return Task.FromResult(true); }
        catch { return Task.FromResult(false); }
    }

    public string GetSafeImagePath(string? absoluteOrNull, string fallbackRelative = "Assets/Images/placeholder.png")
    {
        if (!string.IsNullOrWhiteSpace(absoluteOrNull) && File.Exists(absoluteOrNull))
            return absoluteOrNull!;
        return Path.IsPathRooted(fallbackRelative)
            ? fallbackRelative
            : Path.Combine(AppContext.BaseDirectory, fallbackRelative);
    }

    private string EnsureFolder(string subfolder)
    {
        var folder = Path.Combine(_opt.ImagesRoot, subfolder ?? string.Empty);
        Directory.CreateDirectory(folder);
        return folder;
    }

    private static string? GuessExtension(string? fileName)
        => string.IsNullOrWhiteSpace(fileName) ? null : Path.GetExtension(fileName);
}
