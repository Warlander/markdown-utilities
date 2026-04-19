using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Warlogic.Utils.Markdown
{
    public class MarkdownBlockParser : IMarkdownBlockParser
    {
        private readonly IMarkdownInlineParser _inlineParser;

        public MarkdownBlockParser(IMarkdownInlineParser inlineParser)
        {
            _inlineParser = inlineParser;
        }

        public DocumentNode Parse(LexerLine[] lines)
        {
            var doc = new DocumentNode();
            int i = 0;

            while (i < lines.Length)
            {
                LexerLine line = lines[i];

                switch (line.Type)
                {
                    case LineType.CodeFence:
                        i = ParseCodeBlock(lines, i, doc);
                        break;

                    case LineType.TableRow:
                        i = ParseTable(lines, i, doc);
                        break;

                    case LineType.UnorderedListItem:
                        i = ParseUnorderedList(lines, i, doc);
                        break;

                    case LineType.OrderedListItem:
                        i = ParseOrderedList(lines, i, doc);
                        break;

                    case LineType.Heading:
                        doc.Children.Add(new HeadingNode
                        {
                            Level = line.HeadingLevel,
                            Inline = _inlineParser.Parse(line.Content)
                        });
                        i++;
                        break;

                    case LineType.HorizontalRule:
                        doc.Children.Add(new HorizontalRuleNode());
                        i++;
                        break;

                    case LineType.Blank:
                        doc.Children.Add(new BlankLineNode());
                        i++;
                        break;

                    default: // Text
                        doc.Children.Add(new ParagraphNode
                        {
                            Inline = _inlineParser.Parse(line.Content)
                        });
                        i++;
                        break;
                }
            }

            return doc;
        }

        private int ParseCodeBlock(LexerLine[] lines, int start, DocumentNode doc)
        {
            string language = lines[start].Content;
            var codeLines = new List<string>();
            int i = start + 1;

            while (i < lines.Length)
            {
                if (lines[i].Type == LineType.CodeFence)
                {
                    i++;
                    break;
                }
                codeLines.Add(lines[i].Raw);
                i++;
            }

            doc.Children.Add(new CodeBlockNode
            {
                Language = language,
                Content = string.Join("\n", codeLines)
            });
            return i;
        }

        private int ParseTable(LexerLine[] lines, int start, DocumentNode doc)
        {
            var rawRows = new List<string>();
            int i = start;

            while (i < lines.Length && lines[i].Type == LineType.TableRow)
            {
                rawRows.Add(lines[i].Trimmed);
                i++;
            }

            var table = new TableNode();

            for (int rowIdx = 0; rowIdx < rawRows.Count; rowIdx++)
            {
                var cells = SplitTableRow(rawRows[rowIdx]);
                if (cells.Count == 0)
                    continue;
                if (IsSeparatorRow(cells))
                    continue;

                bool isHeader = rowIdx == 0;
                var row = new TableRowNode { IsHeader = isHeader };

                foreach (string cellText in cells)
                    row.Cells.Add(new TableCellNode { Inline = _inlineParser.Parse(cellText) });

                table.Rows.Add(row);
            }

            doc.Children.Add(table);
            return i;
        }

        private int ParseUnorderedList(LexerLine[] lines, int start, DocumentNode doc)
        {
            var list = new UnorderedListNode();
            int i = start;

            while (i < lines.Length && lines[i].Type == LineType.UnorderedListItem)
            {
                list.Items.Add(new ListItemNode { Inline = _inlineParser.Parse(lines[i].Content) });
                i++;
            }

            doc.Children.Add(list);
            return i;
        }

        private int ParseOrderedList(LexerLine[] lines, int start, DocumentNode doc)
        {
            var list = new OrderedListNode();
            int i = start;

            while (i < lines.Length && lines[i].Type == LineType.OrderedListItem)
            {
                list.Items.Add(new ListItemNode { Inline = _inlineParser.Parse(lines[i].Content) });
                list.Markers.Add(lines[i].OrderedMarker);
                i++;
            }

            doc.Children.Add(list);
            return i;
        }

        private static List<string> SplitTableRow(string line)
        {
            var parts = line.Split('|');
            var cells = new List<string>();
            for (int i = 1; i < parts.Length - 1; i++)
                cells.Add(parts[i].Trim());
            return cells;
        }

        private static bool IsSeparatorRow(List<string> cells)
            => cells.TrueForAll(c => Regex.IsMatch(c, @"^[-: ]+$"));
    }
}
