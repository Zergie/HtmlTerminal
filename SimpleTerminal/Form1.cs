using HtmlTerminal;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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

            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd",
                    Arguments = $"/c help",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.GetEncoding(850),
                };

                p.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        int i = e.Data.IndexOf(' ');

                        if (i > 0 && e.Data.Length != i && e.Data[i + 1] == ' ')
                            terminal1.Commands.Add(e.Data.Substring(0, i), new Command(Command_Other));
                    }
                };

                p.Exited += (sender, e) =>
                {
                    Process process = (Process)sender;
                    process.Dispose();
                };

                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }

            terminal1.Commands["cd"] = new CommandChangeDir();
            terminal1.Commands["dir"] = new CommandListDir();
            terminal1.Commands["ws"] = new Command(Command_writeStyle);
            terminal1.Commands["wc"] = new Command(Command_writeColor);
            terminal1.Commands["ping"] = new CommandPing();
        }

        private void Command_writeStyle(Terminal sender, string[] args)
        {
            Color color = Color.FromName(args[1]);
            FontStyle style = (FontStyle)int.Parse(args[2]);

            terminal1.WriteText(string.Join(" ", args.Skip(3)), color, style);
        }

        private void Command_writeColor(Terminal sender, string[] args)
        {
            Color color = Color.FromName(args[1]);
            terminal1.WriteText(string.Join(" ", args.Skip(2)), color);
        }

        private void Command_Other(Terminal sender, string[] args)
        {
            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c \"{string.Join("\"", args)}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.GetEncoding(850),
                    StandardErrorEncoding = Encoding.GetEncoding(850),
                },
                EnableRaisingEvents = true,
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    terminal1.WriteText(e.Data, Color.Red);
            };
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    terminal1.WriteText(e.Data);
            };
            process.Exited += (s, e) =>
            {
                terminal1.PromptVisible = true;
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            terminal1.PromptVisible = false;
        }

        private void terminal1_Loaded(object sender)
        {
            terminal1.Prompt = $"{Directory.GetCurrentDirectory()}>";
        }

        private void terminal1_CommandException(object sender, System.Exception e)
        {
            if (e is CommandNotFoundException)
                terminal1.WriteText(e.Message, Color.Red, FontStyle.Bold);
            else
                terminal1.WriteText(e.ToString(), Color.Red, FontStyle.Bold);
        }

        private void copyHtmlToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Clipboard.SetText(terminal1.DocumentText);
        }
    }
}
