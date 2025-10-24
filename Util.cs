using System.Text;

namespace MshExplorer;

public class Util
{
    const string reset = "\x1b[0m";
    const string bold = "\x1b[1m";

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

    public static string GetString()
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        ExplorerDraw.CommandLine();
        StringBuilder inputBuilder = new();
        ConsoleKeyInfo key;
        int maxLength = 40;
        string item = string.Empty;
        while (true)
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace && inputBuilder.Length > 0)
            {
                inputBuilder.Remove(inputBuilder.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (inputBuilder.Length < maxLength && !char.IsControl(key.KeyChar))
            {
                inputBuilder.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
        }
        if (key.Key == ConsoleKey.Enter)
        {
            item = inputBuilder.ToString();
        }

        ExplorerDraw.RemoveCommandLine(cursorPos);
        return item;
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

    public static void RemoveItem(ExplorerItem item, ref string errMessage)
    {
        bool dirWithContent = false;
        (int, int) cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(0, 0);

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
            string format = ExplorerDraw.WriteDisplayText(item, false, ref errMessage);
            Console.Write($"   Remove {format} And it's Content?"); Console.SetCursorPosition(0, 1);
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
                Console.Write("\x1b[2K"); Console.SetCursorPosition(0, 0);
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


        Console.Write("\x1b[2K"); Console.SetCursorPosition(0, 0);
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
        errMessage = $"-- Error {DateTime.Now.ToString("G")} --\n{errMessage}\n-- End --\n\n";
        File.AppendAllText(logPath, errMessage);
    }

}
