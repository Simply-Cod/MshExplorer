
namespace MshExplorer;

public class Util
{

    public static bool IsReadableDir(string path, ref string errMessage)
    {
        try
        {
            using var e = Directory.EnumerateFileSystemEntries(path).GetEnumerator();

            _ = e.MoveNext();
            return true;
        }
        catch (UnauthorizedAccessException ex) {errMessage = $"Error: {ex.Message}"; return false; }
        catch (DirectoryNotFoundException ex) {errMessage = $"Error: {ex.Message}"; return false; }
        catch (IOException ex) {errMessage = $"Error: {ex.Message}"; return false; }
    }

    public static void AddItem(string currentPath, string name, ref string errMessage)
    {
        try
        {
            name = name.Trim();

            bool isDir = name.EndsWith("/") || name.EndsWith(Path.DirectorySeparatorChar);

            if (isDir) // Create Directory
            {
                string dirName = name.TrimEnd('/', Path.DirectorySeparatorChar);
                if (string.IsNullOrWhiteSpace(dirName))
                {
                    return;
                }
                string dirPath = Path.Combine(currentPath, dirName);

                if (Directory.Exists(dirPath))
                {
                    return;
                }
                Directory.CreateDirectory(dirPath);

            }
            else                            // Create File
            {
                string filePath = Path.Combine(currentPath, name);
                if (File.Exists(filePath))
                {
                    return;
                }
                FileStream fs = File.Create(filePath);
                fs.Close();
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            errMessage = $"Error: {ex.Message}";
            return;
        }
        catch (IOException ex)
        {
            errMessage = $"Error: {ex.Message}";
            return;
        }
        catch (ArgumentException ex)
        {
            errMessage = $"Error: {ex.Message}";
            return;
        }
    }

    public static void RemoveItem(ExplorerItem item, ref string errMessage, bool nerdFont)
    {
        bool dirWithContent = false;
        (int, int) cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(0, 1);

        if (item.Type == ExplorerType.DIRECTORY)
        {
            if (Directory.GetFiles(item.Path).Length > 0)
            {
                dirWithContent = true;
            }
            if (Directory.GetDirectories(item.Path).Length > 0)
            {
                dirWithContent = true;
            }
        }

        if (dirWithContent)
        {
            string format = Ansi.GetFormattedText(item, nerdFont);
            Console.Write($"   Remove {format} And it's Content?"); Console.SetCursorPosition(0, 2);
            Console.Write("    (y)Yes (n)No");
            char key = 'a';

            while (true)
            {
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y') break;
                else if (key == 'n') break;
            }

            if (key == 'n')
            {
                Console.Write("\x1b[2K"); Console.SetCursorPosition(0, 1);
                Console.Write("\x1b[2K");
                Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);

                return;
            }
        }

        try
        {
            if (item.Type == ExplorerType.DIRECTORY)
            {
                if (Directory.Exists(item.Path))
                {
                    Directory.Delete(item.Path, true);
                }
            }
            else if (item.Type == ExplorerType.FILE)
            {
                if (File.Exists(item.Path))
                {
                    File.Delete(item.Path);
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            errMessage = $"Error: {ex.Message}";
            return;
        }
        catch (IOException ex)
        {
            errMessage = $"Error: {ex.Message}";
            return;
        }
        catch (ArgumentException ex)
        {
            errMessage = $"Error: {ex.Message}";
            return;
        }


        Console.Write("\x1b[2K"); Console.SetCursorPosition(0, 1);
        Console.Write("\x1b[2K");
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public static void PasteFile(ExplorerItem clipboardItem, string currentPath, ref string errMessage)
    {
        string name = Path.GetFileName(clipboardItem.Path);
        string destination = Path.Combine(currentPath, name);

        try
        {
            if (File.Exists(clipboardItem.Path))
            {
                if (!File.Exists(destination))
                {
                    File.Copy(clipboardItem.Path, destination);
                }
            }
        }
        catch (UnauthorizedAccessException ex) 
        { 
            errMessage = $"Error: {ex.Message}";
            return; 
        }
        catch (IOException ex) 
        { 
            errMessage = $"Error: {ex.Message}";
            return; 
        }
        catch (ArgumentException ex) 
        { 
            errMessage = $"Error: {ex.Message}";
            return; 
        }
    }

    public static void PasteDirectory(string clipboardItemPath, string currentPath, ref string errMessage)
    {
        DirectoryInfo dir = new(clipboardItemPath);

        if (!dir.Exists)
        {
            return;
        }
        try
        {

            DirectoryInfo[] subDirs = dir.GetDirectories();

            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }
            else
            {
                currentPath = Path.Combine(currentPath, dir.Name);
                Directory.CreateDirectory(currentPath);
            }

            FileInfo[] subFiles = dir.GetFiles();

            foreach (var f in subFiles)
            {
                string tempPath = Path.Combine(currentPath, f.Name);
                f.CopyTo(tempPath, true); // The bool tells the function to overwrite if file already exists
            }

            foreach (var d in subDirs)
            {
                string tempPath = Path.Combine(currentPath, d.Name);
                PasteDirectory(d.FullName, tempPath, ref errMessage);
            }
        }
        catch (UnauthorizedAccessException ex) 
        { 
            errMessage = $"Error: {ex.Message}";
            return; 
        }
        catch (IOException ex) 
        { 
            errMessage = $"Error: {ex.Message}";
            return; 
        }
        catch (ArgumentException ex) 
        { 
            errMessage = $"Error: {ex.Message}";
            return; 
        }
    }

    public static void LogErrorMessage(string errMessage)
    {
        string logPath = "debug/error.txt";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            errMessage = $"-- Error {DateTime.Now.ToString("G")} --\n{errMessage}\n-- End --\n\n";
            File.AppendAllText(logPath, errMessage);

        }
        catch {}
        }

    public static void Clear()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write("\e[J");
    }



    public static bool CheckFileAccess(string filePath)
    {
        return ProbeFileReadable(filePath);
    }

    public static bool CheckWriteAccess(string path)
    {
        try
        {
            string tempFile = Path.Combine(path, Guid.NewGuid().ToString() + ".tmp");

            using (FileStream fs = File.Create(tempFile, 1, FileOptions.DeleteOnClose))
            {
            }
            return true;
        }
        catch (UnauthorizedAccessException) { return false; }
        catch (Exception) { return false; }
    }
    
    public static bool ProbeFileReadable(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        if (!File.Exists(filePath))
            return false;

        try
        {
            using var fs = new FileStream(
                filePath,
                FileMode.Open,                 
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete
            );
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            return false;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }


}
