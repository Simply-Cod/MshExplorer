
using System.Text;

namespace MshExplorer;

class ListWindow
{

    const string reset = "\e[0m";
    const string bold = "\e[1m";
    const string orange = "\e[38;2;224;122;95m";
    const string darkBlue = "\e[38;2;61;64;91m";

    private int ScrollOffset;

    public const int StartX = 2;
    public const int StartY = 7;
    public int Width;
    public int Height;
    public int SelectedIndex;
    public int TopIndex;
    public List<ExplorerItem> Items;

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
    }

    public void DrawBorder()
    {
        StringBuilder sb = new();
        string top = $"{darkBlue}╭{new string('─', Width - 1)}╮\n";
        string middle = $"│{new string(' ', Width - 1)}│\n";
        string bottom = $"╰{new string('─', Width - 1)}╯{reset}\n";

        (int, int) cursorPos = Console.GetCursorPosition();

        for (int i = 0; i <= Height; i++)
        {
            //Console.SetCursorPosition(StartX, StartY + i);
            Console.Write($"\x1b[{StartY + i};{StartX + 1}H"); // Maybe faster than setcursor
            if (i == 0)
                Console.Write(top);
            else if (i == Height)
                Console.Write(bottom);
            else
                Console.Write(middle);
        }

        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public void SetItems(List<ExplorerItem> items)
    {
        Items = new(items);
        SelectedIndex = 0;
        ScrollOffset = 0;
    }
    public void SetHeight()
    {
        Height = Console.WindowHeight - (StartY + 1); // + 1 Because of statusBar
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
        DrawList();
    }

    public void DrawList()
    {
        (int, int) cursorPos = Console.GetCursorPosition();
        int indent = 2;
        int textIndent = 5;
        

        
        for (int i = 0; i < Height - 1; i++)
        {
            int itemIndex = ScrollOffset + i;
            Console.SetCursorPosition(StartX + indent, StartY + i);

            if (itemIndex < Items.Count)
            {
                //Issue: Ansi.GetFormattedText Slows down linux terminal Alot  🗎🗀
                //string text = Ansi.GetFormattedText(Items[itemIndex], NerdFont);
                
                string text = string.Empty;
               if (Items[itemIndex].Type == ExplorerType.FILE)
                    text = $" {Items[itemIndex].DisplayName}";
                else
                    text = $" {Items[itemIndex].DisplayName}/";

                // StringBuilder ------- 

              //  StringBuilder sb = new();
                
               // if (Items[itemIndex].Type == ExplorerType.FILE)
                //    sb.Append(f).Append(Items[itemIndex].DisplayName).Append(reset); 
               // else
                //    sb.Append(d).Append(Items[itemIndex].DisplayName).Append(reset); 


                //-----------------------➤

                if (itemIndex == SelectedIndex)
                    Console.Write($" {orange}{bold}>{reset} {text.PadRight(Width - textIndent)}{reset}");
                else
                    Console.Write($"   {text.PadRight(Width - textIndent)}");

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

        Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
    }

    public void UpdateConfigs(UserConfigs configs)
    {
        NerdFont = configs.NerdFont;
    }


}
