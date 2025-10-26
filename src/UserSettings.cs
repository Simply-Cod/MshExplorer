using System.Text.Json;

namespace MshExplorer;

class UserHandler
{
    public const string reset = "\e[0m";
    public const string bold = "\e[1m";
    public const string vimColor = "\e[38;2;90;157;82m";
    public const string nanoColor = "\e[38;2;117;0;204m";
    public const string vscColor = "\e[38;2;51;128;203m";

    private const string Path = "settings.json";

    public UserConfigs Configs;
    public string ExceptionMessage;

    public readonly Dictionary<string, string> Editors = new Dictionary<string, string>
    {
        ["vim"] = $"{vimColor}{bold} Vim{reset}",
        ["nvim"] = $"{vimColor}{bold} NeoVim{reset}",
        ["nano"] = $"{nanoColor}{bold} nano{reset}",
        ["code"] = $"{vscColor}{bold} Vs Code{reset}",
        ["notepad"] = $"{vscColor}{bold} notepad{reset}",
    };


    public UserHandler()
    {
        Configs = new();
        ExceptionMessage = string.Empty;
    }
    public string GetEditorStyle()
    {
        if (string.IsNullOrWhiteSpace(Configs.Editor))
            return string.Empty;

        if (Editors.TryGetValue(Configs.Editor, out var val))
            return val;

        return string.Empty;
        
    }

    public void WriteConfigs()
    {
       try
       {
           string json = JsonSerializer.Serialize(Configs, new JsonSerializerOptions {WriteIndented = true}); 
           File.WriteAllText(Path, json);
       }
       catch (Exception ex)
       {
            ExceptionMessage = ex.Message;
       }
    }

    public void ReadConfigs()
    {
       if (!File.Exists(Path))
           return;

       try
       {
           string json = File.ReadAllText(Path);
           if (!string.IsNullOrWhiteSpace(json))
               Configs = JsonSerializer.Deserialize<UserConfigs>(json) ?? new();
       }
       catch (Exception ex)
       {
            ExceptionMessage = ex.Message;
       }
    }

    public void SetEditor(string editor)
    {
        string[] editors = [
            "vim",
            "nvim",
            "nano",
            "notepad",
            "code",
            "null"
        ];

        if (editors.Contains(editor))
        {
            Configs.Editor = editor;
            WriteConfigs();
        }
        else
        {
            ExceptionMessage = "Not valid editor";
        }
        
    }


}

class UserConfigs
{
    public string Editor {get; set;} = string.Empty;
}


