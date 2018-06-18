using HtmlTerminal;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace SimpleTerminal
{
    internal class CommandPing : ICommand
    {
        public IEnumerable<CommandSuggestion> GetSuggestions(int argi, string[] args)
        {
            return null;
        }

        public void Run(Terminal terminal, string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                var progressBar = terminal.CreateElement("div");
                progressBar.Style = "height:20px;background-color:green;width:0%";

                var text = terminal.CreateElement("span");
                text.Style = "top: -19px; position: relative; background-color: rgba(0, 0, 0, 0);";
                text.InnerText = args[i];

                var div = terminal.CreateElement("div");
                div.Style = "margin-bottom: -19px;";
                div.AppendChild(progressBar);
                div.AppendChild(text);

                terminal.Write(div);

                var p = new PingHelper(args[i], progressBar, text);
                p.SendPingAsync();
            }
        }

        private class PingHelper
        {
            internal PingHelper(string hostNameOrAddress, HtmlElement progressBar, HtmlElement text)
            {
                HostNameOrAddress = hostNameOrAddress;

                ProgressBar = progressBar;
                Text = text;

                Ping = new Ping();
                Ping.PingCompleted += PingCompleted;
            }


            private string HostNameOrAddress;
            private int Count = 0;
            private List<long> RoundtripTime = new List<long>();

            HtmlElement ProgressBar { get; }
            HtmlElement Text { get; }
            Ping Ping { get; }


            private void PingCompleted(object sender, PingCompletedEventArgs e)
            {
                Count++;
                ProgressBar.Style = $"height:20px;background-color:green;width:{100 * Count / 20}%";

                if (e.Error != null)
                {
                    ProgressBar.Style = $"height:20px;width:0%";
                    Text.Style += "color:red;";
                    Text.InnerText = e.Error.Message;

                    Ping.PingCompleted -= PingCompleted;
                    Ping.Dispose();
                    return;
                }

                RoundtripTime.Add(e.Reply.RoundtripTime);
                Text.InnerText = $"{HostNameOrAddress} ({e.Reply.Status})";

                if (Count == 20)
                {
                    ProgressBar.Style = $"height:20px;width:{100 * Count / 20}%";
                    Text.InnerText = $"address: {e.Reply.Address}, {RoundtripTime.Average()} ms, min {RoundtripTime.Min()} ms, max {RoundtripTime.Max()} ms";

                    Ping.PingCompleted -= PingCompleted;
                    Ping.Dispose();
                }
                else
                {
                    SendPingAsync();
                }
            }

            public void SendPingAsync()
            {
                Ping.SendPingAsync(HostNameOrAddress, 5000);
            }
        }
    }
}