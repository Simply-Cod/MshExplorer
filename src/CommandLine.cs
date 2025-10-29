using System.Text;

namespace MshExplorer;

class CommandLine
{
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
                        Console.Write($"{Ansi.darkBlue}╭");
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
                        Console.Write($"╯{Ansi.reset}");
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
            Console.Write($" {Ansi.green}{Ansi.bold}{Header}{Ansi.reset} ");
        }

        if (!string.IsNullOrEmpty(ToolTip))
        {
            Console.SetCursorPosition(50, 0);
            Console.Write($" {Ansi.blue}{Ansi.reset} {ToolTip}");
        }

        Console.SetCursorPosition(3, 2);
        Console.Write("Enter to Confirm ─ Esc to Cancel ");

        Console.SetCursorPosition(2, 1); // Input location
        Console.Write($"{Ansi.bold}>{Ansi.reset} ");
        Console.Write(Ansi.showCursor);

    }

    public void RemoveCommandLine()
    {
        Console.Write(Ansi.deleteLine);
        Console.Write(Ansi.deleteLine); Console.SetCursorPosition(0, 0);
        Console.Write(Ansi.deleteLine); Console.SetCursorPosition(0, 2);
        Console.Write(Ansi.deleteLine);

        Console.SetCursorPosition(CursorPos.Item1, CursorPos.Item2);
        Console.Write(Ansi.hideCursor);

    }
}
