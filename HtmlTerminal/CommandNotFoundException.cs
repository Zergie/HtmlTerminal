using System;

namespace HtmlTerminal
{
    [Serializable]
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException(string[] args) : base($"command not found: {args[0]}")
        {
            Command = args[0];
            Arguments = args;
        }

        public string Command { get; }
        public string[] Arguments { get; }
    }
}