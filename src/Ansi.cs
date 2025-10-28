
namespace MshExplorer;

static class Ansi
{
    public const string reset = "\e[0m";
    public const string deleteLine = "\e[2K";
    public const string hideCursor = "\e[?25l";
    public const string showCursor = "\e[?25h";
    public const string eraseLine = "\e[2K";

    public const string bold = "\e[1m";
    public const string orange = "\e[38;2;224;122;95m";
    public const string green = "\e[38;2;129;178;154m";
    public const string blue = "\e[38;2;41;81;242m";
    public const string darkBlue = "\e[38;2;61;64;91m";
    public const string red = "\e[38;2;186;61;52m";
    public const string mellow = "\e[38;2;154;151;132m";
    public const string yellow = "\x1b[38;5;11m";

    public const string vimColor = "\e[38;2;90;157;82m";
    public const string nanoColor = "\e[38;2;117;0;204m";
    public const string vscColor = "\e[38;2;51;128;203m";
    public const string emacsColor = "\x1b[38;5;135m";

    public const string bgDark = "\e[48;2;31;43;61m";
    public const string bgMellow = "\e[48;2;154;151;132m";

    // All
    public static string Border = darkBlue;
    public static string Header = bold + green;


    // Floating Window
    public static string HelpText = mellow;

    // CommandLine
    public static string ToolTip = blue;
    public static string CommandCur = bold;

    // StatusBar
    public static string StatusBG = bgDark;

    // PathBar
    public static string PathBNames = bold + green;
    public static string PathBDivider = bold + orange;

    // ListWindow
    public static string ListCur = bold + orange;


    // File & Directory Colors
    public static string dirColor = "\x1b[38;5;105m";



    public static readonly Dictionary<string, string> NerdEditors = new Dictionary<string, string>
    {
        ["vim"] = $"{vimColor}{bold} Vim{reset}",
        ["nvim"] = $"{vimColor}{bold} NeoVim{reset}",
        ["nano"] = $"{nanoColor}{bold} nano{reset}",
        ["code"] = $"{vscColor}{bold} Vs Code{reset}",
        ["notepad"] = $"{vscColor}{bold} notepad{reset}",
        ["emacs"] = $"{emacsColor}{bold} Emacs{reset}",
        ["pico"] = $"{bold}{nanoColor} pico{reset}",
    };

    public static readonly Dictionary<string, string> Editors = new Dictionary<string, string>
    {
        ["vim"] = $"{vimColor}{bold} Vim{reset}",
        ["nvim"] = $"{vimColor}{bold} NeoVim{reset}",
        ["nano"] = $"{nanoColor}{bold} nano{reset}",
        ["code"] = $"{vscColor}{bold} Vs Code{reset}",
        ["notepad"] = $"{vscColor}{bold} notepad{reset}",
        ["emacs"] = $"{emacsColor}{bold} Emacs{reset}",
        ["pico"] = $"{bold}{nanoColor} pico{reset}",
    };

    public static string GetFormattedEditor(string editor, bool nerdFont)
    {
        if (string.IsNullOrWhiteSpace(editor) || editor == "null")
            return string.Empty;

        if (nerdFont)
        {
            if (NerdEditors.TryGetValue(editor, out var val))
                return val;
        }
        else
        {
            if (Editors.TryGetValue(editor, out var val))
                return val;
        }

        return string.Empty;

    }

    private static readonly Dictionary<string, (string nerd, string uni, string color)> _fileMap =
         new(StringComparer.OrdinalIgnoreCase)
         {
             [".cs"] = ("", "🗎", "\x1b[38;5;11m"),   // Yellow
             [".c"] = ("", "🗎", "\x1b[38;5;208m"),  // Orange
             [".cpp"] = ("", "🗎", "\x1b[38;5;33m"),  // Blue
             [".txt"] = ("", "🗎", "\x1b[33m"),       // Yellow
         };

    public static string GetFormattedText(ExplorerItem item, bool hasNerdFont)
    {
        if (item.Type == ExplorerType.DIRECTORY)
        {
            var icon = hasNerdFont ? "" : "🗀";
            return $"{dirColor}{bold}{icon} {item.DisplayName}{reset}";
        }

        string err = string.Empty;
        string ext = Path.GetExtension(item.Path);

        try
        {
            if (_fileMap.TryGetValue(ext, out var fmt))
            {
                var icon = hasNerdFont ? fmt.nerd : fmt.uni;
                return $"{fmt.color}{icon} {item.DisplayName}{reset}";
            }

            if (ExplorerItem.IsBinaryFile(item.Path, 100, ref err))
            {
                var icon = hasNerdFont ? "" : "🗎";
                return $"\x1b[1;36m{icon} {item.DisplayName}{reset}";
            }

            var defaultIcon = hasNerdFont ? "" : "🗎";
            return $"\x1b[33m{defaultIcon} {item.DisplayName}{reset}";
        }
        catch (UnauthorizedAccessException)
        {
            var icon = hasNerdFont ? "" : "🗎";
            return $"{red}{icon} {item.DisplayName}{reset}";
        }
    }


    public static void Write(int x, int y, string text)
    {
        (int X, int Y) curPos = Console.GetCursorPosition();

        Console.SetCursorPosition(x, y);
        Console.Write(text);

        Console.SetCursorPosition(curPos.X, curPos.Y);
    }
}
