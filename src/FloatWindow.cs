
using System.Text;

namespace MshExplorer;

class FloatingWindow
{

    const string reset = "\e[0m";
    const string bold = "\e[1m";
    const string darkBlue = "\e[38;2;61;64;91m";
    const string mellow = "\e[38;2;154;151;132m";
    const string green = "\e[38;2;129;178;154m";

    private const int RightPadding = 5;
    private const int TopPadding = 5;
    private const int ListWindowRequiredWidth = 50;

    public bool ScreenSizeBigEnough;
    public bool HideWindow;

    public int StartX;
    public int StartY;
    public int Width;
    public int Height;

    public FloatingWindow(int width, int height)
    {
        Width = width;
        Height = height;

        StartX = Console.WindowWidth - (Width + RightPadding);
        StartY = TopPadding;

        HideWindow = false;

        ScreenSizeBigEnough = ListWindowRequiredWidth + (Width + RightPadding) < Console.WindowWidth &&
            Height + TopPadding < Console.WindowHeight;
    }

    public bool CheckWindowSize()
    {
        StartX = Console.WindowWidth - (Width + RightPadding);

        return ListWindowRequiredWidth + (Width + RightPadding) < Console.WindowWidth &&
            Height + TopPadding < Console.WindowHeight;
    }

    

    private void DrawBorder()
    {
        StringBuilder sb = new();
        string top = $"{darkBlue}╭{new string('─', Width - 1)}╮\n";
        string middle = $"│{new string(' ', Width - 1)}│\n";
        string bottom = $"╰{new string('─', Width - 1)}╯{reset}\n";

        (int, int) cursorPos = Console.GetCursorPosition();
        
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
        
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }



    private void DrawWindowText(string header, string[] text)
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        int headerIndent = 5;
        int textOffset = 2;
        int x = StartX;
        int y = StartY;

        Console.SetCursorPosition(x + headerIndent, y);
        Console.Write($" {header} ");
        y++;
        Console.SetCursorPosition(x + textOffset, y);

        for (int i = 0; i < text.Length; i++)
        {
            Console.SetCursorPosition(x + textOffset, y + i);
            Console.Write(text[i]);
        }
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public void DrawQuickHelp(string helpHeader, string[] helpText)
    {
        DrawBorder();
        DrawWindowText(helpHeader, helpText);
    }


    public void ClearWindow()
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(StartX, StartY);

        int y = StartY;
        for (int i = 0; i < Height + TopPadding; i++)
        {
            Console.Write("\e[0K"); // Erase Rest of Line
            Console.SetCursorPosition(StartX, StartY + i);
        }
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }


}
