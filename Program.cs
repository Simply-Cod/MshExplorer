
namespace MshExplorer;

class Program
{
    static void Main(string[] args)
    {
        const string hideCursor = "\x1b[?25l";
        const string showCursor = "\x1b[?25h";

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.CancelKeyPress += (s, e) =>
        {
            Console.Write(showCursor);
            Console.Clear();
        };

        bool isRunning = true;
        string currentPath = Directory.GetCurrentDirectory();
        bool dirChange = true;
        List<ExplorerItem> directoryItems = [];
        List<ExplorerItem> subPage = [];

        ConsoleKeyInfo key;
        int index = 0;
        int previousIndex = 0;
        int itemStart = 4; // Test Change (from 2 to 4)

        string header = string.Empty;

        // Pages Testing
        int pages = 0;
        int currentPage = 0;
        bool pageChange = false;

        Console.Write(hideCursor);

        ExplorerItem clipboardItem = new(string.Empty, string.Empty, ExplorerType.NONE);

        int prevHeight = Console.WindowHeight;
        int prevWidth = Console.WindowWidth;

        string ExceptionMessage = string.Empty;
        bool notifyError = false;
        while (isRunning)
        {

            if (Console.WindowWidth != prevWidth || Console.WindowHeight != prevHeight)
            {
                prevHeight = Console.WindowHeight;
                prevWidth = Console.WindowWidth;
                dirChange = true;
            }

            int footerLine = 1;
            int rows = Math.Max(1, Console.WindowHeight - (itemStart + footerLine));
            int columns = Console.WindowWidth;
            bool showSideWindow = columns > 130;
            int leftPaneWidth = showSideWindow ? Math.Max(0, 70) : columns;

            if (dirChange)
            {
                index = 0;
                previousIndex = 0;
                directoryItems.Clear();
                subPage.Clear();
                currentPage = 0;

                directoryItems = ExplorerItem.GetDirItems(currentPath, ref ExceptionMessage);

                pages = (directoryItems.Count + rows - 1) / rows;

                int startIndex = currentPage * rows;
                int take = Math.Max(0, Math.Min(rows, directoryItems.Count - startIndex));

                if (take > 0)
                    subPage = directoryItems.GetRange(startIndex, take);
                else
                    subPage = new List<ExplorerItem>();

                Console.Clear();
                ExplorerDraw.Header(currentPath);
                ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage, clipboardItem, notifyError, ref ExceptionMessage);
                if (showSideWindow)
                {
                    ExplorerDraw.HelpWindow();
                }
            }

            if (dirChange || pageChange)
                ExplorerDraw.InitItemList(showSideWindow, rows, columns, itemStart, leftPaneWidth, subPage, ref ExceptionMessage);

            // Update
            // -------------------------------------------------------
            if (subPage.Count != 0)
            {
                if (dirChange || pageChange)
                {
                    ExplorerDraw.CurrentItemAfterInit(showSideWindow, index, previousIndex, itemStart,
                     leftPaneWidth, columns, subPage[previousIndex], ref ExceptionMessage);

                    dirChange = false;
                    pageChange = false;
                }
                else
                {
                    ExplorerDraw.CurrentItem(showSideWindow, previousIndex, itemStart, leftPaneWidth,
                            columns, index, subPage[previousIndex], subPage[index], ref ExceptionMessage);

                    previousIndex = index;
                }
            }

            // Input
            // --------------------------------------------------------
            key = Console.ReadKey(true);

            switch (key.KeyChar)
            {
                case 'q':
                    isRunning = false;
                    break;
                case 'j':
                    if (subPage.Count == 0) break;
                    if (index >= subPage.Count - 1)
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                    break;
                case 'k':
                    if (subPage.Count == 0) break;
                    if (index <= 0)
                    {
                        index = subPage.Count - 1;
                    }
                    else
                    {
                        index--;
                    }
                    break;
                case 'h':
                    var parent = Directory.GetParent(currentPath);
                    if (parent != null && Util.IsReadableDir(parent.FullName, ref ExceptionMessage))
                    {
                        currentPath = parent.FullName;
                        dirChange = true;
                    }
                    break;
                case 'l':
                    if (subPage.Count >= 1 && subPage[index].Type == ExplorerType.DIRECTORY)
                    {
                        var next = subPage[index].Path;
                        if (Directory.Exists(next) && Util.IsReadableDir(next, ref ExceptionMessage))
                        {
                            currentPath = next;
                            dirChange = true;
                        }
                    }
                    break;
                case 'a':
                    string newItem = Util.GetString();
                    if (!string.IsNullOrEmpty(newItem))
                    {
                        Util.AddItem(currentPath, newItem, ref ExceptionMessage);
                        dirChange = true;
                    }
                    break;
                case 'd':
                    if (subPage.Count >= 1)
                    {
                        var itemToRemove = subPage[index].Path;
                        if (Directory.Exists(itemToRemove) && Util.IsReadableDir(itemToRemove, ref ExceptionMessage))
                        {
                            Util.RemoveItem(subPage[index], ref ExceptionMessage);

                            dirChange = true;
                        }
                        else if (File.Exists(itemToRemove))
                        {
                            Util.RemoveItem(subPage[index], ref ExceptionMessage);

                            dirChange = true;
                        }
                    }
                    break;
                case 'y':
                    if (subPage[index].Type == ExplorerType.FILE)
                    {
                        var src = subPage[index];
                        clipboardItem = new(src.DisplayName, src.Path, src.Type);
                        ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage,
                                clipboardItem, notifyError, ref ExceptionMessage);
                    }
                    else if (subPage[index].Type == ExplorerType.DIRECTORY)
                    {
                        var src = subPage[index];
                        clipboardItem = new(src.DisplayName, src.Path, src.Type);
                        ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage,
                                clipboardItem, notifyError, ref ExceptionMessage);
                    }
                    break;
                case 'p':
                    if (clipboardItem.Type == ExplorerType.FILE)
                    {
                        Util.PasteFile(clipboardItem, currentPath, ref ExceptionMessage);
                        clipboardItem = new(string.Empty, string.Empty, ExplorerType.NONE);
                        ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage,
                                clipboardItem, notifyError, ref ExceptionMessage);
                        dirChange = true;
                    }
                    else if (clipboardItem.Type == ExplorerType.DIRECTORY)
                    {
                        Util.PasteDirectory(clipboardItem.Path, currentPath, ref ExceptionMessage);
                        clipboardItem = new(string.Empty, string.Empty, ExplorerType.NONE);
                        ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage,
                                clipboardItem, notifyError, ref ExceptionMessage);
                        dirChange = true;
                    }

                    break;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (subPage.Count == 0) break;
                    if (index <= 0)
                    {
                        index = subPage.Count - 1;
                    }
                    else
                    {
                        index--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (subPage.Count == 0) break;
                    if (index >= subPage.Count - 1)
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (subPage.Count >= 1 && subPage[index].Type == ExplorerType.DIRECTORY)
                    {
                        var next = subPage[index].Path;
                        if (Directory.Exists(next) && Util.IsReadableDir(next, ref ExceptionMessage))
                        {
                            currentPath = next;
                            dirChange = true;
                        }
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    var parent = Directory.GetParent(currentPath);
                    if (parent != null && Util.IsReadableDir(parent.FullName, ref ExceptionMessage))
                    {
                        currentPath = parent.FullName;
                        dirChange = true;
                    }
                    break;
                case ConsoleKey.P:
                    if (key.Modifiers == ConsoleModifiers.Control)
                    {
                        if (pages > 1)
                        {
                            currentPage = (currentPage - 1 + pages) % pages;
                            int startIndex = currentPage * rows;
                            int take = Math.Max(0, Math.Min(rows, directoryItems.Count - startIndex));

                            if (take > 0)
                            {
                                subPage = directoryItems.GetRange(startIndex, take);
                            }
                            else
                            {
                                subPage = new List<ExplorerItem>();
                            }
                            index = 0;
                            previousIndex = 0;
                            Console.Clear();
                            ExplorerDraw.Header(currentPath);
                            ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage,
                                    clipboardItem, notifyError, ref ExceptionMessage);
                            pageChange = true;
                        }
                        
                    }
                    break;
                case ConsoleKey.N:
                    if (key.Modifiers == ConsoleModifiers.Control)
                    {
                        if (pages > 1)
                        {
                            currentPage = (currentPage + 1) % pages;

                            int startIndex = currentPage * rows;
                            int take = Math.Max(0, Math.Min(rows, directoryItems.Count - startIndex));

                            if (take > 0)
                            {
                                subPage = directoryItems.GetRange(startIndex, take);
                            }
                            else
                            {
                                subPage = new List<ExplorerItem>();

                            }

                            index = 0;
                            previousIndex = 0;

                            Console.Clear();
                            ExplorerDraw.Header(currentPath);
                            ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage,
                                    clipboardItem, notifyError, ref ExceptionMessage);
                            pageChange = true;
                        }
                        
                    }
                    break;
                case ConsoleKey.Backspace:
                    notifyError = false;
                    ExceptionMessage = string.Empty;
                    ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage,
                                    clipboardItem, notifyError, ref ExceptionMessage);
                    break;

            }
            if (!string.IsNullOrEmpty(ExceptionMessage))
                notifyError = true;

        }
        Console.Write(showCursor); // Show Cursor
        Console.Clear();
    }
}
