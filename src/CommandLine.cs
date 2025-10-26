using System.Text;

namespace MshExplorer;

class CommandLine
{
    const string darkBlue = "\e[38;2;61;64;91m";
    const string green = "\e[38;2;129;178;154m";
    const string blue = "\e[38;2;41;81;242m";
    
    const string bold = "\e[1m";
    const string reset = "\e[0m";
    const string deleteLine = "\e[2K";
    const string hideCursor = "\e[?25l";
    const string showCursor = "\e[?25h";

    public string Header;
    public string ToolTip;
    public string LastCommand;
    private (int, int) CursorPos;

    public CommandLine()
    {
        Header = string.Empty;
        ToolTip = string.Empty;
        LastCommand = string.Empty;
    }


    public string GetString()
    {
        DrawCommandLine();
        StringBuilder inputBuilder = new();
        ConsoleKeyInfo key;
        int maxLength = 40;
        string item = string.Empty;
        while (true)
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace && inputBuilder.Length > 0)
            {
                inputBuilder.Remove(inputBuilder.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (inputBuilder.Length < maxLength && !char.IsControl(key.KeyChar))
            {
                inputBuilder.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
        }
        if (key.Key == ConsoleKey.Enter)
        {
            item = inputBuilder.ToString();
        }

        RemoveCommandLine();
        LastCommand = item;
        return item;
    }



    public void DrawCommandLine()
    {
        CursorPos = Console.GetCursorPosition();
        int startX = 0;
        int startY = 0;
        int width = 48;
        int height = 2;
        Console.SetCursorPosition(startX, startY);

        for (int i = 0; i <= height; i++)
        {
            for (int j = 0; j <= width; j++)
            {
                if (i == 0)
                {
                    if (j == 0)
                        Console.Write($"{darkBlue}╭");
                    else if (j == width)
                        Console.Write("╮");
                    else
                        Console.Write("─");
                }
                else if (i == height)
                {
                    if (j == 0)
                        Console.Write("╰");
                    else if (j == width)
                        Console.Write($"╯{reset}");
                    else
                        Console.Write("─");
                }
                else
                {
                    if (j == 0)
                        Console.Write("│");
                    else if (j == width)
                        Console.Write("│");
                    else
                        Console.Write(" ");
                }
            }
            Console.SetCursorPosition(startX, startY + (i + 1));
        }
        if (!string.IsNullOrEmpty(Header))
        {
            Console.SetCursorPosition(3, 0);
            Console.Write($" {green}{bold}{Header}{reset} ");
        }

        if (!string.IsNullOrEmpty(ToolTip))
        {
            Console.SetCursorPosition(50, 0);
            Console.Write($" {blue}{reset} {ToolTip}");
        }

        Console.SetCursorPosition(3, 2);
        Console.Write("Enter to Confirm ─ Esc to Cancel ");

        Console.SetCursorPosition(2, 1); // Input location
        Console.Write($"{bold}>{reset} ");
        Console.Write(showCursor);

    }

    public void RemoveCommandLine()
    {
        Console.Write(deleteLine);
        Console.Write(deleteLine); Console.SetCursorPosition(0, 0);
        Console.Write(deleteLine); Console.SetCursorPosition(0, 2);
        Console.Write(deleteLine);

        Console.SetCursorPosition(CursorPos.Item1, CursorPos.Item2);
        Console.Write(hideCursor);

    }
}
