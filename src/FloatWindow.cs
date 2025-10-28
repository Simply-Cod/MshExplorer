
using System.Text;

namespace MshExplorer;

class FloatingWindow
{
    private const int RightPadding = 5;
    private const int TopPadding = 5;
    private const int ListWindowRequiredWidth = 50;

    public bool ScreenSizeBigEnough;
    public bool HideWindow;

    public int StartX;
    public int StartY;
    public int Width;
    public int Height;

    public bool NerdFont;
    public HelpStyler Style;

    public FloatingWindow(int width, int height)
    {
        Width = width;
        Height = height;

        StartX = Console.WindowWidth - (Width + RightPadding);
        StartY = TopPadding;

        HideWindow = false;

        ScreenSizeBigEnough = ListWindowRequiredWidth + (Width + RightPadding) < Console.WindowWidth &&
            Height + TopPadding < Console.WindowHeight;

        NerdFont = false;
        Style = new();
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
        string top = $"{Style.Border}╭{new string('─', Width - 1)}╮\n";
        string middle = $"│{new string(' ', Width - 1)}│\n";
        string bottom = $"╰{new string('─', Width - 1)}╯{Style.Reset}\n";

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
        Console.Write($" {Style.Header}{header}{Style.Reset} ");
        y++;
        Console.SetCursorPosition(x + textOffset, y);

        for (int i = 0; i < text.Length; i++)
        {
            Console.SetCursorPosition(x + textOffset, y + i);
            Console.Write($"{Style.Text}{text[i]}{Style.Reset}");
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

    public void UpdateConfigs(UserConfigs configs)
    {
        NerdFont = configs.NerdFont;
        Style.Active = configs.HelpStyle;

        if (Style.Active)
            Style.Activate();
        else
            Style.Deactivate();
    }

    public void ConfigWindow(UserConfigs configs)
    {
        if (!ScreenSizeBigEnough)
        {
            Util.Clear();
            Ansi.Write(StartX + 5, StartY + Height, $"{Style.Text}Press Enter(Confirm) Esc/q(Cancel){Style.Reset}");
            TuiConfig(configs);
            ClearWindow();
        }
        else
        {
            DrawBorder();
            Ansi.Write(StartX + 5, StartY + Height, $"{Style.Text}Press Enter(Confirm) Esc/q(Cancel){Style.Reset}");
            TuiConfig(configs);
            ClearWindow();
        }

    }
    private void TuiConfig(UserConfigs userConfigs)
    {
        UserConfigs configs = new();
        configs = userConfigs;
        string[] editors = [
                "vim",
                "nvim",
                "nano",
                "notepad",
                "code",
                "emacs",
                "pico",
                "null"
                ];
        string ConfigHeader = "Config";


        bool[] configurations = [configs.NerdFont, configs.ListStyle, configs.PathStyle,
        configs.HelpStyle];

        (int X, int Y) CurPos = Console.GetCursorPosition();
        int configStartX = StartX + 23;
        int configStartY = StartY + 1;

        Console.SetCursorPosition(configStartX, configStartY);

        int select = 0;
        ConsoleKeyInfo key;
        bool isConfiging = true;
        while (isConfiging)
        {
            string[] ConfigText = [
    $"Editor                 {Ansi.bold}[{Ansi.yellow}{Ansi.GetFormattedEditor(configs.Editor,configs.NerdFont)}{Ansi.reset}{Ansi.bold}{Ansi.mellow}]{Ansi.reset}     ",
    $"NerdFont               {Ansi.bold}[{Ansi.yellow}{configs.NerdFont}{Ansi.reset}{Ansi.bold}{Ansi.mellow}]{Ansi.reset}   ",
    $"List Window Style      {Ansi.bold}[{Ansi.yellow}{configs.ListStyle}{Ansi.reset}{Ansi.bold}{Ansi.mellow}]{Ansi.reset}  ",
    $"Path Bar Style         {Ansi.bold}[{Ansi.yellow}{configs.PathStyle}{Ansi.reset}{Ansi.bold}{Ansi.mellow}]{Ansi.reset}  ",
    $"Float Window Style     {Ansi.bold}[{Ansi.yellow}{configs.HelpStyle}{Ansi.reset}{Ansi.bold}{Ansi.mellow}]{Ansi.reset}  ",
            ];

            DrawWindowText(ConfigHeader, ConfigText);
            Console.SetCursorPosition(configStartX, configStartY + select);
            Console.Write($"{Ansi.bold}{Ansi.orange}>{Ansi.reset}");
            key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                    isConfiging = false;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.J:
                    if (select == ConfigText.Length - 1)
                        select = 0;
                    else
                        select++;
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.K:
                    if (select == 0)
                        select = ConfigText.Length - 1;
                    else
                        select--;
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.L:
                    if (select == 0)
                    {
                        int editorSelect = Array.IndexOf(editors, configs.Editor);

                        if (editorSelect == editors.Length - 1)
                            editorSelect = 0;
                        else
                            editorSelect++;
                        configs.Editor = editors[editorSelect];
                    }
                    else
                    {
                        configurations[select - 1] = !configurations[select - 1];
                    }
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.H:
                    if (select == 0)
                    {
                        int editorSelect = Array.IndexOf(editors, configs.Editor);

                        if (editorSelect == 0)
                            editorSelect = editors.Length - 1;
                        else
                            editorSelect--;
                        configs.Editor = editors[editorSelect];
                    }
                    else
                    {
                        configurations[select - 1] = !configurations[select - 1];
                    }
                    break;
                case ConsoleKey.Enter:
                    userConfigs = configs;
                    isConfiging = false;
                    break;

            }
            configs.NerdFont = configurations[0];
            configs.ListStyle = configurations[1];
            configs.PathStyle = configurations[2];
            configs.HelpStyle = configurations[3];
        }

        Console.SetCursorPosition(CurPos.X, CurPos.Y);

    }


}
