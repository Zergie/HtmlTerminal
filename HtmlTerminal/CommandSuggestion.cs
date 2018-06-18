using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HtmlTerminal
{
    public class CommandSuggestion
    {
        public CommandSuggestion(string text) : this(text, text)
        {
        }

        public CommandSuggestion(string text, string htmltext)
        {
            Text = text;
            HtmlText = htmltext;
        }

        public string Text { get; }
        public string HtmlText { get; set; }

        public override string ToString()
        {
            return HtmlText;
        }




        public static IEnumerable<CommandSuggestion> SuggestDirectories(int argi, string[] args)
        {
            string path = Directory.GetCurrentDirectory();

            if (args.Length == argi)
                return from d in Directory.GetDirectories(path) select new CommandSuggestion(d.Substring(path.Length));

            if (args[1].StartsWith("\\"))
            {
                path = Path.GetDirectoryName(args[1]) ?? args[1];
                string pattern = Path.GetFileName(args[1]);

                return from d in Directory.GetDirectories(path, pattern + "*") select new CommandSuggestion(d);
            }

            return from d in Directory.GetDirectories(path, args[1] + "*") select new CommandSuggestion(d.Substring(path.Length));
        }
    }
}
