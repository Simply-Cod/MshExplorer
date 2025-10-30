
namespace MshExplorer;

class MarkLogic
{

    public static void MarkMode(MarkWindow markWindow, ExplorerItem current, string currentPath, ref bool dirChange)
    {
        ConsoleKeyInfo key;

        TextStore.MarkKeys(TextStore.markMode);
        key = Console.ReadKey(true);

        switch (key.KeyChar)
        {
            case 'm': // Toggle mark on current file
                ExplorerItem temp = new(string.Empty, string.Empty, ExplorerType.NONE);
                bool itemRemoved = false;
                if (markWindow.MarkedList.Count > 0)
                {
                    foreach (var item in markWindow.MarkedList)
                    {
                        if (current.Path == item.Path)
                        {
                            markWindow.MarkedList.Remove(item);
                            itemRemoved = true;
                            break;
                        }
                    }
                    if (!itemRemoved)
                        markWindow.MarkedList.Add(current);
                }
                else
                {
                    markWindow.MarkedList.Add(current);
                }
                break;
            case 'p':
                if (markWindow.MarkedList.Count > 0)
                    PasteMode(markWindow.MarkedList, currentPath, ref dirChange);
                break;
            case 'c':
                if (markWindow.MarkedList.Count > 0)
                    ClearMode(markWindow.MarkedList);
                break;
        }
        TextStore.ClearMarkKeys(3);
    }

    public static void PasteMode(List<ExplorerItem> markList, string currentPath, ref bool dirChange)
    {
        TextStore.ClearMarkKeys(3);
        TextStore.MarkKeys(TextStore.markPasteMode);
        string err = string.Empty;

        ConsoleKeyInfo key;
        key = Console.ReadKey(true);

        switch (key.KeyChar)
        {

            case 'a':
                foreach (var item in markList)
                {
                    if (item.Type == ExplorerType.FILE)
                        Util.PasteFile(item, currentPath, ref err);
                    else if (item.Type == ExplorerType.DIRECTORY)
                        Util.PasteDirectory(item.Path, currentPath, ref err);

                }
                dirChange = true;
                break;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                int index = key.KeyChar - '0';
                if (index > markList.Count - 1)
                    break;

                if (markList[index].Type == ExplorerType.FILE)
                {
                    Util.PasteFile(markList[index], currentPath, ref err);
                }
                else if (markList[index].Type == ExplorerType.DIRECTORY)
                {
                    Util.PasteDirectory(markList[index].Path, currentPath, ref err);
                }
                dirChange = true;
                break;
        }

    }

    public static void ClearMode(List<ExplorerItem> markList)
    {
        TextStore.ClearMarkKeys(3);
        TextStore.MarkKeys(TextStore.markClearMode);

        ConsoleKeyInfo key;
        key = Console.ReadKey(true);

        switch (key.KeyChar)
        {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                int index = key.KeyChar - '0';
                if (index > markList.Count - 1)
                    break;
                markList.RemoveAt(index);
                break;
            case 'a':
                markList.Clear();
                break;
        }

    }
}
