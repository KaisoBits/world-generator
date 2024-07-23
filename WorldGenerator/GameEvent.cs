using System.Text.RegularExpressions;

namespace WorldGenerator;

public abstract class GameEvent
{
    public abstract EventType Type { get; }
    public virtual string? Message { get; }

    private readonly Dictionary<string, string> _parameters = [];

    protected void SetParameter(string type, string value) 
    {
        _parameters[type] = value;
    }

    public override string ToString()
    {
        if (Message == null)
            return $"[{Type}]";

        MatchCollection templates = Regex.Matches(Message, @"{(.*?)}");

        string msg = Message;
        foreach (Match param in templates.Reverse())
        {
            var templateName = param.Groups[1].Value;
            if (!_parameters.TryGetValue(templateName, out string? value))
                continue;

            msg = msg[..param.Index] + value + msg[(param.Index + param.Length)..];
        }

        return $"[{Type}] {msg}";
    }
}
