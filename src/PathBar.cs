using System.Runtime.InteropServices;

namespace MshExplorer;

class PathBar
{
    const string deleteLine = "\e[2K";
    const string reset = "\e[0m";
    const string bold = "\e[1m";
    const string green = "\e[38;2;129;178;154m";
    const string orange = "\e[38;2;224;122;95m";

    public bool WriteAccess;

    public void Draw(string path)
    {
        List<string> dirs = new();
        char separator = '/';
        WriteAccess = CheckWriteAccess(path);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            separator = '\\';

        string[] splits = path.Split(separator);

        if (splits.Length > 3)
        {
            dirs.Add($"{green}{bold}…{reset}");
            dirs.AddRange(splits.TakeLast(3));
        }
        else
        {
            dirs.AddRange(splits.TakeLast(3));
        }
        
        string header = string.Join($" {bold}{orange}\x1b[0m {green}{bold}", dirs);
        if (!WriteAccess)
            header = $"{header}{reset} \uD83D\uDD12";
        else
            header = $"{header}{reset}";

        
        (int, int) cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(0, 5);
        Console.Write(deleteLine);
        Console.Write("\e[0G"); // Move cursor to column 0
        Console.Write(header);

        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    private bool CheckWriteAccess(string path)
    {
        try
        {
            string tempFile = Path.Combine(path, Guid.NewGuid().ToString() + ".tmp");

            using (FileStream fs = File.Create(tempFile, 1, FileOptions.DeleteOnClose))
            {
            }
            return true;
        }
        catch (UnauthorizedAccessException){ return false;}
        catch (Exception) {return false;}
    }

}
