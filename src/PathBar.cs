using System.Runtime.InteropServices;

namespace MshExplorer;

class PathBar
{
    public bool WriteAccess;
    public bool NerdFont;
    public PathStyler Style = new();

    public void Draw(string path)
    {
        List<string> dirs = new();
        char separator = '/';
        WriteAccess = Util.CheckWriteAccess(path);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            separator = '\\';

        string[] splits = path.Split(separator);

        if (splits.Length > 3)
        {
            dirs.Add($"{Style.TextStyle}…{Style.Reset}");
            dirs.AddRange(splits.TakeLast(3));
        }
        else
        {
            dirs.AddRange(splits.TakeLast(3));
        }
        
        string header = string.Join($" {Style.DividerStyle}›\x1b[0m {Style.TextStyle}", dirs);
        if (!WriteAccess)
            header = NerdFont ? $"{header}{Style.Reset} {Ansi.red}{Ansi.reset}" : $"{header}{Style.Reset} \uD83D\uDD12";
        else
            header = $"{header}{Style.Reset}";

        
        Console.SetCursorPosition(0, 5);
        Console.Write(Ansi.deleteLine);
        Console.Write("\e[0G"); // Move cursor to column 0
        Console.Write(header);

    }
    public static void DrawRecursiveSearchBar(string currentPath)
    {
       if (string.IsNullOrWhiteSpace(currentPath))
           return;

       string name = string.Empty;
       try
       {
            name = Path.GetFileName(currentPath)!;
       }
       catch {}
        Console.SetCursorPosition(0, 5);
        Console.Write(Ansi.deleteLine);
        Console.Write("\e[0G"); // Move cursor to column 0
        string header = $"{Ansi.dirColor}{Ansi.bold} {name}   --- Recursive Search ---{Ansi.reset}";
        Console.Write(header);
    }

    public void UpdateConfigs(UserConfigs configs)
    {
        NerdFont = configs.NerdFont;
        Style.Active = configs.PathStyle;

        if (Style.Active)
            Style.Activate();
        else
            Style.Deactivate();
    }

}
