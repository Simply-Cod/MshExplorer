
using System.Text;

namespace MshExplorer;

enum FloatWindowType
{
    INFO,
    PREVIEW,
    MARK,
    HELP,
}
class FloatingWindow
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

    public bool NerdFont;
    public HelpStyler Style;

    public FloatingWindow()
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
    }

    public bool CheckWindowSize()
    {
        Width = Console.WindowWidth - ListWindowRequiredWidth - RightPadding;
        Height = Console.WindowHeight - TopPadding - 2; // 2 for statusbar 

        ScreenSizeBigEnough = Width > MinimumWidth &&
            Height > MinimumHeight;

        return ScreenSizeBigEnough;
    }
    public void SetWindowSize()
    {
        Width = Console.WindowWidth - ListWindowRequiredWidth - RightPadding;
        Height = Console.WindowHeight - TopPadding - 2; // 2 for statusbar 
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

    public void DrawQuickHelp(string helpHeader, string[] helpText)
    {
        DrawBorder();
        DrawWindowText(helpText, truncate: false);
    }


    public void ClearWindow()
    {
        SetWindowSize();
        Console.SetCursorPosition(StartX, StartY);

        for (int i = 0; i < Height; i++)
        {
            Console.Write("\e[0K"); // Erase Rest of Line
            Console.SetCursorPosition(StartX, StartY + i);
        }
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
            DrawWindowPanes(TextStore.Windows[4]); 
            TuiConfig(configs);
            ClearWindow();
        }

    }
    private void TuiConfig(UserConfigs userConfigs)
    {
        UserConfigs configs = new();
        configs = userConfigs;


        bool[] configurations = [configs.NerdFont, configs.ListStyle, configs.PathStyle,
        configs.HelpStyle];

        int configStartX = StartX + 23;
        int configStartY = StartY + 1;

        Console.SetCursorPosition(configStartX, configStartY);

        int select = 0;
        ConsoleKeyInfo key;
        bool isConfiging = true;
        string shortAnsi = $"{Ansi.reset}{Ansi.bold}{Ansi.mellow}";

        while (isConfiging)
        {
            string defaultWindow = TextStore.FloatingWindows[(int)configs.DefaultWindow];
            string e = Ansi.GetFormattedEditor(configs.Editor, configs.NerdFont);
            string[] ConfigText = [
    $"Editor                 {Ansi.bold}[{Ansi.yellow}{e}{shortAnsi}]{Ansi.reset}     ",
    $"NerdFont                 {Ansi.bold}[{Ansi.yellow}{configs.NerdFont}{shortAnsi}]{Ansi.reset}   ",
    $"List Window Style        {Ansi.bold}[{Ansi.yellow}{configs.ListStyle}{shortAnsi}]{Ansi.reset}  ",
    $"Path Bar Style           {Ansi.bold}[{Ansi.yellow}{configs.PathStyle}{shortAnsi}]{Ansi.reset}  ",
    $"Float Window Style       {Ansi.bold}[{Ansi.yellow}{configs.HelpStyle}{shortAnsi}]{Ansi.reset}  ",
    $"Default Float Window     {Ansi.bold}[{Ansi.yellow}{defaultWindow}{shortAnsi}]{Ansi.reset}         ",
            ];

            DrawWindowText(ConfigText, truncate: false);
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
                    if (select == 0) // Editor select
                    {
                        int editorSelect = Array.IndexOf(Editor.Editors, configs.Editor);

                        if (editorSelect == Editor.Editors.Length - 1)
                            editorSelect = 0;
                        else
                            editorSelect++;
                        configs.Editor = Editor.Editors[editorSelect];
                    }
                    else if (select == 5) // Floatig window select
                    {
                        int windowSelect = (int)configs.DefaultWindow;
                        if (windowSelect == 3) // Maximum amount of windows pr now
                            configs.DefaultWindow = 0;
                        else
                            configs.DefaultWindow++;
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
                        int editorSelect = Array.IndexOf(Editor.Editors, configs.Editor);

                        if (editorSelect == 0)
                            editorSelect = Editor.Editors.Length - 1;
                        else
                            editorSelect--;
                        configs.Editor = Editor.Editors[editorSelect];
                    }
                    else if (select == 5) // Floatig window select
                    {
                        int windowSelect = (int)configs.DefaultWindow;
                        if (windowSelect == 0)
                            configs.DefaultWindow = (FloatWindowType)3;
                        else
                            configs.DefaultWindow--;
                    }
                    else
                    {
                        configurations[select - 1] = !configurations[select - 1];
                    }
                    break;
                case ConsoleKey.Enter:
                    isConfiging = false;
                    break;

            }
            configs.NerdFont = configurations[0];
            configs.ListStyle = configurations[1];
            configs.PathStyle = configurations[2];
            configs.HelpStyle = configurations[3];
        }

    }

    public void DrawInfo(ExplorerItem current)
    {
        DrawBorder();
        int textStartX = StartX + 2;
        int textStartY = StartY + 2;
        int maxLength = Width - 8;

        //Console.SetCursorPosition((StartX + Width) / 2, StartY);
        //Console.Write($"{Style.Header}[1] Info {Style.Reset}");

        string header = $"{Ansi.GetFormattedText(current, NerdFont, maxLength)}";

        if (current.Type == ExplorerType.FILE)
        {
            if (!Util.CheckFileAccess(current.Path))
            {
                if (NerdFont)
                    header = $"{Ansi.red}{Ansi.reset} {header}";
                else
                    header = $"\uD83D\uDD12 {header}";

            }
        }
        else if (current.Type == ExplorerType.DIRECTORY)
        {
            if (!Util.CheckWriteAccess(current.Path))
            {
                if (NerdFont)
                    header = $"{Ansi.red}{Ansi.reset} {header}";
                else
                    header = $"\uD83D\uDD12 {header}";
            }

        }

        Console.SetCursorPosition(textStartX + 3, textStartY);
        Console.Write(header);


        if (current.Type == ExplorerType.DIRECTORY)
        {
            try
            {
                DirectoryInfo dir = new(current.Path);
                Console.SetCursorPosition(textStartX, textStartY + 1);
                Console.Write($"{Style.InfoHL}Creation Time:{Style.Reset}        {Style.Text}{dir.CreationTime}{Style.Reset}");
                Console.SetCursorPosition(textStartX, textStartY + 2);
                Console.Write($"{Style.InfoHL}Last Access Time:{Style.Reset}     {Style.Text}{dir.LastAccessTime}{Style.Reset}");
                Console.SetCursorPosition(textStartX, textStartY + 3);
                Console.Write($"{Style.InfoHL}Sub Directories:{Style.Reset}      {Style.Text}{dir.GetDirectories().Length}{Style.Reset}");
                Console.SetCursorPosition(textStartX, textStartY + 4);
                Console.Write($"{Style.InfoHL}Number of Files:{Style.Reset}      {Style.Text}{dir.GetFiles().Length}{Style.Reset}");
                Console.SetCursorPosition(textStartX, textStartY + 5);
                Console.Write($"{Style.InfoHL}Path:{Style.Reset}      {Style.Text}{Ansi.TruncateString(dir.FullName,Width - 13)}{Style.Reset}");

            }
            catch { }
        }
        else if (current.Type == ExplorerType.FILE)
        {
            try
            {
                FileInfo fil = new(current.Path);
                Console.SetCursorPosition(textStartX, textStartY + 1);
                string size = string.Empty;
                if (fil.Length > 1024)
                    size = $"{((double)fil.Length / 1024.0)} KB";
                else
                    size = $"{fil.Length} bytes";
                Console.Write($"{Style.InfoHL}Size:{Style.Reset}                {Style.Text}{size}{Style.Reset}");
                Console.SetCursorPosition(textStartX, textStartY + 2);
                Console.Write($"{Style.InfoHL}Creation Time:{Style.Reset}       {Style.Text}{fil.CreationTime}{Style.Reset}");
                Console.SetCursorPosition(textStartX, textStartY + 3);
                Console.Write($"{Style.InfoHL}Last Write Time:{Style.Reset}     {Style.Text}{fil.LastWriteTime}{Style.Reset}");
                Console.SetCursorPosition(textStartX, textStartY + 4);
                Console.Write($"{Style.InfoHL}Extension:{Style.Reset}           {Style.Text}{fil.Extension}{Style.Reset}");

                Console.SetCursorPosition(textStartX, textStartY + 5);
                Console.Write($"{Style.InfoHL}Path:{Style.Reset}      {Style.Text}{Ansi.TruncateString(fil.FullName, Width - 13)}{Style.Reset}");

            }
            catch { }
        }

    }

    public void DrawPreviewFile(ExplorerItem file)
    {
        int x = StartX + 2;
        int y = StartY + 2;
        DrawBorder();
        string err = string.Empty;
        if (!File.Exists(file.Path))
        {
            Console.SetCursorPosition(x, y);
            Console.Write("File dont excists error");
            return;
        }
        try
        {

            if (ExplorerItem.IsBinaryFile(file.Path, 500, ref err))
            {
                Console.SetCursorPosition(StartX + 5, StartY + 2);
                Console.Write("--- Binary file ---");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"Error: {Ansi.TruncateString(ex.Message, Width - 10)}");

        }


        try
        {
            string header = string.Empty;
            string[] previewText = File.ReadLines(file.Path)
                            .Take(Height - 2).ToArray();
            if (previewText.Length == 0)
            {
                Console.SetCursorPosition(StartX + 5, StartY + 2);
                Console.Write(" --- Empty File ---");
                return;
            }
            DrawWindowText(previewText, truncate: true);

        }
        catch (FieldAccessException ex)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"FieldAccessException: {Ansi.TruncateString(ex.Message, 10)}");
        }
        catch (Exception ex)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"Error: {Ansi.TruncateString(ex.Message, 15)}");

        }
    }
    public void PreviewDirectory(ExplorerItem dir)
    {
        int x = StartX + 2;
        int y = StartY + 2;
        DrawBorder();
        string err = string.Empty;
        if (!Directory.Exists(dir.Path))
        {
            Console.SetCursorPosition(x, y);
            Console.Write("Directory dont excists error");
            return;
        }

        try
        {
            List<ExplorerItem> list = ExplorerItem.GetDirItems(dir.Path, ref err);
            if (list.Count == 0)
            {
                Console.SetCursorPosition(StartX + 5, StartY + 2);
                Console.Write(" --- Empty Directory ---");
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (i >= Height - 2)
                    break;
                Console.SetCursorPosition(x, y + i);
                if (Style.Active)
                    Console.Write(Ansi.GetFormattedText(list[i], NerdFont, Width - 5));
                else
                    Console.Write(Ansi.TruncateString(list[i].DisplayName, Width - 5));
            }
        }
        catch (Exception) { }
    }

    public void DrawWindowPanes(string windowPanes)
    {
        Console.SetCursorPosition(StartX + 5, StartY);
        Console.Write($"{Style.Border}{windowPanes}{Style.Reset}");
    }
}
