
namespace MshExplorer;

class Bookmarks
{

    public Dictionary<char, ExplorerItem> Items;

    public Bookmarks()
    {
        Items = new();
    }

    public static void DrawBookmarks(Dictionary<char, ExplorerItem> bookmarks, bool nerdFont)
    {
        Console.Write("\e[H");

        int lines = 0;
        foreach (var item in bookmarks)
        {
            if (lines > 3)
                break;
            if (Console.CursorLeft + item.Value.DisplayName.Length >= Console.WindowWidth - 5)
            {
                Console.WriteLine();
                lines++;
            }
            Console.Write($"[{item.Key}]    {Ansi.GetFormattedText(item.Value, nerdFont)}    ");
        }
    }
}
