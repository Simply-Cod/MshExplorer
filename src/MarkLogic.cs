
namespace MshExplorer;

class MarkLogic
{

    public static void MarkMode(MarkWindow markWindow, ExplorerItem current, string currentPath, ref bool dirChange, List<ExplorerItem> allListItems)
    {
        ConsoleKeyInfo key;

        TextStore.MarkKeys(TextStore.markMode);
        key = Console.ReadKey(true);

        switch (key.KeyChar)
        {
            case 'm': // Toggle mark on current file
                int idx = markWindow.MarkedList.FindIndex(x => x.Path == current.Path);
                if (idx >= 0)
                {
                    markWindow.MarkedList.RemoveAt(idx);
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
            case 'a': // Add all that are currently not in the marklist
                if (allListItems.Count > 0)
                {
                    var existing = new HashSet<string>(markWindow.MarkedList.Select(i => i.Path));
                    foreach (var item in allListItems)
                    {
                        if (existing.Add(item.Path))
                        {
                            markWindow.MarkedList.Add(item);
                        }
                    }

                }
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
                for (int i = markList.Count - 1; i >= 0; i--)
                {
                    if (markList[i].Type == ExplorerType.FILE)
                    {
                        Util.PasteFile(markList[i], currentPath, ref err);
                        markList.RemoveAt(i);
                    }
                    else if (markList[i].Type == ExplorerType.DIRECTORY)
                    {
                        Util.PasteDirectory(markList[i].Path, currentPath, ref err);
                        markList.RemoveAt(i);
                    }

                }
                break;
            case 'p':
                if (markList.Count > 0 && markList[markList.Count - 1].Type == ExplorerType.FILE)
                {
                    Util.PasteFile(markList[markList.Count - 1],currentPath, ref err);
                    markList.RemoveAt(markList.Count - 1);
                }
                else if (markList.Count > 0 && markList[markList.Count - 1].Type == ExplorerType.DIRECTORY)
                {
                    Util.PasteDirectory(markList[markList.Count - 1].Path,currentPath, ref err);
                    markList.RemoveAt(markList.Count - 1);
                }
                break;
            case 'f':
                if (markList.Count > 0 && markList[0].Type == ExplorerType.FILE)
                {
                    Util.PasteFile(markList[0],currentPath, ref err);
                    markList.RemoveAt(0);
                }
                else if (markList.Count > 0 && markList[0].Type == ExplorerType.DIRECTORY)
                {
                    Util.PasteDirectory(markList[0].Path,currentPath, ref err);
                    markList.RemoveAt(0);
                }
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
                    markList.RemoveAt(index);
                }
                else if (markList[index].Type == ExplorerType.DIRECTORY)
                {
                    Util.PasteDirectory(markList[index].Path, currentPath, ref err);
                    markList.RemoveAt(index);
                }
                break;

            default:
                return;
        }
        dirChange = true;

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
            case 'c':
                if (markList.Count > 0)
                    markList.RemoveAt(markList.Count -1);
                break;
            case 'f':
                if (markList.Count > 0)
                    markList.RemoveAt(0);
                break;
        }

    }
}
