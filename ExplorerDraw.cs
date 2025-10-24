
namespace MshExplorer;

// ╭─────╮
// │     │
// ╰─────╯

public class ExplorerDraw
{
    const string reset =        "\e[0m";
    const string deleteLine =   "\e[2K";
    const string hideCursor =   "\e[?25l";
    const string showCursor =   "\e[?25h";

    const string bold =         "\e[1m";
    const string orange =       "\e[38;2;224;122;95m";
    const string green =        "\e[38;2;129;178;154m";
    const string blue =         "\e[38;2;41;81;242m";
    const string darkBlue =     "\e[38;2;61;64;91m";
    const string white =        "\e[38;2;244;241;222m";
    const string khaki =        "\e[38;2;242;204;143m";
    const string mellow =       "\e[38;2;154;151;132m";
    const string red =          "\e[38;2;186;61;52m";

    const string bgDark =       "\e[48;2;31;43;61m";
    const string bgMellow =     "\e[48;2;154;151;132m";
    const string eraseLine =    "\e[2K";


    public static void Border(int startX, int startY, int length, int height)
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(startX, startY);

        for (int i = 0; i <= height; i++)
        {
            for (int j = 0; j <= length; j++)
            {
                if (i == 0)
                {
                    if (j == 0)
                        Console.Write($"{darkBlue}╭");
                    else if (j == length)
                        Console.Write("╮");
                    else
                        Console.Write("─");
                }
                else if (i == height)
                {
                    if (j == 0)
                        Console.Write("╰");
                    else if (j == length)
                        Console.Write($"╯{reset}");
                    else
                        Console.Write("─");
                }
                else
                {
                    if (j == 0)
                        Console.Write("│");
                    else if (j == length)
                        Console.Write("│");
                    else
                        Console.Write(" ");
                }
            }
            Console.SetCursorPosition(startX, startY + (i + 1));
        }
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public static void BorderText(int startX, int startY, string header, string[] text)
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        int headerOffset = 5;
        int textOffset = 2;
        Console.SetCursorPosition(startX + headerOffset, startY);
        Console.Write($" {header} ");
        startY++;
        Console.SetCursorPosition(startX + textOffset, startY);

        for (int i = 0; i < text.Length; i++)
        {
            Console.SetCursorPosition(startX + textOffset, startY + i);
            Console.Write(text[i]);
        }
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public static void StatusBar(int itemStart, int rows, int pages, int currentPage, ExplorerItem clipboardItem)
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        int infoY = Math.Min(Console.WindowHeight - 1, itemStart + rows);

        string clipboardText = string.Empty;
        if (clipboardItem.Type != ExplorerType.NONE)
            clipboardText = WriteDisplayText(clipboardItem, false);

        Console.SetCursorPosition(0, infoY);

        string statusText =
            $" Pages [{(pages == 0 ? 0 : currentPage + 1)}/{pages}] | Clipboard: {clipboardText}";

        Console.Write(eraseLine);
        Console.Write($"{bgDark}{bold}{green}{statusText}{bgDark}\x1b[K{reset}");

        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public static string Header(string path)
    {
        string[] splits = path.Split('/');
        string header = string.Join($" {bold}{orange}\x1b[0m {green}{bold}", splits);
        header = $"{header}{reset}";

        (int, int) cursorPos = Console.GetCursorPosition();

        Console.SetCursorPosition(0, 3);
        Console.Write(deleteLine);
        Console.Write("\e[0G"); // Move cursor to column 0
        Console.Write(header);
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);

