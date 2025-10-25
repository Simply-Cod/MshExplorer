
namespace MshExplorer;

class Program
{
    static void Main(string[] args)
    {
        const string hideCursor = "\x1b[?25l";
        const string showCursor = "\x1b[?25h";

        bool isRunning = true;
        bool updateFullWindow = true;
        bool dirChange = false;

        string currentPath = Directory.GetCurrentDirectory();
        string ExceptionMessage = string.Empty;
        string header = string.Empty;

        ConsoleKeyInfo key;
        Console.Write(hideCursor);
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CancelKeyPress += (s, e) =>
        {
            Console.Write(showCursor);
            Util.Clear();
        };
        int prevHeight = Console.WindowHeight;
        int prevWidth = Console.WindowWidth;


        List<ExplorerItem> directoryItems = [];
        directoryItems = ExplorerItem.GetDirItems(currentPath, ref ExceptionMessage);

        CommandLine commandLine = new();
        ListWindow listWin = new(2, 5, 40, prevHeight - 7);
        StatusBar statusBar = new();


        // Main Loop
        // ---------------------------------------------------------------------------
        while (isRunning)
        {
            bool displayHelpWindow = (Console.WindowHeight > 30) && (Console.WindowWidth > 100);

            if (Console.WindowHeight != prevHeight)
            {
                prevHeight = Console.WindowHeight;
                if (listWin.Resize(Console.WindowHeight, directoryItems))
                    updateFullWindow = true;
            }

            // Update
            // -------------------------------------------------------
            if (updateFullWindow)
            {
                Util.Clear();
                ExplorerDraw.Header(currentPath);
                listWin.DrawBorder();
                listWin.DrawListFull(directoryItems);
                statusBar.SelectedIndex = listWin.SelectedIndex + 1;
                statusBar.TotalItems = directoryItems.Count;
                statusBar.Draw();
                
                if (displayHelpWindow)
                    ExplorerDraw.HelpWindow();

                updateFullWindow = false;
            }


            if (dirChange)
            {
                Util.Clear();
                directoryItems.Clear();
                directoryItems = ExplorerItem.GetDirItems(currentPath, ref statusBar.ErrorMessage);
                listWin.TopIndex = 0;
                listWin.SelectedIndex = 0;
                ExplorerDraw.Header(currentPath);
                listWin.DrawBorder();
                listWin.DrawListFull(directoryItems);
                statusBar.SelectedIndex = listWin.SelectedIndex + 1;
                statusBar.TotalItems = directoryItems.Count;
                statusBar.Draw();

                if (displayHelpWindow)
                    ExplorerDraw.HelpWindow();

                dirChange = false;
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
                    listWin.MoveSelection(directoryItems, +1);
                    statusBar.SelectedIndex = listWin.SelectedIndex + 1;
                    statusBar.TotalItems = directoryItems.Count;
                    statusBar.Draw();
                    break;
                case 'k':
                    listWin.MoveSelection(directoryItems, -1);
                    statusBar.SelectedIndex = listWin.SelectedIndex + 1;
                    statusBar.TotalItems = directoryItems.Count;
                    statusBar.Draw();
                    break;
                case 'h':
                    string? tryGetPath = Path.GetDirectoryName(currentPath);
                    if (!string.IsNullOrEmpty(tryGetPath))
                        currentPath = tryGetPath;
                    dirChange = true;
                    break;
                case 'l':
                    if (directoryItems[listWin.SelectedIndex].Type == ExplorerType.DIRECTORY)
                        currentPath = directoryItems[listWin.SelectedIndex].Path;
                    dirChange = true;
                    break;
                case 'a':
                    commandLine.Header = "Add Item";
                    string itemName = commandLine.GetString();
                    if (!string.IsNullOrEmpty(itemName))
                    {
                        Util.AddItem(currentPath, itemName, ref statusBar.ErrorMessage);
                        dirChange = true;
                    }
                    break;
                case 'd':
                    Util.RemoveItem(directoryItems[listWin.SelectedIndex], ref statusBar.ErrorMessage);
                    break;
                case 'y':
                    statusBar.AddClipboardItem(directoryItems[listWin.SelectedIndex]); 
                    break;
                case 'p':
                    if (statusBar.ClipboardItem.Type == ExplorerType.FILE)
                    {
                        Util.PasteFile(statusBar.ClipboardItem, currentPath, ref statusBar.ErrorMessage);
                        statusBar.ClearClipboardItem(); 
                        dirChange = true;
                    }
                    else
                    {
                        Util.PasteDirectory(statusBar.ClipboardItem.Path, currentPath, ref statusBar.ErrorMessage);
                        statusBar.ClearClipboardItem();
                        dirChange = true;
                    }

                    break;
                case '/':

                    break;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    listWin.MoveSelection(directoryItems, -1);
                    statusBar.SelectedIndex = listWin.SelectedIndex + 1;
                    statusBar.TotalItems = directoryItems.Count;
                    statusBar.Draw();
                    break;
                case ConsoleKey.DownArrow:
                    listWin.MoveSelection(directoryItems, +1);
                    statusBar.SelectedIndex = listWin.SelectedIndex + 1;
                    statusBar.TotalItems = directoryItems.Count;
                    statusBar.Draw();
                    break;
                case ConsoleKey.RightArrow:
                    if (directoryItems[listWin.SelectedIndex].Type == ExplorerType.DIRECTORY)
                        currentPath = directoryItems[listWin.SelectedIndex].Path;
                    dirChange = true;

                    break;
                case ConsoleKey.LeftArrow:
                    string? tryGetPath = Path.GetDirectoryName(currentPath);
                    if (!string.IsNullOrEmpty(tryGetPath))
                        currentPath = tryGetPath;
                    dirChange = true;

                    break;


                case ConsoleKey.Backspace:
                    ExceptionMessage = string.Empty;
                    break;

            }

        }
        Console.Write(showCursor); // Show Cursor
        Console.Clear();
    }
}
