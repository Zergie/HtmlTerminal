using HtmlTerminal;
using System.Collections.Generic;
using System.IO;

namespace SimpleTerminal
{
    class CommandChangeDir : ICommand
    {
        public IEnumerable<string> GetSuggestions(int argi, string[] args)
        {
            if (argi != 1)
                yield break;

            foreach (var item in Command.SuggestDirectories(argi, args))
                yield return CommandListDir.RenderDirectory(item);
        }

        public void Run(Terminal terminal, string[] args)
        {
            string path = Path.GetFullPath(args[1]);
            Directory.SetCurrentDirectory(path);
            terminal.Prompt = $"{Directory.GetCurrentDirectory()}>";
        }
    }
}
