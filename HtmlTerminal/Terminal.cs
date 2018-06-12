using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HtmlTerminal
{
    public partial class Terminal : UserControl
    {
        Dictionary<string, Dictionary<string, string>> style = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        Queue<string> writeBuffer = new Queue<string>();

        public Terminal()
        {
            style["*"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "font-family", "Consolas" },
                { "font-size", "15px" },
                { "background-color", "#ffffff" },
                { "color", "#000000"},
                { "line-height", "normal"},
            };

            style["input"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "border", "none" },
                { "border-bottom", "2px solid red" },
                { "width", "100%" },
            };

            style["#prompt_cursor"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "display", "flex" },
            };

            style["pre"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "margin-top", "5px" },
                { "margin-bottom", "5px" },
            };

            InitializeComponent();

            Webview.DocumentText =
@"<html/>
<head><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""/></head>
<style>"
+ GenerateCss() +
@"</style>
<body>
<div id='content'>
    <div id='end'/>
</div>
    <div id='prompt_cursor'>
        <div id='prompt' style='float: left;'>$</div>
        <input id='cursor' />
    </div>
<body/>
<html/>";
        }

        private string GenerateCss()
        {
            var sb = new StringBuilder();

            foreach (var item in style)
            {
                sb.AppendLine(item.Key);
                sb.AppendLine("{");

                foreach (var setting in item.Value)
                {
                    sb.Append(setting.Key);
                    sb.Append(" : ");
                    sb.Append(setting.Value);
                    sb.AppendLine(";");
                }

                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private void Webview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Loaded?.Invoke(this);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Return)
            {
                string message = Document.GetElementById("cursor").GetAttribute("value");

                Write($"{Prompt}{message}");
                ReadLine?.Invoke(this, message);

                var textbox = Document.GetElementById("cursor");
                textbox.SetAttribute("value", string.Empty);
                textbox.ScrollIntoView(true);

                return true;
            }
            else
            {
                var textbox = Document.GetElementById("cursor");
                if (textbox != null)
                {
                    textbox.Focus();
                    textbox.ScrollIntoView(true);
                }

                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            var textbox = Document.GetElementById("cursor");
            if (textbox != null)
            {
                textbox.Focus();
                textbox.ScrollIntoView(true);
            }
        }

        public HtmlDocument Document
        {
            get
            {
                return Webview.Document;
            }
        }

        public string DocumentText
        {
            get
            {
                return $"<html>{Document.GetElementsByTagName("html")[0].InnerHtml}</html>";
            }
        }

        public string Prompt
        {
            get
            {
                if (Webview.ReadyState == WebBrowserReadyState.Complete)
                    return Document.GetElementById("prompt").InnerText;
                return null;
            }
            set
            {
                var prompt = Document.GetElementById("prompt");

                if (prompt != null)
                    prompt.InnerText = value;
            }
        }

        public bool PromptVisible
        {
            get
            {
                if (!style.ContainsKey("#prompt_cursor"))
                    return false;

                return style["#prompt_cursor"]["display"] == "flex";
            }

            set
            {
                style["#prompt_cursor"]["display"] = value ? "flex" : "none";
                UpdateCss();
            }
        }

        public override Font Font
        {
            get
            {
                if (!style.ContainsKey("*"))
                    return base.Font;

                var font_family = style["*"]["font-family"];

                var s = style["*"]["font-size"];
                var font_size = Convert.ToSingle(s.Substring(0, s.Length - 2));

                return new Font(font_family, font_size);
            }
            set
            {
                style["*"]["font-family"] = value.FontFamily.Name;
                style["*"]["font-size"] = $"{value.Size}px";

                style["input"]["font-family"] = style["*"]["font-family"];
                style["input"]["font-size"] = style["*"]["font-size"];
                UpdateCss();
            }
        }

        public override Color BackColor
        {
            get
            {
                if (!style.ContainsKey("*"))
                    return base.BackColor;

                var text = style["*"]["background-color"];
                return ColorTranslator.FromHtml(text);
            }
            set
            {
                style["*"]["background-color"] = ColorTranslator.ToHtml(value);
                UpdateCss();
            }
        }

        public override Color ForeColor
        {
            get
            {
                if (!style.ContainsKey("*"))
                    return base.BackColor;

                var text = style["*"]["color"];
                return ColorTranslator.FromHtml(text);
            }
            set
            {
                style["*"]["color"] = ColorTranslator.ToHtml(value);
                UpdateCss();
            }
        }

        public void Write(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate { Write(message); });
            }
            else
            {
                HtmlElement element = Document.CreateElement("div");
                element.InnerHtml = message;

                Document.GetElementById("end").AppendChild(element);
            }
        }

        private void UpdateCss()
        {
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { UpdateCss(); });
            else
                Document.GetElementsByTagName("style")[0].InnerHtml = GenerateCss();
        }


        public delegate void ReadLineEventHandler(object sender, string message);
        public event ReadLineEventHandler ReadLine;

        public delegate void SimpleEventHandler(object sender);
        public event SimpleEventHandler Loaded;
    }
}
