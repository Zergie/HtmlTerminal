using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SimpleTerminal
{
    public partial class Form1 : Form
    {
        Process process;

        public Form1()
        {
            InitializeComponent();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                terminal1.Write($"<pre>{e.Data}</pre>");
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                terminal1.Write($"<pre style='color:red'>{e.Data}</pre>");
            }
        }

        private void terminal1_Loaded(object sender)
        {
            terminal1.Prompt = $"{Directory.GetCurrentDirectory()}>";
        }

        private void terminal1_ReadLine(object sender, string message)
        {
            try
            {
                if (message.StartsWith("cd", StringComparison.OrdinalIgnoreCase))
                {
                    Command_cd(message);
                }
                else if (string.Compare(message, ".DocumentText", true) == 0)
                {
                    terminal1.Write($"<div style='background-color: grey;'>{WebUtility.HtmlEncode(terminal1.DocumentText)}</div>");
                }
                else if (string.Compare(message, "dir", true) == 0)
                {
                    Command_dir();
                }
                else
                {
                    process = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd",
                            Arguments = $"/c {message}",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            StandardOutputEncoding = Encoding.GetEncoding(850),
                            StandardErrorEncoding = Encoding.GetEncoding(850),
                        },
                        EnableRaisingEvents = true,
                    };

                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.Exited += Process_Exited;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    terminal1.PromptVisible = false;
                }
            }
            catch (Exception e)
            {
                terminal1.Write($"<b style='color=red'>{e.ToString()}</b>");
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            process.ErrorDataReceived -= Process_ErrorDataReceived;
            process.OutputDataReceived -= Process_OutputDataReceived;
            process.Exited -= Process_Exited;
            process.Dispose();

            terminal1.PromptVisible = true;
        }

        private void Command_cd(string message)
        {
            string path = Path.GetFullPath(message.Substring(3));
            Directory.SetCurrentDirectory(path);
            terminal1.Prompt = $"{Directory.GetCurrentDirectory()}>";
        }

        private void Command_dir()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<table>");

            foreach (var d in Directory.GetDirectories(Directory.GetCurrentDirectory()))
            {
                sb.AppendLine("<tr><td valign='middle'>");

                sb.AppendLine("<svg style='height: 32px; width: 32px;' viewBox='0 0 32 32'>");
                sb.AppendLine("<path d='M14 4l4 4h14v22h-32v-26z'></path>");
                sb.AppendLine("</svg>");

                sb.AppendLine("</td><td valign='middle'>");

                sb.AppendLine($"{Path.GetFileName(d)}");
                sb.AppendLine("</td></tr>");
            }

            foreach (var f in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                sb.AppendLine("<tr><td valign='middle'>");

                sb.AppendLine("<svg style='height: 32px; width: 32px;' viewBox='0 0 32 32'>");
                sb.AppendLine("<path d='M28.681 7.159c-0.694-0.947-1.662-2.053-2.724-3.116s-2.169-2.030-3.116-2.724c-1.612-1.182-2.393-1.319-2.841-1.319h-15.5c-1.378 0-2.5 1.121-2.5 2.5v27c0 1.378 1.122 2.5 2.5 2.5h23c1.378 0 2.5-1.122 2.5-2.5v-19.5c0-0.448-0.137-1.23-1.319-2.841zM24.543 5.457c0.959 0.959 1.712 1.825 2.268 2.543h-4.811v-4.811c0.718 0.556 1.584 1.309 2.543 2.268zM28 29.5c0 0.271-0.229 0.5-0.5 0.5h-23c-0.271 0-0.5-0.229-0.5-0.5v-27c0-0.271 0.229-0.5 0.5-0.5 0 0 15.499-0 15.5 0v7c0 0.552 0.448 1 1 1h7v19.5z'></path>");
                sb.AppendLine("</svg>");

                sb.AppendLine("</td><td valign='middle'>");

                sb.Append($"{ Path.GetFileName(f)}");

                sb.AppendLine("</td></tr>");
            }

            sb.AppendLine("</table>");

            terminal1.Write(sb.ToString());
        }


    }
}
