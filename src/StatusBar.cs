
namespace MshExplorer;

class StatusBar
{
    public string ErrorMessage;
    private string StatusBarText;
    public bool Notify = false;
    public int SelectedIndex;
    public int TotalItems;

    public string Editor;
    public bool NerdFont;
    public StatusBar()
    {
        ErrorMessage = string.Empty;
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
       // Console.SetCursorPosition(0, Math.Max(0, Console.WindowHeight - 1));
        Console.Write($"\x1b[{Console.WindowHeight + 1};{0}H"); // Maybe faster than setcursor
        StatusBarText = $" {SelectedIndex}/{TotalItems} ";

        if (!string.IsNullOrEmpty(Editor))
            StatusBarText = $"{StatusBarText} | {Ansi.GetFormattedEditor(Editor, NerdFont)}";


        Console.Write(Ansi.eraseLine);

        if (Notify)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                StatusBarText = $"{StatusBarText}{Ansi.bgDark} | {Ansi.red}{ErrorMessage}{Ansi.reset}";
        }
        

        Console.Write($"{Ansi.bgDark} {StatusBarText}{Ansi.bgDark}\x1b[K{Ansi.reset}");
    }


        public void UpdateConfigs(UserConfigs configs)
    {
        Editor = configs.Editor;
        NerdFont = configs.NerdFont;

    }


}
