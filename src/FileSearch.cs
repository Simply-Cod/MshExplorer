using System.Text;

namespace MshExplorer;

class FileSearch
{

    public static bool PatternMatch(string currentPath, List<ExplorerItem> currentList,
            ListWindow listWin, StatusBar status, List<ExplorerItem> markedList)
    {
        CommandLine cli = new();
        StringBuilder pattern = new();

        ConsoleKeyInfo key;

        int maxLength = 40;

        try
        {
            listWin.SelectedIndex = 0;
            listWin.TopIndex = 0;
            listWin.DrawList();
            status.SetIndexAndCount(listWin.SelectedIndex, listWin.Items.Count);
            status.Draw();
            cli.DrawCommandLine();

            int x = 4; // cursor x position
            int y = 1; // cursor y position
            while (true)
            {
                Console.SetCursorPosition(x, y);
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && pattern.Length > 0)
                {
                    pattern.Remove(pattern.Length - 1, 1);
                    x--;
                    Console.Write("\b \b");
                }
                else if (pattern.Length < maxLength && !char.IsControl(key.KeyChar))
                {
                    pattern.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                    x++;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    listWin.SetItems(currentList, markedList);
                    listWin.SelectedIndex = 0;
                    listWin.TopIndex = 0;
                    listWin.DrawList();
                    status.SetIndexAndCount(listWin.SelectedIndex, listWin.Items.Count);
                    status.Draw();

                    cli.RemoveCommandLine();
                    return false;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    cli.RemoveCommandLine();
                    return true;
                }
                List<ExplorerItem> view = pattern.Length == 0
                    ? currentList
                    : currentList.Where(f => f.DisplayName.Contains(
                        pattern.ToString(), StringComparison.OrdinalIgnoreCase
                    )).ToList();
                
                
                listWin.SelectedIndex = 0;
                listWin.TopIndex = 0;
                listWin.SetItems(view, markedList);
                listWin.DrawList();
                status.SetIndexAndCount(listWin.SelectedIndex, listWin.Items.Count);
                status.Draw();
            }

        }
        catch (Exception)
        {
            cli.RemoveCommandLine();
            return false;
        }
    }


    public static async Task PatternMatchAllAsync(string currentPath, ListWindow listWin, StatusBar status, List<ExplorerItem> markedList)
    {
        var fullList = await GetAllFiles(currentPath);

        CommandLine cli = new();
        cli.ToolTip = "Start with '!' for case sensitive and '*' for extension search";
        StringBuilder pattern = new();
        ConsoleKeyInfo key;

        int maxLength = 40;
    

        try
        {
            listWin.SelectedIndex = 0;
            listWin.TopIndex = 0;
            listWin.Items.Clear();
            PathBar.DrawRecursiveSearchBar(currentPath);
            listWin.DrawList();
            status.SetIndexAndCount(0, fullList.Count);
            status.Draw();
            cli.DrawCommandLine();

            int x = 4; // cursor x position
            int y = 1; // cursor y position

            while (true)
            {
                Console.SetCursorPosition(x, y);
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && pattern.Length > 0)
                {
                    pattern.Remove(pattern.Length - 1, 1);
                    x--;
                    Console.Write("\b \b");
                }
                else if (pattern.Length < maxLength && !char.IsControl(key.KeyChar))
                {
                    pattern.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                    x++;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    cli.RemoveCommandLine();
                    return;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    cli.RemoveCommandLine();
                    return;
                }

                var (filter, currentPattern) = FilterSearch(pattern);

                List<ExplorerItem> filtered;
                if (pattern.Length == 0)
                {
                    filtered = fullList;
                }
                else
                {
                    

                    switch (filter)
                    {
                        case 1: // Search by extension
                            var pat = currentPattern.Trim();
                            if (string.IsNullOrWhiteSpace(pat))
                            {
                                filtered = fullList;
                                break;
                            }
                            if (pat.StartsWith('.')) pat = pat[1..];

                            filtered = fullList.Where(f =>
                            {
                                var name = f.DisplayName;
                                var ext = Path.GetExtension(name);
                                if (!string.IsNullOrEmpty(ext) && ext[0] == '.') ext = ext[1..];

                                return ext.Equals(pat, StringComparison.OrdinalIgnoreCase) ||
                                       name.EndsWith("." + pat, StringComparison.OrdinalIgnoreCase);
                            }).ToList();
                            break;

                        case 2: // Case sensetive
                            filtered = fullList.Where(f =>
                                f.DisplayName.Contains(currentPattern, StringComparison.Ordinal)).ToList();
                            break;

                        default: // No Filter
                            filtered = fullList.Where(f =>
                                f.DisplayName.Contains(currentPattern, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                    }
                }

                listWin.SelectedIndex = 0;
                listWin.TopIndex = 0;
                listWin.SetItems(filtered, markedList);
                listWin.DrawList();
                status.SetIndexAndCount(listWin.SelectedIndex, listWin.Items.Count);
                status.Draw();

            }

        }
        catch (Exception)
        {
            cli.RemoveCommandLine();
        }

    }

    private static async Task<List<ExplorerItem>> GetAllFiles(string currentPath)
    {
        return await Task.Run(() =>
        {
            List<ExplorerItem> files = new(1024);
            try
            {
                var stack = new Stack<string>();
                stack.Push(currentPath);

                while (stack.Count > 0)
                {
                    var dir = stack.Pop();
                    try
                    {
                        foreach (var subDir in Directory.EnumerateDirectories(dir))
                        {
                            stack.Push(subDir);
                        }

                        foreach (var fil in Directory.EnumerateFiles(dir))
                        {
                            FileInfo temp = new(fil);
                            files.Add(new ExplorerItem(temp.Name, temp.FullName, ExplorerType.FILE));
                        }
                    }
                    catch (DirectoryNotFoundException) { }
                    catch (UnauthorizedAccessException) { }
                    catch (PathTooLongException) { }
                }
            }
            catch (Exception) { }
            return files;
        });
    }


    public static async Task SpinnerWithText(string message, CancellationToken token)
    {
        var sequence = new[] { '|', '/', '-', '\\' };
        int counter = 0;

        while (!token.IsCancellationRequested)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"{sequence[counter % sequence.Length]} {message}");
            counter++;
            await Task.Delay(100, token);
        }

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', message.Length + 2));
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    public static (int filter, string currentPattern) FilterSearch(StringBuilder sb)
    {
        if (sb.Length == 0) return (0, string.Empty);

        var s = sb.ToString();
        return s[0] switch
        {
            '*' => (1, s[1..]),
            '!' => (2, s[1..]),
            _ => (0, s)
        };
    }

}
