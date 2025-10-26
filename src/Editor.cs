using System.Diagnostics;

namespace MshExplorer;

class Editor
{
    public static readonly string[] termEditors = [
        "nano",
        "nvim",
        "vim",
    ];
    public static readonly string[] guiEditors = [
        "code",
        "notepad"
    ];
    private enum EdType
    {
        UNKNOWN,
        TERMINAL,
        GUI
    };


    public static void OpenEditor(ExplorerItem argument, string editor)
    {
        EdType type = EdType.UNKNOWN;
        ProcessStartInfo psi = new();
        string arg = string.Empty;


        foreach (var e in Editor.termEditors)
        {
            if (editor == e)
                type = EdType.TERMINAL;
        }

        if (type == EdType.UNKNOWN)
        {
            foreach (var e in Editor.guiEditors)
            {
                if (editor == e)
                    type = EdType.GUI;
            }
        }


        if (argument.Type == ExplorerType.NONE)
            return;



        if (type == EdType.TERMINAL)
        {
            if (argument.Type == ExplorerType.FILE)
            {
                psi = new ProcessStartInfo
                {
                    FileName = editor,
                    Arguments = argument.Path,
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };
            }
            else
            {
                psi = new ProcessStartInfo
                {
                    FileName = editor,
                    WorkingDirectory = argument.Path,
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

            }
        }
        else
        {
            psi = new ProcessStartInfo
            {
                FileName = editor,
                Arguments = argument.Path,
                UseShellExecute = true
            };
        }


        try
        {
            Console.Write("\e[?25h");
            var process = Process.Start(psi);

            if (process != null)
                process.WaitForExit();

            Console.Write("\e[?25l");
        }
        catch
        {

        }
    }

}
