using System.Collections.Generic;

namespace HtmlTerminal
{
    public interface ICommand
    {
        void Run(Terminal terminal, string[] args);
        IEnumerable<CommandSuggestion> GetSuggestions(int argi, string[] args);
    }
}
