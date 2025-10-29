
namespace MshExplorer;

public enum ExplorerType
{
    DIRECTORY,
    FILE,
    NONE
}


public class ExplorerItem
{
    public string DisplayName { get; set; }
    public string Path { get; set; }
    public ExplorerType Type { get; set; }

    public ExplorerItem(string displayName, string path, ExplorerType type)
    {
        DisplayName = displayName;
        Path = path;
        Type = type;
    }

    public static List<ExplorerItem> GetDirItems(string currentPath, ref string errMessage)
    {
        List<ExplorerItem> items = [];
        try
        {
            var cwd = new DirectoryInfo(currentPath);
            var opts = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false,
                AttributesToSkip = 0
            };

            var dirs = cwd.EnumerateDirectories("*", opts)
                        .OrderBy(d => d.Name, StringComparer.CurrentCultureIgnoreCase);

            foreach (var dir in dirs)
            {
                items.Add(new ExplorerItem(dir.Name, dir.FullName, ExplorerType.DIRECTORY));
            }
            var files = cwd.EnumerateFiles("*", opts)
                        .OrderBy(f => f.Name, StringComparer.CurrentCultureIgnoreCase);

            foreach (var f in files)   
            {
                    items.Add(new ExplorerItem(f.Name, f.FullName, ExplorerType.FILE));
            }
        }
        catch
        {
            items.Clear();
            errMessage = "Error: when fetching directories In function: GetDirItems()";
        }

        return items;
    }
    public static bool IsBinaryFile(string filePath, int sampleSize, ref string errMessage)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[sampleSize];
                int bytesRead = fs.Read(buffer, 0, sampleSize);

                // Check if any null byte is present in the sampled portion
                return buffer.Take(bytesRead).Any(b => b == 0);
            }
        }
        catch (IOException ex)
        {
            errMessage = $"Error: {ex}";
            return false;
        }
    }

}
