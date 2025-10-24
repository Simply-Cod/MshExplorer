
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

                directoryItems = ExplorerItem.GetDirItems(currentPath);

                pages = (directoryItems.Count + rows - 1) / rows;

                int startIndex = currentPage * rows;
                int take = Math.Max(0, Math.Min(rows, directoryItems.Count - startIndex));

                if (take > 0)
                    subPage = directoryItems.GetRange(startIndex, take);
                else
                    subPage = new List<ExplorerItem>();

                Console.Clear();
                ExplorerDraw.Header(currentPath);
                ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage, clipboardItem);
                if (showSideWindow)
                {
                    ExplorerDraw.HelpWindow();
                }
            }

            if (dirChange || pageChange)
                ExplorerDraw.InitItemList(showSideWindow, rows, columns, itemStart, leftPaneWidth, subPage);

            // Update
            // -------------------------------------------------------
            if (subPage.Count != 0)
            {
                if (dirChange || pageChange)
                {
                    ExplorerDraw.CurrentItemAfterInit(showSideWindow, index, previousIndex, itemStart,
                     leftPaneWidth, columns, subPage[previousIndex]);

                    dirChange = false;
                    pageChange = false;
                }
                else
                {
                    ExplorerDraw.CurrentItem(showSideWindow, previousIndex, itemStart, leftPaneWidth,
                            columns, index, subPage[previousIndex], subPage[index]);

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
                    if (parent != null && Util.IsReadableDir(parent.FullName))
                    {
                        currentPath = parent.FullName;
                        dirChange = true;
                    }
                    break;
                case 'l':
                    if (subPage.Count >= 1 && subPage[index].Type == ExplorerType.DIRECTORY)
                    {
                        var next = subPage[index].Path;
                        if (Directory.Exists(next) && Util.IsReadableDir(next))
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
                        Util.AddItem(currentPath, newItem);
                        dirChange = true;
                    }
                    break;
                case 'd':
                    if (subPage.Count >= 1)
                    {
                        var itemToRemove = subPage[index].Path;
                        if (Directory.Exists(itemToRemove) && Util.IsReadableDir(itemToRemove))
                        {
                            Util.RemoveItem(subPage[index]);

                            dirChange = true;
                        }
                        else if (File.Exists(itemToRemove))
                        {
                            Util.RemoveItem(subPage[index]);

                            dirChange = true;
                        }
                    }
                    break;
                case 'y':
                    if (subPage[index].Type == ExplorerType.FILE)
                    {
                        var src = subPage[index];
                        clipboardItem = new(src.DisplayName, src.Path, src.Type);
                        ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage, clipboardItem);
                    }
                    break;
                case 'p':
                    if (clipboardItem.Type == ExplorerType.FILE)
                    {
                        Util.PasteItem(clipboardItem, currentPath);
                        clipboardItem = new(string.Empty, string.Empty, ExplorerType.NONE);
                        ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage, clipboardItem);
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
                        if (Directory.Exists(next) && Util.IsReadableDir(next))
                        {
                            currentPath = next;
                            dirChange = true;
                        }
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    var parent = Directory.GetParent(currentPath);
                    if (parent != null && Util.IsReadableDir(parent.FullName))
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
                            ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage, clipboardItem);
                            pageChange = true;
                        }
                        else
                        {
                            // Todo: No Previous page signal
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
                            ExplorerDraw.StatusBar(itemStart, rows, pages, currentPage, clipboardItem);
                            pageChange = true;
                        }
                        else
                        {
                            // Todo: No next page signal
                        }
                    }
                    break;
            }

        }
        Console.Write(showCursor); // Show Cursor
        Console.Clear();
    }
}
