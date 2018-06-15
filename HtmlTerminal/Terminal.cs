using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace HtmlTerminal
{
    public partial class Terminal : UserControl
    {
        Dictionary<string, Dictionary<string, string>> style = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

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
<style id='style'>"
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
            switch (keyData)
            {
                case Keys.Return:
                    try
                    {
                        string text = Document.GetElementById("cursor").GetAttribute("value");

                        Write($"{Prompt}{text}");

                        var args = CommandCollection.SplitArguments(text);

                        if (args.Length > 0)
                        {
                            if (!Commands.Contains(args[0]))
                                throw new CommandNotFoundException(args);

                            Commands[args[0]](args);
                        }
                    }
                    catch (Exception e)
                    {
                        if (CommandException == null)
                            throw;
                        CommandException(this, e);
                    }
                    finally
                    {
                        var textbox = Document.GetElementById("cursor");
                        textbox.SetAttribute("value", string.Empty);
                        ScrollToCursor();
                    }
                    return true;

                case Keys.Tab:
                    {
                        string text = Document.GetElementById("cursor").GetAttribute("value");

                        if (!text.Contains(" "))
                        {
                            //autocomplete command
                            var suggestions = Commands.CompleteCommand(text);

                            if (suggestions.Count == 1)
                            {
                                Document.GetElementById("cursor").SetAttribute("value", suggestions[0]);
                            }
                            else
                            {
                                Write($"{Prompt}{text}");
                                Write(string.Join("<br/>", suggestions));
                            }

                            ScrollToCursor();
                        }
                        else
                        {
                            //autocomplete argument
                            //todo
                        }

                        return true;
                    }

                default:
                    {
                        ScrollToCursor();
                        return base.ProcessCmdKey(ref msg, keyData);
                    }
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            var textbox = Document.GetElementById("cursor");
            if (textbox != null)
            {
                textbox.Focus();
                textbox.ScrollIntoView(false);
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
                var html = Document.GetElementsByTagName("html");
                if (html.Count > 0)
                    return $"<html>{html[0].InnerHtml}</html>";

                return string.Empty;
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

                if (value)
                {
                    if (InvokeRequired)
                        BeginInvoke((MethodInvoker)delegate { ScrollToCursor(); });
                    else
                        ScrollToCursor();
                }
            }
        }

        private void ScrollToCursor()
        {
            var textbox = Document.GetElementById("cursor");
            if (textbox != null)
            {
                textbox.ScrollIntoView(false);
                textbox.Focus();
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


        public CommandCollection Commands { get; } = new CommandCollection(StringComparer.OrdinalIgnoreCase);


        public void WriteText(string text)
        {
            Write($"<pre>{WebUtility.HtmlEncode(text)}</pre>");
        }

        public void WriteText(string text, Color color)
        {
            Write($"<pre style='color:{ColorTranslator.ToHtml(color)}'>{WebUtility.HtmlEncode(text)}</pre>");
        }

        public void WriteText(string text, Color color, FontStyle style = FontStyle.Regular)
        {
            var css = new StringBuilder();
            css.Append($"color:{ColorTranslator.ToHtml(color)};");

            if (style.HasFlag(FontStyle.Bold))
                css.Append("font-weight: bold;");

            if (style.HasFlag(FontStyle.Italic))
                css.Append("font-style: italic;");

            if (style.HasFlag(FontStyle.Underline) && style.HasFlag(FontStyle.Strikeout))
                css.Append("text-decoration: underline line-through;");
            else if (style.HasFlag(FontStyle.Underline))
                css.Append("text-decoration: underline;");
            else if (style.HasFlag(FontStyle.Strikeout))
                css.Append("text-decoration: line-through;");


            Write($"<pre style='{css.ToString()}'>{WebUtility.HtmlEncode(text)}</pre>");
        }

        public void Write(string html)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate { Write(html); });
            }
            else
            {
                HtmlElement element = Document.CreateElement("div");
                element.InnerHtml = html;

                var endElement = Document.GetElementById("end");

                if (endElement != null)
                {
                    endElement.AppendChild(element);
                    endElement.ScrollIntoView(false);
                }
            }
        }

        private void UpdateCss()
        {
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { UpdateCss(); });
            else
            {
                var element = Document.GetElementById("style");

                if (element != null)
                    element.InnerHtml = GenerateCss();
            }
        }



        public delegate void CommandExceptionEventHandler(object sender, Exception e);
        public event CommandExceptionEventHandler CommandException;

        public delegate void SimpleEventHandler(object sender);
        public event SimpleEventHandler Loaded;
    }
}
