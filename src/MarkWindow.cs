using System.Text;

namespace MshExplorer;

class MarkWindow
{
    private const int RightPadding = 5;
    private const int TopPadding = 6;
    private const int ListWindowRequiredWidth = 50;
    private const int MinimumHeight = 15;
    private const int MinimumWidth = 40;

    public bool ScreenSizeBigEnough;
    public bool HideWindow;

    public int StartX;
    public int StartY;
    public int Width;
    public int Height;

    public List<ExplorerItem> MarkedList;

    public bool NerdFont;
    public HelpStyler Style;

    public MarkWindow()
    {
        Width = Console.WindowWidth - ListWindowRequiredWidth - RightPadding;
        Height = Console.WindowHeight - TopPadding - 2; // 2 for statusbar 

        StartX = ListWindowRequiredWidth;
        StartY = TopPadding;

        HideWindow = false;

        ScreenSizeBigEnough = ListWindowRequiredWidth + (Width + RightPadding) < Console.WindowWidth &&
            Height + TopPadding < Console.WindowHeight;

        NerdFont = false;
        Style = new();
        MarkedList = new();
    }
    public void SetStyle(HelpStyler style)
    {
        Style.InfoHeader = style.InfoHeader;
        Style.Active = style.Active;
        Style.Border = style.Border;
        Style.InfoHL = style.InfoHL;
    }
    public bool CheckWindowSize()
    {
        Width = Console.WindowWidth - ListWindowRequiredWidth - RightPadding;
        Height = Console.WindowHeight - TopPadding - 2; // 2 for statusbar 

        ScreenSizeBigEnough = Width > MinimumWidth &&
            Height > MinimumHeight;

        return ScreenSizeBigEnough;
    }
    public void SetSize()
    {
        Width = Console.WindowWidth - ListWindowRequiredWidth - RightPadding;
        Height = Console.WindowHeight - TopPadding - 2;
        StartX = ListWindowRequiredWidth;
        StartY = TopPadding;
    }

    private void DrawBorder()
    {
        StringBuilder sb = new();
        string top = $"{Style.Border}╭{new string('─', Width - 1)}╮\n";
        string middle = $"│{new string(' ', Width - 1)}│\n";
        string bottom = $"╰{new string('─', Width - 1)}╯{Style.Reset}\n";


        for (int i = 0; i <= Height; i++)
        {
            Console.SetCursorPosition(StartX, StartY + i);

            if (i == 0)
                Console.Write(top);
            else if (i == Height)
                Console.Write(bottom);
            else
                Console.Write(middle);
        }
    }
    private void DrawWindowText(string[] text, bool truncate)
    {
        int textOffset = 2;
        int x = StartX;
        int y = StartY + 1;

        Console.SetCursorPosition(x + textOffset, y);

        for (int i = 0; i < text.Length; i++)
        {
            string t = Ansi.ConvertTabsToSpaces(text[i]);
            Console.SetCursorPosition(x + textOffset, y + i);
            if (truncate)
                Console.Write($"{Style.Text}{Ansi.TruncateString(t, Width - 2)}{Style.Reset}"); // Test add truncation
            else
                Console.Write($"{Style.Text}{t}{Style.Reset}"); // Test add truncation
        }
    }

    public void DrawMarks()
    {
        DrawBorder();
        int textStartX = StartX + 2;
        int textStartY = StartY + 2;
        int maxLength = Width - 8;


        if (MarkedList.Count <= 0)
            return;

        for (int i = 0; i < MarkedList.Count; i++)
        {
            if (i >= Height - 2)
                return;

            Console.SetCursorPosition(textStartX, textStartY + i);
            if(Style.Active)
                Console.Write($"{i} " + Ansi.GetFormattedText(MarkedList[i], NerdFont, 40));
            else
                Console.Write($"{i} " + Ansi.TruncateString(MarkedList[i].DisplayName, 40));

        }
    }

    public void ClearWindow()
    {
        Console.SetCursorPosition(StartX, StartY);

        int y = StartY;
        for (int i = 0; i < Height + TopPadding; i++)
        {
            Console.Write("\e[0K"); // Erase Rest of Line
            Console.SetCursorPosition(StartX, StartY + i);
        }
    }
}
