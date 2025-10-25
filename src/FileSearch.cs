using System.Text;

namespace MshExplorer;

public class FileSearch
{

    public static void PatternMatch(string currentPath, List<ExplorerItem> currentList)
    {
        int topBuffer = 10;
        int maxListLength = Console.WindowHeight - topBuffer;
        int count = 0;
        List<ExplorerItem> searchList = new();

        (int, int) cursorPos = Console.GetCursorPosition();
        ExplorerDraw.CommandLine("Search", string.Empty);
        StringBuilder inputBuilder = new();
        string comp = string.Empty;
        ConsoleKeyInfo key;
        int maxLength = 40;
        try
        {
            (int, int) tempCursorPos = Console.GetCursorPosition();
            Console.SetCursorPosition(0, 5);
            Console.Write("\e[0J");
            foreach (var f in currentList)
            {
                if (count >= maxListLength)
                    break;
                Console.WriteLine(ExplorerDraw.GetFormattedText(f));
                count++;
            }
            count = 0;
            Console.SetCursorPosition(tempCursorPos.Item1, tempCursorPos.Item2);
            comp = inputBuilder.ToString();
            while (true)
            {
                maxListLength = Console.WindowHeight - topBuffer;


                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && inputBuilder.Length > 0)
                {
                    inputBuilder.Remove(inputBuilder.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (inputBuilder.Length < maxLength && !char.IsControl(key.KeyChar))
                {
                    inputBuilder.Append(key.KeyChar);
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
                var fileQuery = currentList.Where(f => f.DisplayName.Contains(inputBuilder.ToString(),
                                                StringComparison.OrdinalIgnoreCase));

                searchList = fileQuery.ToList();
                if (comp != inputBuilder.ToString())
                {
                    tempCursorPos = Console.GetCursorPosition();
                    Console.SetCursorPosition(0, 5);
                    Console.Write("\e[0J");
                    foreach (var f in fileQuery)
                    {
                        Console.WriteLine(ExplorerDraw.GetFormattedText(f));
                        if (count >= maxListLength)
                            break;
                        count++;

                    }
                    count = 0;
                    Console.SetCursorPosition(tempCursorPos.Item1, tempCursorPos.Item2);
                    comp = inputBuilder.ToString();
                }
            }
        }
        catch
        {

        }
        ExplorerDraw.RemoveCommandLine(cursorPos);

        // Todo:
        // Add Navigation and selection to search results here
        // Or create a new method for it
        // Also fix statusbar being erased


    }
}
