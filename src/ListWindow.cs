
using System.Runtime.CompilerServices;

namespace MshExplorer;

class ListWindow
{

    const string reset = "\e[0m";
    const string bold = "\e[1m";
    const string orange = "\e[38;2;224;122;95m";
    const string darkBlue = "\e[38;2;61;64;91m";


    public int StartX;
    public int StartY;
    public int Width;
    public int Height;
    public int SelectedIndex;
    public int TopIndex;

    private int TextLeft => StartX + 2;
    private int ViewRows => Math.Max(0, Height - 1);
    private int TextWidth => Math.Max(0, Width - 3);

    public ListWindow(int startX, int startY, int width, int height)
    {
        StartX = startX;
        StartY = startY;
        Width = width;
        Height = height;
        SelectedIndex = 0;
        TopIndex = 0;
    }

    public void DrawBorder()
    {
        (int, int) _cursorPos = Console.GetCursorPosition();
        Console.SetCursorPosition(StartX, StartY);

        for (int i = 0; i <= Height; i++)
        {
            for (int j = 0; j <= Width; j++)
            {
                if (i == 0)
                {
                    if (j == 0)
                        Console.Write($"{darkBlue}╭");
                    else if (j == Width)
                        Console.Write("╮");
                    else
                        Console.Write("─");
                }
                else if (i == Height)
                {
                    if (j == 0)
                        Console.Write("╰");
                    else if (j == Width)
                        Console.Write($"╯{reset}");
                    else
                        Console.Write("─");
                }
                else
                {
                    if (j == 0)
                        Console.Write("│");
                    else if (j == Width)
                        Console.Write("│");
                    else
                        Console.Write(" ");
                }
            }
            if (i < Height)
                Console.SetCursorPosition(StartX, StartY + (i + 1));
        }
        Console.SetCursorPosition(_cursorPos.Item1, _cursorPos.Item2);
    }

    public void DrawListFull(List<ExplorerItem> list)
    {
        (int, int) _cursorPos = Console.GetCursorPosition();

        for (int row = 0; row < ViewRows; row++)
        {
            int itemIndex = TopIndex + row;
            int y = StartY + 1 + row;

            if (itemIndex >= 0 && itemIndex < list.Count)
            {
                bool highlight = (itemIndex == SelectedIndex);
                WriteRow(y, list[itemIndex], highlight);
            }
            else
            {
                ClearRow(y);
            }

        }
        // This places the cursor on the selected row at redraw
        //int cursorRow = Math.Clamp(SelectedIndex - TopIndex, 0, Math.Max(0, ViewRows - 1));
       // Console.SetCursorPosition(TextLeft, StartY + 1 + cursorRow);
        Console.SetCursorPosition(_cursorPos.Item1, _cursorPos.Item2);

    }

    public void MoveSelection(List<ExplorerItem> list, int delta)
    {
        if (list.Count == 0 || ViewRows == 0) return;

        int oldSelected = SelectedIndex;
        int oldTop = TopIndex;

        // Wrap around
        SelectedIndex = ((SelectedIndex + delta) % list.Count + list.Count) % list.Count;

        if (SelectedIndex < TopIndex)
            TopIndex = SelectedIndex;
        else if (SelectedIndex >= TopIndex + ViewRows)
            TopIndex = SelectedIndex - ViewRows + 1;

        if (TopIndex != oldTop)
        {
            DrawListFull(list);
            return;
        }

        int oldRow = oldSelected - TopIndex;
        if (oldRow >= 0 && oldRow < ViewRows && oldSelected >= 0 && oldSelected < list.Count)
            WriteRow(StartY + 1 + oldRow, list[oldSelected], highlighted: false);

        int newRow = SelectedIndex - TopIndex;
        if (newRow >= 0 && newRow < ViewRows)
            WriteRow(StartY + 1 + newRow, list[SelectedIndex], highlighted: true);

        Console.SetCursorPosition(TextLeft, StartY + 1 + newRow);
    }

    public bool Resize(int newHeight, List<ExplorerItem> list)
    {
        if (newHeight == Height) return false;

        Height = newHeight;

        int maxTop = Math.Max(0, list.Count - ViewRows);
        if (TopIndex > maxTop)
            TopIndex = maxTop;
        if (SelectedIndex < 0 && list.Count > 0)
            SelectedIndex = 0;
        if (SelectedIndex >= list.Count && list.Count > 0)
            SelectedIndex = list.Count - 1;

        return true;
    }

    private void WriteRow(int y, ExplorerItem item, bool highlighted)
    {
        (int, int) cursorPos = Console.GetCursorPosition();

        int innerLeft = StartX + 1;
        int innerWidth = Math.Max(0, Width - 1);

        Console.SetCursorPosition(innerLeft, y);
        Console.Write(new string(' ', innerWidth));
        Console.SetCursorPosition(innerLeft, y);

        string prefix = "  ";
        if (highlighted)
            prefix = $"{orange}{reset} ";

        string content = ExplorerDraw.GetFormattedText(item);
        if (string.IsNullOrEmpty(content))
            content = string.Empty;

        int contentRoom = Math.Max(0, innerWidth - VisibleLength(prefix));
        content = TruncateVisible(content, contentRoom);

        string line = prefix + content;
        int deficit = innerWidth - VisibleLength(line);
        if (deficit > 0)
            line += new string(' ', deficit);

        Console.Write(line);

        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    private void ClearRow(int y)
    {
        (int, int) cursorPos = Console.GetCursorPosition();

        // Test
        int innerLeft = StartX + 1;
        int innerWidth = Math.Max(0, Width - 1);

        //----

        Console.SetCursorPosition(innerLeft, y); // Test TextLeft replaced with innerLeft
        Console.Write(new string(' ', innerWidth)); // and TextWidth replaced with innerWidth
        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    private static string StripAnsi(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        var sb = new System.Text.StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (c == '\x1b' && i + 1 < s.Length && s[i + 1] == '[')
            {
                i += 2;
                
                while (i < s.Length)
                {
                    char ch = s[i];
                    if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))
                        break;
                    i++;
                }
                
                continue;
            }
            sb.Append(c);
        }
        return sb.ToString();
    }

    private static int VisibleLength(string s) => StripAnsi(s).Length;
    
    private static string TruncateVisible(string s, int maxVisible)
    {
        if (string.IsNullOrEmpty(s) || maxVisible <= 0) return string.Empty;

        int visible = 0;
        var sb = new System.Text.StringBuilder(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            
            if (c == '\e' && i + 1 < s.Length && s[i + 1] == '[')
            {
                int start = i;
                i += 2;
                while (i < s.Length)
                {
                    char ch = s[i];
                    if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))
                        break;
                    i++;
                }
                int end = Math.Min(i, s.Length - 1);
                sb.Append(s.AsSpan(start, end - start + 1));
                continue;
            }

            if (visible >= maxVisible) break;

            sb.Append(c);
            visible++;
        }

        return sb.ToString();
    }

}
