using System.Text;

namespace MshExplorer;

class FileSearch
{

    public static void PatternMatch(string currentPath, List<ExplorerItem> currentList, ListWindow listWin, StatusBar status)
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
            listWin.DrawListFull(currentList);
            status.TotalItems = currentList.Count;
            status.SelectedIndex = 0;
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
                    break;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                var fileQuery = currentList.Where(f => f.DisplayName.Contains(pattern.ToString(),
                                                StringComparison.OrdinalIgnoreCase));

                searchList = fileQuery.ToList();
                listWin.SelectedIndex = 0;
                listWin.TopIndex = 0;
                listWin.DrawListFull(searchList);
                status.TotalItems = searchList.Count;
                status.SelectedIndex = 0;
                status.Draw();

            }

        }
        catch (Exception)
        {

        }
        cli.RemoveCommandLine();
    }
}
