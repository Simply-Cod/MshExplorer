
namespace MshExplorer;

static class TextStore
{
    public const string HelpHeader = "[?] Quick Help";
    public static string[] HelpWindowText = [
            "Navigate Up/Down            ↑/↓ or k/j",
            "Move Between Directories    ←/→ or h/l",
            "Add File/Directory          a",
            "Mark mode                   m",
            "Bookmarks                   b",
            "Delete File/Directory       D",
            "Open Editor                 e",
            "Explorer CommandLine        :",
            "Toggle floating Window      ?",
            "Search Index                /",
            "Recursive File Search       F",
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
        $"{Ansi.mellow}[m]    Toggle Mark     [a]     Mark all{Ansi.reset}",
        $"{Ansi.mellow}[c]    Clear           [p]     Paste{Ansi.reset}",
        $"{Ansi.mellow}[g]    Go to           {Ansi.reset}",
    ];
    public static string[] markPasteMode = [
        $"{Ansi.mellow}         {Ansi.green}--- Paste ---{Ansi.reset}",
        $"{Ansi.mellow}<index>    at index    [a]     Paste all{Ansi.reset}",
        $"{Ansi.mellow}[p]        Paste Last  [f]     Paste first{Ansi.reset}",
    ];
    public static string[] markClearMode = [
        $"         {Ansi.green}--- Clear ---{Ansi.reset}",
        $"{Ansi.mellow}<index>    at index    [a]     Clear all{Ansi.reset}",
        $"{Ansi.mellow}[c]        Clear last  [f]     Clear first{Ansi.reset}",
    ];

    public static string[] markGoToMode = [
        $"         {Ansi.green}--- Go To ---{Ansi.reset}",
        $"{Ansi.mellow}<index>    at index    [f]     Go to first{Ansi.reset}",
        $"{Ansi.mellow}[l]        Go to last  {Ansi.reset}",
    ];
    public static string[] bookmarkMode = [
        $"       {Ansi.green}--- Bookmarks ---{Ansi.reset}",
        $"{Ansi.mellow}[a]    Add Bookmark      [b]     Go to Bookmark{Ansi.reset}",
        $"{Ansi.mellow}[c]    Clear Bookmark     {Ansi.reset}",
    ];

    public static string[] bookmarkAdd = [
        $"       {Ansi.green}--- Listening for hot key ---{Ansi.reset}",
        $"{Ansi.mellow}Type the hot key you want the bookmark under.{Ansi.reset}",
        
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
            Console.Write("\e[2K"); // Delete line
        }
    }
}
