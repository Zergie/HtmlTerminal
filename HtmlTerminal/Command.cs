using System.Collections.Generic;

namespace HtmlTerminal
{
    public class Command : ICommand
    {
        public Command(RunHandler onRun)
        {
            Run = onRun;
        }

        public delegate void RunHandler(Terminal terminal, string[] args);
        public RunHandler Run { get; set; } = null;
        void ICommand.Run(Terminal terminal, string[] args) => Run(terminal, args);

        public delegate IEnumerable<CommandSuggestion> GetSuggestionsHandler(int argi, string[] args);
        public GetSuggestionsHandler GetSuggestions { get; set; } = null;
        IEnumerable<CommandSuggestion> ICommand.GetSuggestions(int argi, string[] args) => GetSuggestions(argi, args);

    }
}
