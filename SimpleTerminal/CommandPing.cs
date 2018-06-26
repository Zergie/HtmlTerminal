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
            if (argi == 1 || !args[argi - 1].StartsWith("-"))
            {
                yield return new CommandSuggestion("-n");
                yield return new CommandSuggestion("-w");
                yield return new CommandSuggestion("-ttl");
                yield return new CommandSuggestion("-DontFragment");
            }
        }

        public void Run(Terminal terminal, string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                var maxPakets = 10;
                var timeout = 1000;
                var DontFragment = false;
                var ttl = 128;

                if (string.Compare(args[i], "-n", true) == 0)
                {
                    maxPakets = int.Parse(args[i + 1]);
                    i += 2;
                }
                if (string.Compare(args[i], "-w", true) == 0)
                {
                    timeout = int.Parse(args[i + 1]);
                    i += 2;
                }
                if (string.Compare(args[i], "-ttl", true) == 0)
                {
                    ttl = int.Parse(args[i + 1]);
                    i += 2;
                }
                if (string.Compare(args[i], "-DontFragment", true) == 0)
                {
                    DontFragment = true;
                    i += 1;
                }

                var p = new PingHelper(terminal, args[i]);
                p.Pakets = maxPakets;
                p.TimeOut = timeout;
                p.TimeToLive = ttl;
                p.DontFragment = DontFragment;
                p.SendPingAsync();
            }
        }

        private class PingHelper
        {
            internal PingHelper(Terminal terminal, string hostNameOrAddress)
            {

                HostNameOrAddress = hostNameOrAddress;

                div = terminal.CreateElement("div");
                div.Style = "height:20px;background-color:green;width:0%";

                span = terminal.CreateElement("span");
                span.Style = "top: -19px; position: relative; background-color: rgba(0, 0, 0, 0);";
                span.InnerText = $"pinging: {HostNameOrAddress}";

                container = terminal.CreateElement("div");
                container.Style = "margin-bottom: -19px;";
                container.AppendChild(div);
                container.AppendChild(span);

                terminal.Write(container);

                Ping = new Ping();
                Ping.PingCompleted += PingCompleted;
            }


            private string HostNameOrAddress;
            private int Count = 0;
            private List<long> RoundtripTime = new List<long>();
            private int LostCount = 0;

            private HtmlElement container { get; }
            private HtmlElement div { get; }
            private HtmlElement span { get; }
            private Ping Ping { get; }

            public int Pakets { get; set; }
            public int TimeOut { get; set; }
            public bool DontFragment { get; set; }
            public int TimeToLive { get; set; }


            private void PingCompleted(object sender, PingCompletedEventArgs e)
            {
                Count++;
                div.Style = $"height:20px; background-color:green; width:{100 * Count / Pakets}%";

                if (e.Error != null)
                {
                    LostCount++;
                    span.InnerText = $"{HostNameOrAddress} ({e.Error.InnerException.Message})";
                }
                else
                {
                    if (e.Reply.RoundtripTime == 0)
                        LostCount++;
                    else
                        RoundtripTime.Add(e.Reply.RoundtripTime);
                    span.InnerText = $"{HostNameOrAddress} ({e.Reply.Status})";
                }

                if (Count == Pakets)
                {
                    double lost_percent = (double)LostCount / Count;

                    container.InnerHtml = "<div style='background-color: red;position: absolute;width: 100px;height: 100px;border-radius: 50px;box-shadow: 0px 0px 0px 1px;'></div>" +
                                        $"<div style='transform: rotate({ -90 + (lost_percent > .5 ? (lost_percent - .5) * 360 : 0): 0}deg);transform-origin: bottom;background-color: green;position: absolute;width: 100px;height: 50px;border-radius: 50px 50px 0 0;'></div>" +
                                        $"<div style='transform: rotate({  90 + (lost_percent < .5 ? lost_percent * 360 : 0): 0}deg);transform-origin: bottom;background-color: { (lost_percent < .5 ? "green" : "red") };position: absolute;width: 100px;height: 50px;border-radius: 50px 50px 0 0;'></div>" +
                                        $"<div style='position: absolute; background-color: rgba(0,0,0,0); height: 100px;display: flex;align-items: center;'><span style='text-align: center; width: 100px; background-color: rgba(0,0,0,0);'>{ (lost_percent * 100).ToString("0") } % lost</span></div>" +
                                        $"<table style='display: block;padding-left: 120px;padding-top: 5px;'>" +
                                        $"<tr><td style='padding: 0px;'>{Count}</td>              <td style='padding: 0px 0px 0px 4px;'>paket(s) send</td></tr>" +
                                        $"<tr><td style='padding: 0px;'>{RoundtripTime.Count}</td><td style='padding: 0px 0px 0px 4px;'>paket(s) received</td></tr>" +
                                        $"<tr><td style='padding: 0px;'>{LostCount}</td>          <td style='padding: 0px 0px 0px 4px;'>paket(s) lost</td></tr>" +
                                        $"</table>" +
                                        (RoundtripTime.Count == 0 ? "" :
                                            $"<span style='display: block;padding-left: 120px;font-weight: bold;'>Roundtrip time:</span>" +
                                            $"<span style='display: block;padding-left: 120px;'>average {RoundtripTime.Average(): 0.0} ms, min {RoundtripTime.Min(): 0.0} ms, max {RoundtripTime.Max(): 0.0} ms</span>"
                                        );
                    container.Style = @"height: 100px;";
                    container.ScrollIntoView(true);

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
                Ping.SendPingAsync(HostNameOrAddress, TimeOut, new byte[] { }, new PingOptions { DontFragment = DontFragment, Ttl = TimeToLive });
            }
        }
    }
}