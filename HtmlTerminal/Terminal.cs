using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace HtmlTerminal
{
    public enum EnumListDisplay
    {
        SimpleList,
        Table2Columns = 0x12,
        Table3Columns = 0x13,
        Table4Columns = 0x14,
        Table5Columns = 0x15,
    }

    public partial class Terminal : UserControl
    {
        public Terminal()
        {
            Style["*"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "font-family", "Consolas" },
                { "font-size", "15px" },
                { "background-color", "#ffffff" },
                { "color", "#000000"},
                { "line-height", "normal"},
            };

            Style["input"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "border", "none" },
                { "border-bottom", "2px solid red" },
                { "width", "100%" },
                { "caret-color",  "red" },
            };

            Style["#end"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "width", "100%" },
            };

            Style["#prompt_cursor"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "display", "flex" },
            };

            Style["pre"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "margin-top", "5px" },
                { "margin-bottom", "5px" },
                { "white-space", "pre-wrap"},
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

            foreach (var item in Style)
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

        private void ScrollToCursor()
        {
            var textbox = Document.GetElementById("cursor");
            if (textbox != null)
            {
                textbox.ScrollIntoView(false);
                textbox.Focus();
            }
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

                            Commands[args[0]].Run(this, args);
                        }
                    }
                    catch (Exception e)
                    {
                        if (CommandException == null)
                            WriteText(e.ToString(), Color.Red, FontStyle.Bold);
                        else
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

                            if (suggestions.Count() == 1)
                            {
                                Document.GetElementById("cursor").SetAttribute("value", suggestions.First() + " ");
                            }
                            else
                            {
                                Write($"{Prompt}{text}");
                                Write(suggestions);
                            }
                        }
                        else
                        {
                            //autocomplete argument
                            var args = CommandCollection.SplitArguments(text);

                            if (args.Length > 0)
                            {
                                int argi = args.Length - 1;

                                if (text.EndsWith(" ")) argi++;

                                if (Commands.Contains(args[0]))
                                {
                                    var suggestions = from s in Commands[args[0]].GetSuggestions(argi, args)
                                                      where s.Text.StartsWith(args[args.Length - 1], StringComparison.OrdinalIgnoreCase)
                                                      select s;
                                    if (suggestions.Count() == 1)
                                    {
                                        if (argi == args.Length - 1)
                                            text = text.Substring(0, text.Length - args[args.Length - 1].Length) + suggestions.First().Text;
                                        else
                                            text = text + suggestions.First().Text;

                                        Document.GetElementById("cursor").SetAttribute("value", text + " ");
                                    }
                                    else
                                    {
                                        Write($"{Prompt}{text}?");
                                        Write(suggestions);
                                    }
                                }
                            }
                        }

                        ScrollToCursor();
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





        private HtmlDocument Document
        {
            get => Webview.Document;
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

        StyleCollection Style { get; } = new StyleCollection();

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
                if (!Style.ContainsKey("#prompt_cursor"))
                    return false;

                return Style["#prompt_cursor"]["display"] == "flex";
            }

            set
            {
                Style["#prompt_cursor"]["display"] = value ? "flex" : "none";
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

        public override Font Font
        {
            get
            {
                if (!Style.ContainsKey("*"))
                    return base.Font;

                var font_family = Style["*"]["font-family"];

                var s = Style["*"]["font-size"];
                var font_size = Convert.ToSingle(s.Substring(0, s.Length - 2));

                return new Font(font_family, font_size);
            }
            set
            {
                Style["*"]["font-family"] = value.FontFamily.Name;
                Style["*"]["font-size"] = $"{value.Size}px";

                Style["input"]["font-family"] = Style["*"]["font-family"];
                Style["input"]["font-size"] = Style["*"]["font-size"];
                UpdateCss();
            }
        }

        public override Color BackColor
        {
            get
            {
                if (!Style.ContainsKey("*"))
                    return base.BackColor;

                var text = Style["*"]["background-color"];
                return ColorTranslator.FromHtml(text);
            }
            set
            {
                Style["*"]["background-color"] = ColorTranslator.ToHtml(value);
                UpdateCss();
            }
        }

        public override Color ForeColor
        {
            get
            {
                if (!Style.ContainsKey("*"))
                    return base.BackColor;

                var text = Style["*"]["color"];
                return ColorTranslator.FromHtml(text);
            }
            set
            {
                Style["*"]["color"] = ColorTranslator.ToHtml(value);
                Style["input"]["caret-color"] = ColorTranslator.ToHtml(value);
                UpdateCss();
            }
        }

        public EnumListDisplay SuggestionDisplayFormat { get; set; } = EnumListDisplay.SimpleList;

        public override ContextMenuStrip ContextMenuStrip
        {
            get => base.ContextMenuStrip;
            set
            {
                Webview.ContextMenuStrip = value;
                base.ContextMenuStrip = value;
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

                Write(element);
            }
        }

        public void Write(IEnumerable<object> list)
        {
            if (list == null)
            {
                WriteText("no suggestions found", Color.Red, FontStyle.Bold);
            }
            else
            {
                switch (SuggestionDisplayFormat)
                {
                    case EnumListDisplay.SimpleList:
                        Write(string.Join("<br/>", list));
                        break;

                    case EnumListDisplay.Table5Columns:
                    case EnumListDisplay.Table4Columns:
                    case EnumListDisplay.Table3Columns:
                    case EnumListDisplay.Table2Columns:
                        int div = ((int)SuggestionDisplayFormat) & 0x0f;

                        var sb = new StringBuilder();
                        sb.Append("<table style='width:100%;table-layout: fixed;'>");
                        sb.Append($"<tr>");

                        int i = 0;
                        foreach (var item in list)
                        {
                            sb.Append($"<td>{item}</td>");

                            if ((i % div) == (div - 1))
                                sb.Append($"</tr><tr>");

                            i++;
                        }

                        sb.Append("</tr>");
                        sb.Append("</table>");
                        Write(sb.ToString());
                        break;
                }

            }
        }

        public void Write(HtmlElement element)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate { Write(element); });
            }
            else
            {
                var endElement = Document.GetElementById("end");

                if (endElement != null)
                {
                    endElement.AppendChild(element);
                    endElement.ScrollIntoView(false);
                }
            }
        }





        public HtmlElement CreateElement(string elementTag)
        {
            return Document.CreateElement(elementTag);
        }





        public delegate void CommandExceptionEventHandler(object sender, Exception e);
        public event CommandExceptionEventHandler CommandException;

        public delegate void SimpleEventHandler(object sender);
        public event SimpleEventHandler Loaded;
    }
}
