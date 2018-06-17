using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public delegate IEnumerable<string> GetSuggestionsHandler(int argi, string[] args);
        public GetSuggestionsHandler GetSuggestions { get; set; } = null;
        IEnumerable<string> ICommand.GetSuggestions(int argi, string[] args) => GetSuggestions(argi, args);




        public static IEnumerable<string> SuggestDirectories(int argi, string[] args)
        {
            if (args.Length == argi)
                return from d in Directory.GetDirectories(Directory.GetCurrentDirectory()) select Path.GetFileName(d);

            return from d in Directory.GetDirectories(Directory.GetCurrentDirectory(), args[1] + "*") select Path.GetFileName(d);
        }
    }
}
