using System;
using System.IO;

namespace Shared.Utils;

public static class FileHelper
{
    public static string EnsureRootFolder(string subFolder)
    {
        var rootPath = AppContext.BaseDirectory; // Program ажиллаж байгаа хавтас
        var folderPath = Path.Combine(rootPath, subFolder);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }
}
