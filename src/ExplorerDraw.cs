using System.Runtime.InteropServices;

namespace MshExplorer;

// ╭─────╮
// │     │
// ╰─────╯

class ExplorerDraw
{
    const string reset = "\e[0m";
    const string deleteLine = "\e[2K";
    const string hideCursor = "\e[?25l";
    const string showCursor = "\e[?25h";

    const string bold = "\e[1m";
    const string orange = "\e[38;2;224;122;95m";
    const string green = "\e[38;2;129;178;154m";
    const string blue = "\e[38;2;41;81;242m";
    const string darkBlue = "\e[38;2;61;64;91m";
    const string mellow = "\e[38;2;154;151;132m";
    const string red = "\e[38;2;186;61;52m";

    const string bgDark = "\e[48;2;31;43;61m";
    const string bgMellow = "\e[48;2;154;151;132m";
    const string eraseLine = "\e[2K";


    public static void Border(int startX, int startY, int length, int height)
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(startX, startY);

        for (int i = 0; i <= height; i++)
        {
            for (int j = 0; j <= length; j++)
            {
                if (i == 0)
                {
                    if (j == 0)
                        Console.Write($"{darkBlue}╭");
                    else if (j == length)
                        Console.Write("╮");
                    else
                        Console.Write("─");
                }
                else if (i == height)
                {
                    if (j == 0)
                        Console.Write("╰");
                    else if (j == length)
                        Console.Write($"╯{reset}");
                    else
                        Console.Write("─");
                }
                else
                {
                    if (j == 0)
                        Console.Write("│");
                    else if (j == length)
                        Console.Write("│");
                    else
                        Console.Write(" ");
                }
            }
            Console.SetCursorPosition(startX, startY + (i + 1));
        }
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public static void BorderText(int startX, int startY, string header, string[] text)
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        int headerOffset = 5;
        int textOffset = 2;
        Console.SetCursorPosition(startX + headerOffset, startY);
        Console.Write($" {header} ");
        startY++;
        Console.SetCursorPosition(startX + textOffset, startY);

        for (int i = 0; i < text.Length; i++)
        {
            Console.SetCursorPosition(startX + textOffset, startY + i);
            Console.Write(text[i]);
        }
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    
    public static string Header(string path)
    {
        char separator = '/';
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            separator = '\\';

        string[] splits = path.Split(separator);
        string header = string.Join($" {bold}{orange}\x1b[0m {green}{bold}", splits);
        header = $"{header}{reset}";

        (int, int) cursorPos = Console.GetCursorPosition();

        Console.SetCursorPosition(0, 5);
        Console.Write(deleteLine);
        Console.Write("\e[0G"); // Move cursor to column 0
        Console.Write(header);

        // Debug ----
        // Console.SetCursorPosition(0, 4);
        // Console.Write($"{blue}Debug:{reset} Columns: {Console.WindowWidth}, Rows: {Console.WindowHeight}");
        // ----------
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);

        return header;
    }

    public static void HelpWindow()
    {
        ExplorerDraw.Border(70, 3, 44, 12);
        string helpHeader = $"{bold}{green}Quick Help{reset}";
        string[] helpWindowText = [
            $"{mellow}Navigate Up/Down            󰜷/󰁆 or k/j",
            "Move To Parent Directory    󰁎 or h",
            "Move To Sub Directory       󰜴 or l",
            "Open Editor                 e",
            "Add File/Directory          a",
            "Delete File/Directory       d",
            "Copy/Yank File              y",
            "Paste File                  p",
            "Remove Status Message       Backspace",
           $"Quit                        q{reset}",
        ];
        ExplorerDraw.BorderText(70, 3, helpHeader, helpWindowText);
    }


    public static string WriteDisplayText(ExplorerItem item, bool isCurrentItem, ref string errMessage)
    {
        string displayName = string.Empty;

        if (item.Type == ExplorerType.DIRECTORY)
        {
            displayName = $"\x1b[38;5;105m{bold}  {item.DisplayName}\x1b[0m";
        }
        else
        {
            try
            {
                if (System.IO.Path.GetExtension(item.Path) == ".cs")
                    displayName = $"\x1b[38;5;11m  {item.DisplayName}{reset}";
                else if (System.IO.Path.GetExtension(item.Path) == ".c")
                    displayName = $"\x1b[38;5;208m  {item.DisplayName}{reset}";
                else if (ExplorerItem.IsBinaryFile(item.Path, 100, ref errMessage))
                    displayName = $"\x1b[1;36m  {item.DisplayName}{reset}";
                else
                    displayName = $"\x1b[33m  {item.DisplayName}{reset}";
            }
            catch (UnauthorizedAccessException)
            {
                if (isCurrentItem)
                    return $"{red}    {item.DisplayName}{reset}";
                else
                    return $"{red}     {item.DisplayName}{reset}";
            }

        }

        if (isCurrentItem)
            displayName = $" {bold}{orange}{reset}  {displayName}";
        else
            displayName = $"   {displayName}";

        return displayName;
    }

    public static string GetFormattedText(ExplorerItem item)
    {
        string displayName = string.Empty;
        string errMessage = string.Empty;

        if (item.Type == ExplorerType.DIRECTORY)
        {
            displayName = $"\x1b[38;5;105m{bold} {item.DisplayName}\x1b[0m";
        }
        else
        {
            try
            {
                if (System.IO.Path.GetExtension(item.Path) == ".cs")
                    displayName = $"\x1b[38;5;11m {item.DisplayName}{reset}";
                else if (System.IO.Path.GetExtension(item.Path) == ".c")
                    displayName = $"\x1b[38;5;208m {item.DisplayName}{reset}";
                else if (ExplorerItem.IsBinaryFile(item.Path, 100, ref errMessage))
                    displayName = $"\x1b[1;36m {item.DisplayName}{reset}";
                else
                    displayName = $"\x1b[33m {item.DisplayName}{reset}";
            }
            catch (UnauthorizedAccessException)
            {
                    return $"{red} {item.DisplayName}{reset}";
            }

        }
        return displayName;
    }

    public static void OutputText(string output) // Debug
    {
        Console.SetCursorPosition(15, 70);
        Console.Write(output);

        Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
    }

}
