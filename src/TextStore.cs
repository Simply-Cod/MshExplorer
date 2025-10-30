
namespace MshExplorer;

static class TextStore
{
    public const string HelpHeader = "[?] Quick Help";
    public static string[] HelpWindowText = [
            "Navigate Up/Down            ↑/↓ or k/j",
            "Move Between Directories    ←/→ or h/l",
            "Add File/Directory          a",
            "Mark mode                   m",
            "Delete File/Directory       d",
            "Open Editor                 e",
            "Explorer CommandLine        :",
            "Toggle floating Window      ?",
            "Search Index                /",
            "Toggle Info/Quick help      !",
            "Cycle floating windows      (1-3) or ?",
            "Remove Status Message       Backspace",
            "Quit                        q",
        ];

    public const string ConfigHeader = "Config";
    public static string[] ConfigText = [
        "Editor                 [ ]",
        "NerdFont               [ ]",
        "List Window Style      [ ]",
        "Path Bar Style         [ ]",
        "Float Window Style     [ ]",
    ];

    public const string CommandHeader = "Command";
    public static string[] CommandText = [
        " 'quit'        Quit Explorer",
        " 'set home'    Set current directory to home",
        " 'home'        Takes you back to start",
        " 'config'      Set your configurations",
    ];

    public static string[] Windows = [
        $"[1] [2] [3] {Ansi.InfoHeader}[?] Quck Help{Ansi.reset}",
        $"{Ansi.InfoHeader}[1] Info{Ansi.reset}{Ansi.Border} [2] [3] [?]",
        $"[1] {Ansi.InfoHeader}[2]Preview{Ansi.reset}{Ansi.Border} [3] [?]",
        $"[1] [2] {Ansi.InfoHeader}[3] Marks{Ansi.reset}{Ansi.Border} [?]"
    ];

    public static string[] markMode = [
        $"       {Ansi.green}--- Mark Mode ---{Ansi.reset}",
        $"{Ansi.mellow}[m]    Toggle Mark     [p]     Paste{Ansi.reset}",
        $"{Ansi.mellow}[c]    Clear{Ansi.reset}",
    ];
    public static string[] markPasteMode = [
        $"{Ansi.mellow}         {Ansi.green}--- Paste ---{Ansi.reset}",
        $"{Ansi.mellow}<index>    at index    [a]     Paste all{Ansi.reset}"
    ];
    public static string[] markClearMode = [
        $"         {Ansi.green}--- Clear ---{Ansi.reset}",
        $"{Ansi.mellow}<index>    at index    [a]     Clear all{Ansi.reset}"
    ];
    public static void MarkKeys(string[] markHelp)
    {
        int startX = 50;
        int startY = 0;


        for (int i = 0; i < markHelp.Length; i++)
        {
            Console.SetCursorPosition(startX, startY + i);
            Console.Write(markHelp[i]);
        }
    }
    public static void ClearMarkKeys(int numberOfLines)
    {
        int startX = 50;
        int startY = 0;
        for (int i = 0; i < numberOfLines; i++)
        {
            Console.SetCursorPosition(startX, startY + i);
            Console.Write("\e[0K"); // Delete rest of line
        }
    }
}
