
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

        UserHandler userSettings = new();
        userSettings.ReadConfigs();

        CommandLine commandLine = new();
        ListWindow listWin = new(2, 5, 40, prevHeight - 7);
        StatusBar statusBar = new();
        statusBar.Editor = userSettings.GetEditorStyle();




        // Main Loop
        // ---------------------------------------------------------------------------
        while (isRunning)
        {
            bool displayHelpWindow = (Console.WindowHeight > 30) && (Console.WindowWidth > 100);

            if (Console.WindowHeight != prevHeight)
            {
                prevHeight = Console.WindowHeight;

                listWin.Height = (Console.WindowHeight - 7);
                if (listWin.Resize(listWin.Height, directoryItems))
                {
                    updateFullWindow = true;
                }
                dirChange = true;
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

            // Exceptions
            // ------------------------------------------------------
            // User setings



            if (!string.IsNullOrWhiteSpace(userSettings.ExceptionMessage))
            {
                statusBar.ErrorMessage = userSettings.ExceptionMessage;
                statusBar.Notify = true;
                statusBar.Draw();
                userSettings.ExceptionMessage = string.Empty;
            }

            // File Manipulation
            if (!string.IsNullOrWhiteSpace(ExceptionMessage))
            {
                statusBar.ErrorMessage = ExceptionMessage;
                statusBar.Notify = true;
                statusBar.Draw();
                ExceptionMessage = string.Empty;
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
                    if (directoryItems.Count > 0 && directoryItems[listWin.SelectedIndex].Type == ExplorerType.DIRECTORY)
                        currentPath = directoryItems[listWin.SelectedIndex].Path;
                    dirChange = true;
                    break;
                case 'a':
                    commandLine.Header = "Add Item";
                    commandLine.ToolTip = "End name with / to create a directory";
                    string itemName = commandLine.GetString();
                    if (!string.IsNullOrEmpty(itemName))
                    {
                        Util.AddItem(currentPath, itemName, ref ExceptionMessage);
                        dirChange = true;
                    }
                    commandLine.Header = string.Empty;
                    commandLine.ToolTip = string.Empty;
                    break;
                case 'd':
                    if (directoryItems.Count > 0)
                    {
                        Util.RemoveItem(directoryItems[listWin.SelectedIndex], ref ExceptionMessage);
                        dirChange = true;
                    }
                    break;
                case 'y':
                    if (directoryItems.Count > 0)
                        statusBar.AddClipboardItem(directoryItems[listWin.SelectedIndex]);
                    break;
                case 'p':
                    if (statusBar.ClipboardItem.Type == ExplorerType.FILE)
                    {
                        Util.PasteFile(statusBar.ClipboardItem, currentPath, ref ExceptionMessage);
                        statusBar.ClearClipboardItem();
                        dirChange = true;
                    }
                    else if (statusBar.ClipboardItem.Type == ExplorerType.DIRECTORY)
                    {
                        Util.PasteDirectory(statusBar.ClipboardItem.Path, currentPath, ref ExceptionMessage);
                        statusBar.ClearClipboardItem();
                        dirChange = true;
                    }

                    break;
                    // Command line input ------------------------------------------------------
                case ':':
                    // Todo: Add A Command Page and a tooltip letting users know about commands
                    string command = commandLine.GetString();
                    if (!string.IsNullOrEmpty(command))
                    {
                        CommandType type = 0;
                        string value;
                        (type, value) = CommandParser.Parse(command);

                        switch (type)
                        {
                            case CommandType.SET_EDITOR:
                                userSettings.SetEditor(value);
                                statusBar.Editor = userSettings.GetEditorStyle();
                                dirChange = true;
                                break;
                                
                            case CommandType.QUIT:
                                isRunning = false;
                                break;

                        }

                    }
                    // ------------------------------------------------------------------------

                    break;
                case 'e':
                    if (userSettings.Configs.Editor == string.Empty || userSettings.Configs.Editor == "null")
                    {
                        string editor = commandLine.GetString();
                        commandLine.Header = " Editor ";
                        Editor.OpenEditor(directoryItems[listWin.SelectedIndex], editor);
                    }
                    else
                    {
                        Editor.OpenEditor(directoryItems[listWin.SelectedIndex], userSettings.Configs.Editor);
                    }
                    dirChange = true;
                    break;
                case '/':
                    FileSearch.PatternMatch(currentPath, directoryItems, listWin, statusBar);
                    dirChange = true;
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
                    if (directoryItems.Count > 0 && directoryItems[listWin.SelectedIndex].Type == ExplorerType.DIRECTORY)
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
                    statusBar.ErrorMessage = string.Empty;
                    statusBar.Notify = false;
                    statusBar.Draw();
                    break;

            }

        }
        Console.Write(showCursor); // Show Cursor
        Console.Clear();
    }
}
