using System.Text;

namespace MshExplorer;

class CommandLine
{
    public string Header;
    public string ToolTip;
    public string LastCommand;

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

    private void DrawBorder()
    {
        int width = 48;

        StringBuilder sb = new();
        string top = $"{Ansi.Border}╭{new string('─', width - 1)}╮\n";
        string middle = $"│{new string(' ', width - 1)}│\n";
        string bottom = $"╰{new string('─', width - 1)}╯{Ansi.reset}\n";
        sb.Append(top).Append(middle).Append(bottom);
        Console.SetCursorPosition(0,0);
        Console.Write(sb.ToString());
        
    }

    public void DrawCommandLine()
    {
        DrawBorder();

        if (!string.IsNullOrEmpty(Header))
        {
            Console.SetCursorPosition(3, 0);
            Console.Write($" {Ansi.green}{Ansi.bold}{Header}{Ansi.reset} ");
        }

        if (!string.IsNullOrEmpty(ToolTip))
        {
            Console.SetCursorPosition(0, 3);
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
        Console.SetCursorPosition(0, 0);
        Console.Write(Ansi.deleteLine); Console.SetCursorPosition(0, 1);
        Console.Write(Ansi.deleteLine); Console.SetCursorPosition(0, 2);
        Console.Write(Ansi.deleteLine); Console.SetCursorPosition(0, 3);
        Console.Write(Ansi.deleteLine);

        Console.Write(Ansi.hideCursor);

    }

    
}
