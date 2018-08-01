using System;
using System.Collections.Generic;
using System.Text;

namespace XDocumentor.Markdown
{
    class MarkdownBuilder
    {
        private readonly StringBuilder _sb;

        public MarkdownBuilder()
        {
            _sb = new StringBuilder();
        }

        public void Append(string text)
        {
            _sb.Append(text);
        }

        public void AppendLine()
        {
            _sb.AppendLine();
        }

        public void AppendLine(string text)
        {
            _sb.AppendLine(text);
        }

        public void Header(int level, string text)
        {
            for (int i = 0; i < level; i++)
            {
                _sb.Append("#");
            }
            _sb.Append(" ");
            _sb.AppendLine(text);
        }

        public void HeaderWithCode(int level, string code)
        {
            for (int i = 0; i < level; i++)
            {
                _sb.Append("#");
            }
            _sb.Append(" ");
            CodeQuote(code);
            _sb.AppendLine();
        }

        public void HeaderWithLink(int level, string text, string url)
        {
            for (int i = 0; i < level; i++)
            {
                _sb.Append("#");
            }
            _sb.Append(" ");
            Link(text, url);
            _sb.AppendLine();
        }

        public void Link(string text, string url)
        {
            _sb.Append("[");
            _sb.Append(text);
            _sb.Append("]");
            _sb.Append("(");
            _sb.Append(url);
            _sb.Append(")");
        }

        public void Image(string altText, string imageUrl)
        {
            _sb.Append("!");
            Link(altText, imageUrl);
        }

        public void Code(string language, string code)
        {
            _sb.Append("```");
            _sb.AppendLine(language);
            _sb.AppendLine(code);
            _sb.AppendLine("```");
        }

        public void CodeQuote(string code)
        {
            _sb.Append("`");
            _sb.Append(code);
            _sb.Append("`");
        }

        public void Table(string[] headers, IEnumerable<string[]> items, bool indent = false)
        {
            string tableIndent = indent ? "   " : "";

            _sb.Append(tableIndent + "| ");
            foreach (var item in headers)
            {
                _sb.Append(item);
                _sb.Append(" | ");
            }
            _sb.AppendLine();

            _sb.Append(tableIndent + "| ");
            foreach (var item in headers)
            {
                _sb.Append("---");
                _sb.Append(" | ");
            }
            _sb.AppendLine();


            foreach (var item in items)
            {
                _sb.Append(tableIndent + "| ");
                foreach (var item2 in item)
                {
                    _sb.Append(item2);
                    _sb.Append(" | ");
                }
                _sb.AppendLine();
            }
            _sb.AppendLine();
        }

        public void List(string text)
        {
            _sb.Append("- ");
            _sb.AppendLine(text);
        }
        
        public void ListLink(string text, string url)
        {
            _sb.Append("- ");
            Link(text, url);
            _sb.AppendLine();
        }

        public void HorizontalRule()
        {
            _sb.Append("---");
            _sb.AppendLine();
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
