using HtmlTerminal;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleTerminal
{
    class CommandListDir : ICommand
    {
        public IEnumerable<CommandSuggestion> GetSuggestions(int argi, string[] args)
        {
            if (argi != 1)
                yield break;

            foreach (var item in CommandSuggestion.SuggestDirectories(argi, args))
            {
                item.HtmlText = RenderDirectory(item.Text);
                yield return item;
            }
        }

        public void Run(Terminal terminal, string[] args)
        {
            string path = args.Length == 1 ? Directory.GetCurrentDirectory() : args[1];

            var Result = new List<string>();

            foreach (var d in Directory.GetDirectories(path))
            {
                Result.Add(RenderDirectory(d));
            }

            foreach (var f in Directory.GetFiles(path))
            {
                Result.Add(RenderFile(f));
            }

            terminal.Write(Result);
        }

        internal static string RenderFile(string f)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><td valign='middle'>");

            sb.AppendLine("<svg style='height: 32px; width: 32px;' viewBox='0 0 32 32'>");
            sb.AppendLine("<path d='M28.681 7.159c-0.694-0.947-1.662-2.053-2.724-3.116s-2.169-2.030-3.116-2.724c-1.612-1.182-2.393-1.319-2.841-1.319h-15.5c-1.378 0-2.5 1.121-2.5 2.5v27c0 1.378 1.122 2.5 2.5 2.5h23c1.378 0 2.5-1.122 2.5-2.5v-19.5c0-0.448-0.137-1.23-1.319-2.841zM24.543 5.457c0.959 0.959 1.712 1.825 2.268 2.543h-4.811v-4.811c0.718 0.556 1.584 1.309 2.543 2.268zM28 29.5c0 0.271-0.229 0.5-0.5 0.5h-23c-0.271 0-0.5-0.229-0.5-0.5v-27c0-0.271 0.229-0.5 0.5-0.5 0 0 15.499-0 15.5 0v7c0 0.552 0.448 1 1 1h7v19.5z'></path>");
            sb.AppendLine("</svg>");

            sb.AppendLine("</td><td valign='middle'>");

            sb.Append($"{ Path.GetFileName(f)}");

            sb.AppendLine("</td></tr>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        internal static string RenderDirectory(string d)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><td valign='middle'>");

            sb.AppendLine("<svg style='height: 32px; width: 32px;' viewBox='0 0 32 32'>");
            sb.AppendLine("<path d='M14 4l4 4h14v22h-32v-26z'></path>");
            sb.AppendLine("</svg>");

            sb.AppendLine("</td><td valign='middle'>");

            sb.AppendLine($"{Path.GetFileName(d)}");
            sb.AppendLine("</td></tr>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}
