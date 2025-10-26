
namespace MshExplorer;

public enum CommandType
{
    NONE,
    QUIT,
    SET_EDITOR,
}


class CommandParser
{
    private static readonly Dictionary<string,CommandType> _commands = new()
    {
        ["set editor"] = CommandType.SET_EDITOR,
        ["quit"] = CommandType.QUIT,
    };
    
    public static (CommandType type, string val) Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return (CommandType.NONE, string.Empty);

        string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
            return (CommandType.NONE, string.Empty);

        string key = string.Empty;

        if (tokens.Length > 1)
            key = $"{tokens[0]} {tokens[1]}";
        else
            key = tokens[0];

        if (_commands.TryGetValue(key, out CommandType commandType))
        {
            string value;
            if (tokens.Length > 2)
                value = string.Join(' ', tokens.Skip(2));
            else 
                value = string.Empty;

            return (commandType, value);
        }

    
        return (CommandType.NONE, string.Empty);  
    }

}
