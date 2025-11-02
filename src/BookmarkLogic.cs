
using System.Text.Json;

namespace MshExplorer;

class BookmarkLogic
{
    public static void BookmarkMode(Dictionary<char, ExplorerItem> bookmarks, ref string currentPath)
    {
        TextStore.MarkKeys(TextStore.bookmarkMode);
        ConsoleKeyInfo key;

        key = Console.ReadKey(true);

        switch (key.KeyChar)
        {
            case 'a': // Add bookmark
                AddBookmark(bookmarks, currentPath);
                break;
            case 'b': // Go to bookmark
                GoToBookmark(bookmarks, ref currentPath);
                break;
            case 'c': // Clear a bookmark
                ClearBookmark(bookmarks);
                break;
        }
        TextStore.ClearMarkKeys(3);

    }
    public static void AddBookmark(Dictionary<char, ExplorerItem> bookmarks, string currentPath)
    {
        TextStore.ClearMarkKeys(3);
        TextStore.MarkKeys(TextStore.bookmarkAdd);
        string dirName = Path.GetFileName(currentPath);
        ExplorerItem item = new(dirName, currentPath, ExplorerType.DIRECTORY);

        ConsoleKeyInfo key;
        key = Console.ReadKey(true);

        if (bookmarks.ContainsKey(key.KeyChar)) // To do: Add error message later
            return;

        bookmarks.Add(key.KeyChar, item);
        WriteBookMarks(bookmarks);
    }

    private static void GoToBookmark(Dictionary<char, ExplorerItem> bookmarks, ref string currentPath)
    {
        TextStore.ClearMarkKeys(3);
        Bookmarks.DrawBookmarks(bookmarks, true);
        ConsoleKeyInfo key;
        key = Console.ReadKey(true);
        if (!bookmarks.ContainsKey(key.KeyChar))
            return;

        currentPath = bookmarks[key.KeyChar].Path;
    }
    private static void ClearBookmark(Dictionary<char, ExplorerItem> bookmarks)
    {
        TextStore.ClearMarkKeys(3);
        Bookmarks.DrawBookmarks(bookmarks, true);

        ConsoleKeyInfo key;
        key = Console.ReadKey(true);
        if (!bookmarks.ContainsKey(key.KeyChar))
            return;
        bookmarks.Remove(key.KeyChar);
        WriteBookMarks(bookmarks);
    }

    private static void WriteBookMarks(Dictionary<char, ExplorerItem> bookmarks) 
    {
        string? exeDir = AppContext.BaseDirectory;
        string filePath = string.Empty;

        if (!string.IsNullOrWhiteSpace(exeDir))
        {
            filePath = Path.Combine(exeDir, "bookmarks.json");
        }

        string json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions {WriteIndented = true});

        try
        {
            File.WriteAllText(filePath, json);
        }
        catch {}
    }

    public static Dictionary<char, ExplorerItem> LoadBookmarks(Dictionary<char, ExplorerItem> bookmarks)
    {
         string? exeDir = AppContext.BaseDirectory;
        string filePath = string.Empty;

        if (!string.IsNullOrWhiteSpace(exeDir))
        {
            filePath = Path.Combine(exeDir, "bookmarks.json");
        }

        if (!File.Exists(filePath))
            return new();

        string json = string.Empty;
        try
        {
            json = File.ReadAllText(filePath);
            bookmarks = new();

            bookmarks = JsonSerializer.Deserialize<Dictionary<char, ExplorerItem>>(json)!;
        }
        catch {}
        
        return bookmarks;
    }

}
