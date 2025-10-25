
namespace MshExplorer;

class StatusBar
{
    const string red = "\e[38;2;186;61;52m";
    const string green = "\e[38;2;129;178;154m";
    const string bgDark = "\e[48;2;31;43;61m";
    const string eraseLine = "\e[2K";
    const string bold = "\e[1m";
    const string reset = "\e[0m";
    

    public string ErrorMessage;
    private string StatusBarText;
    public ExplorerItem ClipboardItem;
    public bool Notify = false;
    public int SelectedIndex;
    public int TotalItems;

    public StatusBar()
    {
        ErrorMessage = string.Empty;
        ClipboardItem = new(string.Empty, string.Empty, ExplorerType.NONE);
        StatusBarText = string.Empty;
    }

    public void Draw()
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(0, Console.WindowHeight);
        StatusBarText = $" {SelectedIndex}/{TotalItems} ";

        if (ClipboardItem.Type != ExplorerType.NONE)
            StatusBarText = $"{StatusBarText} | {ExplorerDraw.GetFormattedText(ClipboardItem)}";

        Console.Write(eraseLine);
        if (Notify)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                StatusBarText = $"{StatusBarText} | {red}{ErrorMessage}{reset}";
        }
        Console.Write($"{bgDark} {StatusBarText}{bgDark}\x1b[K{reset}");


        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }
    public void AddClipboardItem(ExplorerItem item)
    {
        ClipboardItem = new(item.DisplayName, item.Path, item.Type);
        Draw();
    }
    public void ClearClipboardItem()
    {
        ClipboardItem = new(string.Empty, string.Empty, ExplorerType.NONE);
        Draw();
    }


}
