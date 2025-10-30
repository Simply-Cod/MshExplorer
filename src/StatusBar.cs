
namespace MshExplorer;

class StatusBar
{
    public string ErrorMessage;
    private string StatusBarText;
    public ExplorerItem ClipboardItem;
    public bool Notify = false;
    public int SelectedIndex;
    public int TotalItems;

    public string Editor;
    public bool NerdFont;
    public StatusBar()
    {
        ErrorMessage = string.Empty;
        ClipboardItem = new(string.Empty, string.Empty, ExplorerType.NONE);
        StatusBarText = string.Empty;
        Editor = string.Empty;
        NerdFont = false;
    }
    public void SetIndexAndCount(int index, int count)
    {
        if (count == 0)
        {
            SelectedIndex = 0;
            TotalItems = 0;
        }
        else
        {
            SelectedIndex = index + 1;
            TotalItems = count;
        }
    }

    public void Draw()
    {
        Console.SetCursorPosition(0, Math.Max(0, Console.WindowHeight - 1));
        StatusBarText = $" {SelectedIndex}/{TotalItems} ";

        if (!string.IsNullOrEmpty(Editor))
            StatusBarText = $"{StatusBarText} | {Ansi.GetFormattedEditor(Editor, NerdFont)}";


        Console.Write(Ansi.eraseLine);

        if (Notify)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                StatusBarText = $"{StatusBarText}{Ansi.bgDark} | {Ansi.red}{ErrorMessage}{Ansi.reset}";
        }
        else
        {
            if (ClipboardItem.Type != ExplorerType.NONE)
                StatusBarText = $"{StatusBarText}{Ansi.bgDark} | {Ansi.GetFormattedText(ClipboardItem, NerdFont)}";
        }

        Console.Write($"{Ansi.bgDark} {StatusBarText}{Ansi.bgDark}\x1b[K{Ansi.reset}");
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
    public void UpdateConfigs(UserConfigs configs)
    {
        Editor = configs.Editor;
        NerdFont = configs.NerdFont;

    }


}
