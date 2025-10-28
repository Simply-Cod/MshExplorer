
namespace MshExplorer;

class PathStyler
{
    public string TextStyle;
    public string DividerStyle;
    public string Reset;
    public bool Active;

    public PathStyler()
    {
        TextStyle = Ansi.PathBNames;
        DividerStyle = Ansi.PathBDivider;
        Reset = Ansi.reset;
        Active = true;
    }
    public void Activate()
    {
        TextStyle = Ansi.PathBNames;
        DividerStyle = Ansi.PathBDivider;
        Reset = Ansi.reset;
        Active = true;
    }
    public void Deactivate()
    {
        TextStyle = string.Empty;
        DividerStyle = string.Empty;
        Reset = string.Empty;
        Active = false;
    }
    public void SetStyle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        if (value == "true" || value == "false")
        {
            if (value == "true")
                Activate();
            else
                Deactivate();
        }
        else
        {
            return;
        }
    }
}

class StatusStyler
{
    public string Background;
    public string Editor;
    public string Clipboard;
    public string Reset;
    public bool Active;

    public StatusStyler()
    {
        Background = Ansi.bgDark;
        Editor = string.Empty;
        Clipboard = string.Empty;
        Reset = Ansi.reset;
        Active = false;
    }
}

class ListStyler
{
    public string Cursor;
    public string Reset;
    public string Border;
    public bool Active;

    public ListStyler()
    {
        Cursor = Ansi.ListCur;
        Reset = Ansi.reset;
        Border = Ansi.Border;
        Active = true;
    }
    public void Activate()
    {
        Cursor = Ansi.ListCur;
        Reset = Ansi.reset;
        Border = Ansi.Border;
        Active = true;
    }
    public void Deactivate()
    {
        Cursor = string.Empty;
        Reset = string.Empty;
        Border = string.Empty;
        Active = false;
    }
    public void SetStyle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        if (value == "true" || value == "false")
        {
            if (value == "true")
                Activate();
            else
                Deactivate();
        }
        else
        {
            return;
        }

    }

}

class HelpStyler
{
    public string Border;
    public string Header;
    public string Text;
    public string Reset;
    public bool Active;

    public string InfoHeader;
    public string InfoHL;

    

    public HelpStyler()
    {
        Border = Ansi.Border;
        Header = Ansi.Header;
        Text = Ansi.HelpText;
        Reset = Ansi.reset;
        InfoHeader = Ansi.InfoHeader;
        InfoHL = Ansi.InfoHighL;
        Active = true;
    }

    public void Activate()
    {
        Border = Ansi.Border;
        Header = Ansi.Header;
        Text = Ansi.HelpText;
        Reset = Ansi.reset;
        InfoHeader = Ansi.InfoHeader;
        InfoHL = Ansi.InfoHighL;
        Active = true;
    }
    public void Deactivate()
    {
        Border = string.Empty;
        Header = string.Empty;
        Text = string.Empty;
        Reset = string.Empty;
        InfoHeader = string.Empty;
        InfoHL = string.Empty;
        Active = false;
    }
    public void SetStyle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        if (value == "true" || value == "false")
        {
            if (value == "true")
                Activate();
            else
                Deactivate();
        }
        else
        {
            return;
        }

    }
}

class CommandStyler
{
    public string Border;
    public string Header;
    public string Reset;
    public bool Active;

    public CommandStyler()
    {
        Border = string.Empty;
        Header = string.Empty;
        Reset = string.Empty;
        Active = false;
    }
}


