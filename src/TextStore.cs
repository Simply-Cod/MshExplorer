
namespace MshExplorer;

static class TextStore
{
    public const string HelpHeader = "Quick Help";
    public static string[] HelpWindowText = [
            "Navigate Up/Down            ↑/↓ or k/j",
            "Move Between Directories    ←/→ or h/l",
            "Add File/Directory          a",
            "Delete File/Directory       d",
            "Copy File/Directory         y",
            "Paste File/Directory        p",
            "Open Editor                 e",
            "Explorer CommandLine        :",
            "Toggle floating Window      ?",
            "Search Index                /",
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
        " 'home'        Takes you back to start",
        " 'config'      Set your configurations",
    ];
}
