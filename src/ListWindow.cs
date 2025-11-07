
using System.Text;

namespace MshExplorer;

class ListWindow
{
    private int ScrollOffset;

    public const int StartX = 2;
    public const int StartY = 7;
    public int Width;
    public int Height;
    public int SelectedIndex;
    public int TopIndex;
    public List<ExplorerItem> Items;
    private List<string> styleCacheList;
    public ListStyler Style;

    public string PreviousPath;

    public bool NerdFont;


    public ListWindow(int width, List<ExplorerItem> items)
    {
        Width = width;
        Height = Console.WindowHeight - (StartY + 1); // + 1 Because of statusBar
        SelectedIndex = 0;
        TopIndex = 0;
        ScrollOffset = 0;
        Items = items;
        NerdFont = false;
        Style = new();
        styleCacheList = new();

        PreviousPath = string.Empty;
    }

    public void DrawBorder()
    {
        StringBuilder sb = new();
        string top = $"{Style.Border}╭{new string('─', Width - 1)}╮\n";
        string middle = $"│{new string(' ', Width - 1)}│\n";
        string bottom = $"╰{new string('─', Width - 1)}╯{Style.Reset}\n";


        for (int i = 0; i <= Height; i++)
        {
            Console.Write($"\x1b[{StartY + i};{StartX + 1}H"); // Maybe faster than setcursor
            if (i == 0)
                Console.Write(top);
            else if (i == Height)
                Console.Write(bottom);
            else
                Console.Write(middle);
        }

    }

    public void ReCheckMarks(string currentPath, List<ExplorerItem> markList)
    {
        if (currentPath != PreviousPath)
        {
            PreviousPath = currentPath;
            return;
        }
        if (Items.Count > 0)
        {
            var markedPaths = new HashSet<string>(markList.Select(m => m.Path));

            if (markList.Count == 0)
            {
                foreach (var item in Items)
                {
                    item.Marked = false;
                }
                return;
            }
            foreach (var item in Items)
            {
                if (markedPaths.Contains(item.Path))
                    item.Marked = true;
                else
                    item.Marked = false;
            }

        }
    }

    public void SetItems(List<ExplorerItem> items, List<ExplorerItem> markList)
    {
        Items = new(items);
        SelectedIndex = 0;
        ScrollOffset = 0;
        SetStyleCacheList();

        if (items.Count > 0 && markList.Count > 0)
        {
            var markedPaths = new HashSet<string>(markList.Select(m => m.Path));

            foreach (var item in items)
            {
                if (markedPaths.Contains(item.Path))
                    item.Marked = true;
            }
        }

    }
    // --- Test --- 
    private void SetStyleCacheList()
    {
        if (Items.Count <= 0)
            return;
        styleCacheList = new();
        int maxLength = 30;
        int textIndent = 5;

        foreach (var item in Items)
        {
            styleCacheList.Add($"{Ansi.GetFormattedText(item, NerdFont, maxLength).PadRight(Width - textIndent)}");
        }

    }
    // --- 
    public void SetHeight()
    {
        Height = Console.WindowHeight - (StartY + 1);
    }

    public void ScrollUp()
    {
        if (Items.Count == 0)
            return;

        if (SelectedIndex == 0)
        {
            SelectedIndex = Items.Count - 1;
            ScrollOffset = Math.Max(0, Items.Count - Height + 1);
        }
        else
        {
            SelectedIndex--;
            if (SelectedIndex < ScrollOffset)
                ScrollOffset--;
        }
        SetHeight();
        DrawList();

    }

    public void ScrollDown()
    {
        if (Items.Count == 0)
            return;

        if (SelectedIndex == Items.Count - 1)
        {
            SelectedIndex = 0;
            ScrollOffset = 0;
        }
        else
        {
            SelectedIndex++;
            if (SelectedIndex >= ScrollOffset + Height - 1)
                ScrollOffset++;
        }
        SetHeight();
        DrawList();
    }

    public void DrawList()
    {
        int indent = 2;
        string listCursor = $"{Style.Cursor}>{Style.Reset}";


        for (int i = 0; i < Height - 1; i++)
        {
            int itemIndex = ScrollOffset + i;
            //Console.SetCursorPosition(StartX + indent, StartY + i);
            Console.Write($"\x1b[{StartY + i + 1};{StartX + indent}H"); // Maybe faster than setcursor

            if (itemIndex < Items.Count)
            {
                string text = string.Empty;
                if (Style.Active)
                {
                    text = styleCacheList[itemIndex];
                }
                else
                {
                    if (Items[itemIndex].Type == ExplorerType.FILE)
                        text = $" {Items[itemIndex].DisplayName}";
                    else
                        text = $" *{Items[itemIndex].DisplayName}/";
                }

                if (Items[itemIndex].Marked)
                {
                    string mark = "\e[48;5;240m";

                    if (itemIndex == SelectedIndex)
                        Console.Write($"  {listCursor} {mark}{text}");
                    else
                        Console.Write($"    {mark}{text}");
                }
                else
                {
                    if (itemIndex == SelectedIndex)
                        Console.Write($"  {listCursor} {text}");
                    else
                        Console.Write($"    {text}");
                }

                int curX = Console.CursorLeft;
                int remaining = (StartX + Width) - curX - 1;

                if (remaining > 0)
                    Console.Write(new string(' ', remaining));

            }
            else
            {
                if (i != Height - 1)
                    Console.Write(new string(' ', Width - indent));
            }
        }
    }

    public void UpdateConfigs(UserConfigs configs)
    {
        NerdFont = configs.NerdFont;
        Style.Active = configs.ListStyle;

        if (Style.Active)
            Style.Activate();
        else
            Style.Deactivate();
    }

}
