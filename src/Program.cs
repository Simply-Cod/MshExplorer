// unicode for File 🗎
// unicode for folder 🖿 🗁 🗀
// unicode for info 🛈
// ↵


namespace MshExplorer;

class Program
{
    static async Task Main(string[] args)
    {

        bool isRunning = true;
        bool updateFullWindow = true;
        bool dirChange = false;
        bool configChange = true;

        string currentPath = Directory.GetCurrentDirectory();
        string ExceptionMessage = string.Empty;

        ConsoleKeyInfo key;
        Console.Write(Ansi.hideCursor);
        Console.Write(Ansi.enableAltBuffer); // Swapping to alternative buffer (might help the gnome terminal)
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Msh Explorer";
        Console.CancelKeyPress += (s, e) =>
        {
            Console.Write(Ansi.showCursor);
            Console.Write(Ansi.disableAltBuffer);
            Util.Clear();
        };
        int prevHeight = Console.WindowHeight;
        int prevWidth = Console.WindowWidth;


        List<ExplorerItem> directoryItems = [];
        directoryItems = ExplorerItem.GetDirItems(currentPath, ref ExceptionMessage);

        UserHandler userSettings = new();
        userSettings.ReadConfigs();

        PathBar pathBar = new();
        CommandLine commandLine = new();
        ListWindow listWindow = new(width: 40, directoryItems);
        FloatingWindow floatingWin = new();
        StatusBar statusBar = new();
        FloatWindowType floatType = FloatWindowType.HELP;

        MarkWindow markWindow = new();

        Bookmarks bookmarks = new();
        bookmarks.Items = BookmarkLogic.LoadBookmarks(bookmarks.Items);



        // Main Loop
        // ---------------------------------------------------------------------------
        while (isRunning)
        {
            listWindow.ReCheckMarks(currentPath, markWindow.MarkedList);

            if (Console.WindowHeight != prevHeight || prevWidth != Console.WindowWidth)
            {
                prevHeight = Console.WindowHeight;
                prevWidth = Console.WindowWidth;
                listWindow.SetHeight();
                markWindow.SetSize();
                updateFullWindow = true;
                dirChange = true;
                Util.Clear();
            }

            if (floatingWin.CheckWindowSize() && !floatingWin.HideWindow && !dirChange)
            {
                switch (floatType)
                {
                    case FloatWindowType.HELP:
                        if (dirChange || updateFullWindow)
                        {
                            floatingWin.DrawQuickHelp(TextStore.HelpHeader, TextStore.HelpWindowText);
                            floatingWin.DrawWindowPanes(TextStore.Windows[0]);
                        }
                        break;
                    case FloatWindowType.INFO:
                        if (listWindow.Items.Count > 0)
                        {
                            floatingWin.DrawInfo(listWindow.Items[listWindow.SelectedIndex]);
                        }
                        floatingWin.DrawWindowPanes(TextStore.Windows[1]);
                        break;
                    case FloatWindowType.MARK:
                        markWindow.DrawMarks();
                        floatingWin.DrawWindowPanes(TextStore.Windows[3]);
                        break;
                    case FloatWindowType.PREVIEW:
                        if (listWindow.Items.Count > 0)
                        {
                            if (listWindow.Items[listWindow.SelectedIndex].Type == ExplorerType.FILE)
                            {
                                floatingWin.DrawPreviewFile(listWindow.Items[listWindow.SelectedIndex]);
                            }
                            else if (listWindow.Items[listWindow.SelectedIndex].Type == ExplorerType.DIRECTORY)
                            {
                                floatingWin.PreviewDirectory(listWindow.Items[listWindow.SelectedIndex]);
                            }
                        }
                        floatingWin.DrawWindowPanes(TextStore.Windows[2]);
                        break;
                }
            }

            if (configChange)
            {
                pathBar.UpdateConfigs(userSettings.Configs);
                listWindow.UpdateConfigs(userSettings.Configs);
                listWindow.SetItems(directoryItems, markWindow.MarkedList);
                floatingWin.UpdateConfigs(userSettings.Configs);
                statusBar.UpdateConfigs(userSettings.Configs);
                markWindow.SetStyle(floatingWin.Style);
                configChange = false;
            }

            // Update
            // -------------------------------------------------------
            if (updateFullWindow)
            {
                pathBar.Draw(currentPath);
                listWindow.DrawBorder();
                listWindow.DrawList();
                statusBar.SetIndexAndCount(listWindow.SelectedIndex, listWindow.Items.Count);
                statusBar.Draw();

                updateFullWindow = false;
            }


            if (dirChange)
            {
                directoryItems.Clear();
                directoryItems = ExplorerItem.GetDirItems(currentPath, ref statusBar.ErrorMessage);
                pathBar.Draw(currentPath);
                listWindow.SetItems(directoryItems, markWindow.MarkedList);
                listWindow.DrawBorder();
                listWindow.DrawList();
                statusBar.SetIndexAndCount(listWindow.SelectedIndex, listWindow.Items.Count);
                statusBar.Draw();


                if (floatingWin.CheckWindowSize() && !floatingWin.HideWindow)
                {
                    switch (floatType)
                    {
                        case FloatWindowType.INFO:
                            if (listWindow.Items.Count > 0)
                            {
                                floatingWin.DrawInfo(listWindow.Items[listWindow.SelectedIndex]);
                            }
                            floatingWin.DrawWindowPanes(TextStore.Windows[1]);
                            break;
                        case FloatWindowType.MARK:
                            markWindow.DrawMarks();
                            floatingWin.DrawWindowPanes(TextStore.Windows[3]);
                            break;
                        case FloatWindowType.PREVIEW:
                            if (listWindow.Items.Count > 0)
                            {
                                if (listWindow.Items[listWindow.SelectedIndex].Type == ExplorerType.FILE)
                                {
                                    floatingWin.DrawPreviewFile(listWindow.Items[listWindow.SelectedIndex]);
                                }
                                else if (listWindow.Items[listWindow.SelectedIndex].Type == ExplorerType.DIRECTORY)
                                {
                                    floatingWin.PreviewDirectory(listWindow.Items[listWindow.SelectedIndex]);
                                }
                            }
                            floatingWin.DrawWindowPanes(TextStore.Windows[2]);
                            break;
                    }
                }
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

            while (Console.KeyAvailable) // Empty out input buffer
            {
                Console.ReadKey(true);
            }

            switch (key.Key)
            {
                case ConsoleKey.Q:
                    isRunning = false;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.J:
                    listWindow.ScrollDown();
                    statusBar.SetIndexAndCount(listWindow.SelectedIndex, listWindow.Items.Count);
                    statusBar.Draw();
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.K:
                    listWindow.ScrollUp();
                    statusBar.SetIndexAndCount(listWindow.SelectedIndex, listWindow.Items.Count);
                    statusBar.Draw();
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.H:
                    string? tryGetPath = Path.GetDirectoryName(currentPath);
                    if (!string.IsNullOrEmpty(tryGetPath))
                        currentPath = tryGetPath;
                    dirChange = true;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.L:
                    if (listWindow.Items.Count > 0)
                    {
                        if (listWindow.Items[listWindow.SelectedIndex].Type == ExplorerType.DIRECTORY)
                        {
                            currentPath = listWindow.Items[listWindow.SelectedIndex].Path;
                            dirChange = true;
                        }
                        else if (listWindow.Items[listWindow.SelectedIndex].Type == ExplorerType.FILE)
                        {
                            floatType = FloatWindowType.PREVIEW;
                            updateFullWindow = true;
                        }
                    }
                    break;
                case ConsoleKey.A:
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
                case ConsoleKey.E:
                    if (userSettings.Configs.Editor == string.Empty || userSettings.Configs.Editor == "null")
                    {
                        commandLine.Header = " Editor ";
                        commandLine.ToolTip = string.Empty;
                        string editor = commandLine.GetString();
                        Editor.OpenEditor(listWindow.Items[listWindow.SelectedIndex], editor);
                        commandLine.Header = string.Empty;
                        commandLine.ToolTip = string.Empty;
                    }
                    else
                    {
                        Editor.OpenEditor(listWindow.Items[listWindow.SelectedIndex], userSettings.Configs.Editor);
                    }
                    dirChange = true;
                    break;
                case ConsoleKey.Divide:
                    bool success = FileSearch.PatternMatch(currentPath, listWindow.Items, listWindow, statusBar, markWindow.MarkedList);
                    if (!success)
                        dirChange = true;
                    break;

                case ConsoleKey.B:
                    string tempPath = currentPath;
                    BookmarkLogic.BookmarkMode(bookmarks.Items, ref currentPath);
                    if (tempPath != currentPath)
                        dirChange = true;
                    else
                        updateFullWindow = true;
                        break;

                case ConsoleKey.Backspace:
                    statusBar.ErrorMessage = string.Empty;
                    statusBar.Notify = false;
                    statusBar.Draw();
                    break;
                case ConsoleKey.M:
                if (floatingWin.CheckWindowSize() && !floatingWin.HideWindow)
                {    
                    markWindow.DrawMarks();
                    floatingWin.DrawWindowPanes(TextStore.Windows[3]);
                }
                    if (listWindow.Items.Count > 0)
                        MarkLogic.MarkMode(markWindow, listWindow.Items[listWindow.SelectedIndex], ref currentPath, ref dirChange, listWindow.Items);
                    else
                        MarkLogic.MarkMode(markWindow, new(string.Empty, string.Empty, ExplorerType.NONE), ref currentPath, ref dirChange, listWindow.Items);

                    updateFullWindow = true;
                    break;
                case ConsoleKey.D1:
                    floatType = FloatWindowType.INFO;
                    updateFullWindow = true;
                    break;
                case ConsoleKey.D2:
                    floatType = FloatWindowType.PREVIEW;
                    updateFullWindow = true;
                    break;
                case ConsoleKey.D3:
                    floatType = FloatWindowType.MARK;
                    updateFullWindow = true;
                    break;
                default:

                    switch (key.KeyChar)
                    {
                        case '/':
                            {
                                bool ok = FileSearch.PatternMatch(currentPath, listWindow.Items,
                                        listWindow, statusBar, markWindow.MarkedList);
                                if (!ok)
                                    dirChange = true;
                                break;
                            }
                        case 'F':
                            await FileSearch.PatternMatchAllAsync(currentPath, listWindow, statusBar, markWindow.MarkedList);
                            break;

                        // Command line input ------------------------------------------------------
                        case ':':
                            if (!floatingWin.HideWindow && floatingWin.ScreenSizeBigEnough)
                                floatingWin.DrawQuickHelp(TextStore.CommandHeader, TextStore.CommandText);

                            string command = commandLine.GetString();
                            if (!string.IsNullOrEmpty(command))
                            {
                                CommandType type = 0;
                                string value;
                                (type, value) = CommandParser.Parse(command);

                                switch (type)
                                {
                                    case CommandType.QUIT:
                                        isRunning = false;
                                        break;

                                    case CommandType.HOME:
                                        if (!string.IsNullOrWhiteSpace(userSettings.Configs.HomePath))
                                            currentPath = userSettings.Configs.HomePath;

                                        dirChange = true;
                                        break;

                                    case CommandType.CONFIG:
                                        floatingWin.ConfigWindow(userSettings.Configs);
                                        listWindow.Style.Active = userSettings.Configs.ListStyle;
                                        pathBar.Style.Active = userSettings.Configs.PathStyle;
                                        floatingWin.Style.Active = userSettings.Configs.HelpStyle;
                                        updateFullWindow = true;
                                        configChange = true;
                                        break;
                                    case CommandType.SET_HOME:
                                        userSettings.Configs.HomePath = currentPath;
                                        userSettings.WriteConfigs();
                                        break;
                                    case CommandType.NONE:
                                        updateFullWindow = true;
                                        break;


                                }

                                if (configChange)
                                    userSettings.Update(listWindow.Style, pathBar.Style, floatingWin.Style);
                            }
                            else
                            {
                                updateFullWindow = true;
                            }
                            break;
                        // ------------------------------------------------------------------------

                        case '?':
                            floatType = FloatWindowType.HELP;
                            updateFullWindow = true;
                            break;
                        case 'D':
                            if (directoryItems.Count > 0)
                            {
                                if (pathBar.WriteAccess)
                                {
                                    Util.RemoveItem(listWindow.Items[listWindow.SelectedIndex],
                                            ref ExceptionMessage, userSettings.Configs.NerdFont);
                                    dirChange = true;
                                }
                                else
                                {
                                    ExceptionMessage = "You do not have access to files in this directory.";
                                }
                            }
                            break;
                    }
                    break;

            } // Switch case closing bracket

        } // Main loop closing bracket

        Console.Write(Ansi.showCursor); // Show Cursor
        Console.Write(Ansi.disableAltBuffer); // Deactivate alternative screen buffer
        Console.Clear();
    }
}
