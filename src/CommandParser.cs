
namespace MshExplorer;

public enum CommandType
{
    NONE,
    SET_EDITOR,
}

public enum FirstToken
{
    NONE,
    SET,
}
public enum SecondToken
{
    NONE,
    EDITOR,
}

class CommandParser
{
    private static readonly Dictionary<string,int>firstToken = new Dictionary<string, int>()
    {
        ["set"] = 1,
    };
    private static readonly Dictionary<string,int>secondToken = new Dictionary<string, int>()
    {
        ["editor"] = 1,
    };
    

    public static string Parse(string command, ref CommandType commandType)
    {
        FirstToken token1 = 0;
        SecondToken token2 = 0;
        commandType = CommandType.NONE;

        
        if (string.IsNullOrEmpty(command))
            return string.Empty;
        string[] tokens = command.Split(' ');
        if (tokens.Length < 3)
            return string.Empty;

        string value = tokens[2];

        if (firstToken.ContainsKey(tokens[0]))
            token1 = (FirstToken)firstToken[tokens[0]];
        else
            return string.Empty;

        if (secondToken.ContainsKey(tokens[1]))
            token2 = (SecondToken)secondToken[tokens[1]];
        else 
            return string.Empty;

        if (token1 == FirstToken.NONE || token2 == MshExplorer.SecondToken.NONE)
            return string.Empty;

        switch (token1)
        {
            case FirstToken.SET:
                
                switch (token2)
                {
                    case SecondToken.EDITOR:
                        
                        commandType = CommandType.SET_EDITOR;
                        return value;
                }

                break;

        }

        return string.Empty;
    }

    

}
