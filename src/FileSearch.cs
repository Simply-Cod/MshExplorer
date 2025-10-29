using System.Text;

namespace MshExplorer;

class FileSearch
{

    public static bool PatternMatch(string currentPath, List<ExplorerItem> currentList, ListWindow listWin, StatusBar status)
    {
        CommandLine cli = new();
        StringBuilder pattern = new();
        string compare = string.Empty;
        List<ExplorerItem> searchList = new();

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
            while (true)
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && pattern.Length > 0)
                {
                    pattern.Remove(pattern.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (pattern.Length < maxLength && !char.IsControl(key.KeyChar))
                {
                    pattern.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    cli.RemoveCommandLine();
                    return false;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    cli.RemoveCommandLine();
                    return true;
                }
                var fileQuery = currentList.Where(f => f.DisplayName.Contains(pattern.ToString(),
                                                StringComparison.OrdinalIgnoreCase));
                searchList = fileQuery.ToList();
                listWin.SelectedIndex = 0;
                listWin.TopIndex = 0;
                listWin.SetItems(searchList);
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
}
