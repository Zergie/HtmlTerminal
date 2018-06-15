using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlTerminal
{
    public class CommandCollection
    {
        public delegate void CommandHandler(string[] args);
        Dictionary<string, CommandHandler> commands;
        StringComparer Comparer;

        internal CommandCollection(StringComparer comparer)
        {
            Comparer = comparer;
            commands = new Dictionary<string, CommandHandler>(comparer);
        }

        public void Add(string command, CommandHandler handler)
        {
            commands.Add(command, handler);
        }

        public bool Contains(string command)
        {
            return commands.ContainsKey(command);
        }

        public CommandHandler this[string command]
        {
            get
            {
                return commands[command];
            }
            set
            {
                commands[command] = value;
            }
        }


        internal List<string> CompleteCommand(string command)
        {
            var Result = new List<string>();

            foreach (var item in commands.Keys)
            {
                if (item.Length >= command.Length
                    && Comparer.Equals(item.Substring(0, command.Length), command))
                    Result.Add(item);
            }

            Result.Sort();
            return Result;
        }





        internal static string[] SplitArguments(string text)
        {
            var Result = new List<string>();
            var SubResult = new StringBuilder();
            int brackets = 0;

            foreach (char c in text)
            {
                if (c == '"')
                {
                    brackets += brackets == 0 ? +1 : -1;
                }
                else if (c == ' ' && brackets == 0)
                {
                    if (SubResult.Length > 0)
                    {
                        Result.Add(SubResult.ToString());
                        SubResult.Clear();
                    }
                }
                else
                {
                    SubResult.Append(c);
                }
            }

            if (SubResult.Length > 0)
                Result.Add(SubResult.ToString());

            return Result.ToArray();
        }
    }
}
