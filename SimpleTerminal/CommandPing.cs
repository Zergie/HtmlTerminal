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
                if (string.Compare(args[i], "-n", true) == 0)
                {
                    i++;
                    var maxPakets = int.Parse(args[i]);

                    i++;
                    string ipaddr = args[i];

                    var p = new PingHelper(args[i], terminal, maxPakets);
                    p.SendPingAsync();
                }
                else
                {
                    var p = new PingHelper(args[i], terminal);
                    p.SendPingAsync();
                }
            }
        }

        private class PingHelper
        {
            internal PingHelper(string hostNameOrAddress, Terminal terminal, int maxPackets = 10)
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

                MaxPackets = maxPackets;

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
            private int MaxPackets { get; }
            private Ping Ping { get; }


            private void PingCompleted(object sender, PingCompletedEventArgs e)
            {
                Count++;
                div.Style = $"height:20px; background-color:green; width:{100 * Count / MaxPackets}%";

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

                if (Count == MaxPackets)
                {
                    double lost_percent = (double)LostCount / Count;

                    container.InnerHtml = "<div style='background-color: red;position: absolute;width: 100px;height: 100px;border-radius: 50px;box-shadow: 0px 0px 0px 1px;'></div>" +
                                        $"<div style='transform: rotate({-90 + (lost_percent > .5 ? (lost_percent - .5) * 360 : 0) }deg);transform-origin: bottom;background-color: green;position: absolute;width: 100px;height: 50px;border-radius: 50px 50px 0 0;'></div>" +
                                        $"<div style='transform: rotate({ 90 + (lost_percent < .5 ? lost_percent * 360 : 0) }deg);transform-origin: bottom;background-color: { (lost_percent < .5 ? "green" : "red") };position: absolute;width: 100px;height: 50px;border-radius: 50px 50px 0 0;'></div>" +
                                        $"<div style='position: absolute; background-color: rgba(0,0,0,0); height: 100px;display: flex;align-items: center;'><span style='text-align: center; width: 100px; background-color: rgba(0,0,0,0);'>{ (lost_percent * 100).ToString("0") } % lost</span></div>" +
                                        $"<table style='display: block;padding-left: 120px;padding-top: 5px;'>" +
                                        $"<tr><td style='padding: 0px;'>{Count}</td>              <td style='padding: 0px 0px 0px 4px;'>packet(s) send</td></tr>" +
                                        $"<tr><td style='padding: 0px;'>{RoundtripTime.Count}</td><td style='padding: 0px 0px 0px 4px;'>packet(s) received</td></tr>" +
                                        $"<tr><td style='padding: 0px;'>{LostCount}</td>          <td style='padding: 0px 0px 0px 4px;'>packet(s) lost</td></tr>" +
                                        $"</table>" +
                                        $"<span style='display: block;padding-left: 120px;font-weight: bold;'>RoundtripTime:</span>" +
                                        $"<span style='display: block;padding-left: 120px;'>average {RoundtripTime.Average(): 0.0} ms, min {RoundtripTime.Min(): 0.0} ms, max {RoundtripTime.Max(): 0.0} ms</span>";
                    container.Style = @"height: 100px;";

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
                Ping.SendPingAsync(HostNameOrAddress, 1000);
            }
        }
    }
}