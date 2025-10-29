// unicode for File 🗎
// unicode for folder 🖿 🗁 🗀
// unicode for info 🛈
// ↵

namespace MshExplorer;

class Program
{
    static void Main(string[] args)
    {

        bool isRunning = true;
        bool updateFullWindow = true;
        bool dirChange = false;
        bool configChange = true;

        string currentPath = Directory.GetCurrentDirectory();
        string homePath = currentPath;
        string ExceptionMessage = string.Empty;
        string header = string.Empty;

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
        FloatingWindow floatingWin = new(width: 50, height: 15); // Width normal: 45
        StatusBar statusBar = new();

        FloatWindowType floatType = FloatWindowType.HELP;


        // Main Loop
        // ---------------------------------------------------------------------------
        while (isRunning)
        {

            if (Console.WindowHeight != prevHeight)
            {
                prevHeight = Console.WindowHeight;
                listWindow.SetHeight();
                updateFullWindow = true;
                dirChange = true;
                Util.Clear();
            }

            if (floatingWin.CheckWindowSize() && !floatingWin.HideWindow)
            {
                switch (floatType)
                {
                    case FloatWindowType.HELP:
                        if (dirChange || updateFullWindow)
                            floatingWin.DrawQuickHelp(TextStore.HelpHeader, TextStore.HelpWindowText);
                        break;
                    case FloatWindowType.INFO:
                        if (listWindow.Items.Count > 0)
                            floatingWin.DrawInfo(listWindow.Items[listWindow.SelectedIndex]);
                        break;
                }

            }

            if (configChange)
            {
                pathBar.UpdateConfigs(userSettings.Configs);
                listWindow.UpdateConfigs(userSettings.Configs);
                floatingWin.UpdateConfigs(userSettings.Configs);
                statusBar.UpdateConfigs(userSettings.Configs);
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
                listWindow.SetItems(directoryItems);
                listWindow.DrawBorder();
                listWindow.DrawList();
                statusBar.SetIndexAndCount(listWindow.SelectedIndex, listWindow.Items.Count);
                statusBar.Draw();


                if (floatingWin.CheckWindowSize() && !floatingWin.HideWindow && floatType == FloatWindowType.INFO)
                {
                    if (listWindow.Items.Count > 0)
                        floatingWin.DrawInfo(listWindow.Items[listWindow.SelectedIndex]);
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
                    if (listWindow.Items[listWindow.SelectedIndex].Type == ExplorerType.DIRECTORY)
                    {
                        currentPath = listWindow.Items[listWindow.SelectedIndex].Path;
                        dirChange = true;
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
                case ConsoleKey.D:
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
                case ConsoleKey.Y:
                    if (pathBar.WriteAccess)
                    {
                        statusBar.ClipboardItem = listWindow.Items[listWindow.SelectedIndex];
                        statusBar.Draw();
                    }
                    else
                    {
                        ExceptionMessage = "You do not have access to files in this directory";
                    }
                    break;
                case ConsoleKey.P:
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

                case ConsoleKey.E:
                    if (userSettings.Configs.Editor == string.Empty || userSettings.Configs.Editor == "null")
                    {
                        string editor = commandLine.GetString();
                        commandLine.Header = " Editor ";
                        Editor.OpenEditor(listWindow.Items[listWindow.SelectedIndex], editor);
                    }
                    else
                    {
                        Editor.OpenEditor(listWindow.Items[listWindow.SelectedIndex], userSettings.Configs.Editor);
                    }
                    dirChange = true;
                    break;
                case ConsoleKey.Divide:
                    bool success = FileSearch.PatternMatch(currentPath, listWindow.Items, listWindow, statusBar);
                    if (!success)
                        dirChange = true;
                    break;

                case ConsoleKey.Backspace:
                    statusBar.ErrorMessage = string.Empty;
                    statusBar.Notify = false;
                    statusBar.Draw();
                    break;
                default:

                    switch (key.KeyChar)
                    {
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
                                        currentPath = homePath;
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
                            floatingWin.HideWindow = !floatingWin.HideWindow;
                            if (floatingWin.HideWindow)
                                floatingWin.ClearWindow();
                            else
                                floatingWin.DrawQuickHelp(TextStore.HelpHeader, TextStore.HelpWindowText);

                            break;

                        case '!':
                            if (floatType == FloatWindowType.HELP)
                                floatType = FloatWindowType.INFO;
                            else
                                floatType = FloatWindowType.HELP;
                            updateFullWindow = true;
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
