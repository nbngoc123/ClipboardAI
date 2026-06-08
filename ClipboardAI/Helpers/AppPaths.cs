using System.IO;

namespace ClipboardAI.Helpers;

public static class AppPaths
{
    public static string AppDataFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
        "ClipboardAI");

    public static string DatabasePath => Path.Combine(AppDataFolder, "clipboard.db");

    public static string ImagesFolder => Path.Combine(AppDataFolder, "Images");

    public static void EnsureDirectoriesCreated()
    {
        if (!Directory.Exists(AppDataFolder))
        {
            Directory.CreateDirectory(AppDataFolder);
        }
        if (!Directory.Exists(ImagesFolder))
        {
            Directory.CreateDirectory(ImagesFolder);
        }
    }
}
