using HtmlTerminal;
using System.Collections.Generic;
using System.IO;

namespace SimpleTerminal
{
    class CommandChangeDir : ICommand
    {
        public IEnumerable<CommandSuggestion> GetSuggestions(int argi, string[] args)
        {
            if (argi != 1)
                yield break;

            foreach (var item in CommandSuggestion.SuggestDirectories(argi, args))
            {
                item.HtmlText = CommandListDir.RenderDirectory(item.Text);
                yield return item;
            }
        }

        public void Run(Terminal terminal, string[] args)
        {
            string path = Path.GetFullPath(args[1]);
            Directory.SetCurrentDirectory(path);
            terminal.Prompt = $"{Directory.GetCurrentDirectory()}>";
        }
    }
}