        return header;
    }

    public static void HelpWindow()
    {
        ExplorerDraw.Border(70, 3, 42, 11);
        string helpHeader = $"{bold}{green}Quick Help{reset}";
        string[] helpWindowText = [
            $"{mellow}Navigate Up/Down            󰜷/󰁆 or k/j",
            "Move To Parent Directory    󰁎 or h",
            "Move To Sub Directory       󰜴 or l",
            "Change Page Forward         <Ctrl-n>",
            "Change Page Backwards       <Ctrl-p>",
            "Add File/Directory          a",
            "Delete File/Directory       d",
            "Copy/Yank File              y",
            "Paste File                  p",
            $"Quit                        q{reset}"
        ];
        ExplorerDraw.BorderText(70, 3, helpHeader, helpWindowText);
    }


    public static string WriteDisplayText(ExplorerItem item, bool isCurrentItem)
    {
        string displayName = string.Empty;
        
        if (item.Type == ExplorerType.DIRECTORY)
        {
            displayName = $"\x1b[38;5;105m{bold}  {item.DisplayName}\x1b[0m";
        }
        else
        {
            try
            {
                if (System.IO.Path.GetExtension(item.Path) == ".cs")
                    displayName = $"\x1b[38;5;11m  {item.DisplayName}{reset}";
                else if (System.IO.Path.GetExtension(item.Path) == ".c")
                    displayName = $"\x1b[38;5;208m  {item.DisplayName}{reset}";
                else if (ExplorerItem.IsBinaryFile(item.Path, 100))
                    displayName = $"\x1b[1;36m  {item.DisplayName}{reset}";
                else
                    displayName = $"\x1b[33m  {item.DisplayName}{reset}";
            }
            catch (UnauthorizedAccessException) {return $"{red}     {item.DisplayName}{reset}";}

        }

        if (isCurrentItem)
            displayName = $" {bold}{orange}{reset}  {displayName}";
        else
            displayName = $"   {displayName}";

        return displayName;
    }


    public static void InitItemList(bool showSideWindow, int rows,
                            int columns, int itemStart, int leftPaneWidth, List<ExplorerItem> subPage)
    {

        for (int i = 0; i < rows; i++)
        {
            Console.SetCursorPosition(0, itemStart + i);
            if (showSideWindow)
            {
                Console.Write(new string(' ', Math.Min(leftPaneWidth, Math.Max(0, columns))));
                Console.SetCursorPosition(0, itemStart + i);
            }
            else
            {
                Console.Write(deleteLine);

            }
            if (i < subPage.Count)
            {
                Console.Write(WriteDisplayText(subPage[i], false)); // False Means It is not selected
            }
        }

        if (showSideWindow)
        {
            ExplorerDraw.HelpWindow();
        }
        Console.SetCursorPosition(0, itemStart);

    }
    public static void CurrentItemAfterInit(bool showSideWindow, int index, int previousIndex, int itemStart,
                       int leftPaneWidth, int columns, ExplorerItem item)
    {
        index = 0;
        previousIndex = 0;

        Console.SetCursorPosition(0, previousIndex + itemStart);
        if (showSideWindow)
        {
            Console.Write(new string(' ', Math.Min(leftPaneWidth, Math.Max(0, columns))));
            Console.SetCursorPosition(0, previousIndex + itemStart);
        }
        else
        {
            Console.Write(deleteLine);
        }
        Console.Write(WriteDisplayText(item, true)); // bool means selected item
        
    }

    public static void CurrentItem(bool showSideWindow, int previousIndex, int itemStart,
            int leftPaneWidth, int columns, int index, ExplorerItem previousItem, ExplorerItem currentItem)
    {
        Console.SetCursorPosition(0, previousIndex + itemStart);
        if (showSideWindow)
        {
            Console.Write(new string(' ', Math.Min(leftPaneWidth, Math.Max(0, columns))));
            Console.SetCursorPosition(0, previousIndex + itemStart);
        }
        else
        {
            Console.Write(deleteLine);
        }
        Console.Write(WriteDisplayText(previousItem, false));

        Console.SetCursorPosition(0, index + itemStart);
        if (showSideWindow)
        {
            Console.Write(new string(' ', Math.Min(leftPaneWidth, Math.Max(0, columns))));
            Console.SetCursorPosition(0, index + itemStart);
        }
        else
        {
            Console.Write(deleteLine);
        }
        Console.Write(WriteDisplayText(currentItem, true));
    }

 
    public static void CommandLine()
    {
        Border(0,0, 48, 2);

        Console.SetCursorPosition(3, 0);
        Console.Write($" {green}{bold}Add Item{reset} ");

        Console.SetCursorPosition(50, 0);
        Console.Write($" {blue}{reset} End name with / to create a Directory");

        Console.SetCursorPosition(3, 2);
        Console.Write("Enter to Confirm ─ Esc to Cancel ");

        Console.SetCursorPosition(2, 1); // Input location
        Console.Write($"{bold}>{reset} ");
        Console.Write(showCursor);
    }

    public static void RemoveCommandLine((int, int) cursorPos)
    {
        Console.Write(deleteLine);
        Console.Write(deleteLine);Console.SetCursorPosition(0,0);
        Console.Write(deleteLine);Console.SetCursorPosition(0,2);
        Console.Write(deleteLine);

        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
        Console.Write(hideCursor);   
    }

}
