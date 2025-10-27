
namespace MshExplorer;

static class TextStore
{
    const string reset = "\e[0m";
    const string bold = "\e[1m";
    const string mellow = "\e[38;2;154;151;132m";
    const string green = "\e[38;2;129;178;154m";

    public const string HelpHeader = $"{bold}{green}Quick Help{reset}";
    public static string[] HelpWindowText = [
        $"{mellow}Navigate Up/Down            ↑/↓ or k/j",
            "Move Between Directories    ←/→ or h/l",
            "Add File/Directory          a",
            "Delete File/Directory       d",
            "Copy File/Directory         y",
            "Paste File/Directory        p",
            "Open Editor                 e",
            "Explorer CommandLine        :",
            "Toggle floating Window      ?",
            "Remove Status Message       Backspace",
           $"Quit                        q{reset}",
        ];
}
