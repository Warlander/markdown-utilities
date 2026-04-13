using System.Collections.Generic;
using System.Text;

namespace Warlogic.Utils.Markdown
{
    public class MarkdownInlineParser : IMarkdownInlineParser
    {
        public List<InlineNode> Parse(string text)
        {
            var nodes = new List<InlineNode>();
            if (string.IsNullOrEmpty(text))
                return nodes;

            ParseInto(text, 0, text.Length, nodes);
            return nodes;
        }

        private void ParseInto(string text, int start, int end, List<InlineNode> nodes)
        {
            var buffer = new StringBuilder();
            int i = start;

            while (i < end)
            {
                // Code span: `...`
                if (text[i] == '`')
                {
                    FlushBuffer(buffer, nodes);
                    int closeIdx = text.IndexOf('`', i + 1);
                    if (closeIdx < 0 || closeIdx >= end)
                    {
                        buffer.Append(text[i]);
                        i++;
                        continue;
                    }
                    nodes.Add(new CodeSpanNode { Content = text.Substring(i + 1, closeIdx - i - 1) });
                    i = closeIdx + 1;
                    continue;
                }

                // Bold: **...**
                if (i + 1 < end && text[i] == '*' && text[i + 1] == '*')
                {
                    int closeIdx = text.IndexOf("**", i + 2, System.StringComparison.Ordinal);
                    if (closeIdx >= 0 && closeIdx < end)
                    {
                        FlushBuffer(buffer, nodes);
                        var inner = new List<InlineNode>();
                        ParseInto(text, i + 2, closeIdx, inner);
                        nodes.Add(new BoldNode { Children = inner });
                        i = closeIdx + 2;
                        continue;
                    }
                }

                // Italic: *...*  (only when not **)
                if (text[i] == '*')
                {
                    int closeIdx = FindItalicClose(text, i + 1, end);
                    if (closeIdx >= 0)
                    {
                        FlushBuffer(buffer, nodes);
                        var inner = new List<InlineNode>();
                        ParseInto(text, i + 1, closeIdx, inner);
                        nodes.Add(new ItalicNode { Children = inner });
                        i = closeIdx + 1;
                        continue;
                    }
                }

                buffer.Append(text[i]);
                i++;
            }

            FlushBuffer(buffer, nodes);
        }

        // Find the closing * for italic, skipping ** pairs.
        private int FindItalicClose(string text, int from, int end)
        {
            int i = from;
            while (i < end)
            {
                if (text[i] == '*')
                {
                    // Skip ** pairs inside
                    if (i + 1 < end && text[i + 1] == '*')
                    {
                        int innerClose = text.IndexOf("**", i + 2, System.StringComparison.Ordinal);
                        if (innerClose >= 0 && innerClose < end)
                        {
                            i = innerClose + 2;
                            continue;
                        }
                    }
                    return i;
                }
                i++;
            }
            return -1;
        }

        private void FlushBuffer(StringBuilder buffer, List<InlineNode> nodes)
        {
            if (buffer.Length == 0)
                return;
            nodes.Add(new TextNode { Content = buffer.ToString() });
            buffer.Clear();
        }
    }
}
