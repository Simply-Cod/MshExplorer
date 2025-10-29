using System.Text.Json;

namespace MshExplorer;

class UserHandler
{

    private const string Path = "settings.json";
    public UserConfigs Configs;
    public string ExceptionMessage;

    public UserHandler()
    {
        Configs = new();
        ExceptionMessage = string.Empty;
    }
    
    public void WriteConfigs()
    {
        try
        {
            string json = JsonSerializer.Serialize(Configs, new JsonSerializerOptions { WriteIndented = true });
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

        if (Editor.Editors.Contains(editor))
        {
            Configs.Editor = editor;
            WriteConfigs();
        }
        else
        {
            ExceptionMessage = "Not valid editor";
        }

    }

    public void SetNerdFont(string val)
    {
        if (val == "true" || val == "false")
        {
            if (val == "true")
            {
                Configs.NerdFont = true;
            }
            else if (val == "false")
            {
                Configs.NerdFont = false;
            }
            WriteConfigs();
        }
    }

    public void Update(ListStyler listS, PathStyler pathS, HelpStyler helpS)
    {
        Configs.ListStyle = listS.Active;
        Configs.PathStyle = pathS.Active;
        Configs.HelpStyle = helpS.Active;

        WriteConfigs();
    }


}

class UserConfigs
{
    public string Editor { get; set; } = string.Empty;
    public bool NerdFont { get; set; } = true;
    public bool ListStyle {get; set;} = true;
    public bool PathStyle {get; set;} = true;
    public bool HelpStyle {get ; set;} = true;
}


