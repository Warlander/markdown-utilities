using System.Text.RegularExpressions;

namespace Warlogic.Utils.Markdown
{
    public enum LineType
    {
        Blank,
        Heading,
        HorizontalRule,
        CodeFence,
        TableRow,
        UnorderedListItem,
        OrderedListItem,
        Text
    }

    public readonly struct LexerLine
    {
        public readonly LineType Type;
        public readonly string Raw;
        public readonly string Trimmed;
        public readonly string Content;
        public readonly int HeadingLevel;
        public readonly string OrderedMarker;

        private LexerLine(LineType type, string raw, string trimmed, string content,
            int headingLevel = 0, string orderedMarker = null)
        {
            Type = type;
            Raw = raw;
            Trimmed = trimmed;
            Content = content;
            HeadingLevel = headingLevel;
            OrderedMarker = orderedMarker;
        }

        public static LexerLine Classify(string raw)
        {
            string trimmed = raw.TrimEnd();

            if (trimmed.Length == 0)
                return new LexerLine(LineType.Blank, raw, trimmed, string.Empty);

            if (trimmed.StartsWith("```"))
                return new LexerLine(LineType.CodeFence, raw, trimmed,
                    trimmed.Length > 3 ? trimmed.Substring(3).Trim() : string.Empty);

            if (trimmed == "---" || trimmed == "***" || trimmed == "___")
                return new LexerLine(LineType.HorizontalRule, raw, trimmed, string.Empty);

            if (trimmed.StartsWith("### "))
                return new LexerLine(LineType.Heading, raw, trimmed, trimmed.Substring(4), headingLevel: 3);

            if (trimmed.StartsWith("## "))
                return new LexerLine(LineType.Heading, raw, trimmed, trimmed.Substring(3), headingLevel: 2);

            if (trimmed.StartsWith("# "))
                return new LexerLine(LineType.Heading, raw, trimmed, trimmed.Substring(2), headingLevel: 1);

            if (trimmed.StartsWith("|"))
                return new LexerLine(LineType.TableRow, raw, trimmed, trimmed);

            if (trimmed.StartsWith("- ") || trimmed.StartsWith("* "))
                return new LexerLine(LineType.UnorderedListItem, raw, trimmed, trimmed.Substring(2));

            var orderedMatch = Regex.Match(trimmed, @"^(\d+)\. (.+)$");
            if (orderedMatch.Success)
                return new LexerLine(LineType.OrderedListItem, raw, trimmed,
                    orderedMatch.Groups[2].Value, orderedMarker: orderedMatch.Groups[1].Value + ".");

            return new LexerLine(LineType.Text, raw, trimmed, trimmed);
        }
    }

    public class MarkdownLexer : IMarkdownLexer
    {
        private static readonly string[] LineSeparators = { "\r\n", "\n" };

        public LexerLine[] Tokenize(string text)
        {
            string[] rawLines = text.Split(LineSeparators, System.StringSplitOptions.None);
            var result = new LexerLine[rawLines.Length];
            for (int i = 0; i < rawLines.Length; i++)
                result[i] = LexerLine.Classify(rawLines[i]);
            return result;
        }
    }
}
