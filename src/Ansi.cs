
namespace MshExplorer;

class Ansi
{
    const string reset = "\e[0m";
    const string deleteLine = "\e[2K";
    const string hideCursor = "\e[?25l";
    const string showCursor = "\e[?25h";
    const string eraseLine = "\e[2K";

    const string bold = "\e[1m";
    const string orange = "\e[38;2;224;122;95m";
    const string green = "\e[38;2;129;178;154m";
    const string blue = "\e[38;2;41;81;242m";
    const string darkBlue = "\e[38;2;61;64;91m";
    const string red = "\e[38;2;186;61;52m";
    const string mellow = "\e[38;2;154;151;132m";

    public const string vimColor = "\e[38;2;90;157;82m";
    public const string nanoColor = "\e[38;2;117;0;204m";
    public const string vscColor = "\e[38;2;51;128;203m";

    const string bgDark = "\e[48;2;31;43;61m";
    const string bgMellow = "\e[48;2;154;151;132m";

    public static readonly Dictionary<string, string> NerdEditors = new Dictionary<string, string>
    {
        ["vim"] = $"{vimColor}{bold} Vim{reset}",
        ["nvim"] = $"{vimColor}{bold} NeoVim{reset}",
        ["nano"] = $"{nanoColor}{bold} nano{reset}",
        ["code"] = $"{vscColor}{bold} Vs Code{reset}",
        ["notepad"] = $"{vscColor}{bold} notepad{reset}",
    };

    public static readonly Dictionary<string, string> Editors = new Dictionary<string, string>
    {
        ["vim"] = $"{vimColor}{bold} Vim{reset}",
        ["nvim"] = $"{vimColor}{bold} NeoVim{reset}",
        ["nano"] = $"{nanoColor}{bold} nano{reset}",
        ["code"] = $"{vscColor}{bold} Vs Code{reset}",
        ["notepad"] = $"{vscColor}{bold} notepad{reset}",
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

    public static string GetFormattedText(ExplorerItem item, bool hasNerdFont)
    {
        if(hasNerdFont)
            return GetFormattedNerdFont(item);
        else
            return GetFormattedUniCode(item);
    }


    public static string GetFormattedNerdFont(ExplorerItem item)
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


    public static string GetFormattedUniCode(ExplorerItem item)
    {
        string displayName = string.Empty;
        string errMessage = string.Empty;

        if (item.Type == ExplorerType.DIRECTORY)
        {
            displayName = $"\x1b[38;5;105m{bold}🗀 {item.DisplayName}\x1b[0m";
        }
        else
        {
            try
            {
                if (System.IO.Path.GetExtension(item.Path) == ".cs")
                    displayName = $"\x1b[38;5;11m🗎 {item.DisplayName}{reset}";
                else if (System.IO.Path.GetExtension(item.Path) == ".c")
                    displayName = $"\x1b[38;5;208m🗎 {item.DisplayName}{reset}";
                else if (ExplorerItem.IsBinaryFile(item.Path, 100, ref errMessage))
                    displayName = $"\x1b[1;36m🗎 {item.DisplayName}{reset}";
                else
                    displayName = $"\x1b[33m🗎 {item.DisplayName}{reset}";
            }
            catch (UnauthorizedAccessException)
            {
                return $"{red}🗎 {item.DisplayName}{reset}";
            }

        }
        return displayName;
    }

}
