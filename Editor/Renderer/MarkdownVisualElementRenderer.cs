using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Warlogic.Utils.Markdown
{
    public class MarkdownVisualElementRenderer : IMarkdownVisualElementRenderer
    {
        private static readonly Color CodeBackground = new Color(0f, 0f, 0f, 0.15f);
        private static readonly Color BorderColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        private static readonly Color HeaderRowBackground = new Color(0f, 0f, 0f, 0.2f);
        private static readonly Color AlternateRowBackground = new Color(0f, 0f, 0f, 0.05f);

        public VisualElement Render(DocumentNode doc)
        {
            var root = new VisualElement();
            root.style.flexGrow = 1;

            foreach (MarkdownNode node in doc.Children)
                root.Add(RenderBlock(node));

            return root;
        }

        private VisualElement RenderBlock(MarkdownNode node)
        {
            switch (node)
            {
                case HeadingNode heading:       return RenderHeading(heading);
                case ParagraphNode paragraph:   return RenderParagraph(paragraph);
                case CodeBlockNode code:        return RenderCodeBlock(code);
                case HorizontalRuleNode _:      return RenderHorizontalRule();
                case BlankLineNode _:           return RenderSpacer();
                case UnorderedListNode ulist:   return RenderUnorderedList(ulist);
                case OrderedListNode olist:     return RenderOrderedList(olist);
                case TableNode table:           return RenderTable(table);
                default:                        return new VisualElement();
            }
        }

        private VisualElement RenderHeading(HeadingNode node)
        {
            int fontSize = node.Level == 1 ? 22 : node.Level == 2 ? 18 : 15;
            var label = new Label(InlineToPlainText(node.Inline));
            label.enableRichText = false;
            label.style.fontSize = fontSize;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginTop = 16;
            label.style.marginBottom = 4;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        private VisualElement RenderParagraph(ParagraphNode node)
        {
            var el = RenderInline(node.Inline);
            el.style.marginBottom = 4;
            return el;
        }

        private VisualElement RenderCodeBlock(CodeBlockNode node)
        {
            var wrapper = new VisualElement();
            wrapper.style.backgroundColor = CodeBackground;
            wrapper.style.paddingTop = 8;
            wrapper.style.paddingBottom = 8;
            wrapper.style.paddingLeft = 8;
            wrapper.style.paddingRight = 8;
            wrapper.style.borderTopLeftRadius = 4;
            wrapper.style.borderTopRightRadius = 4;
            wrapper.style.borderBottomLeftRadius = 4;
            wrapper.style.borderBottomRightRadius = 4;
            wrapper.style.marginTop = 4;
            wrapper.style.marginBottom = 8;

            var label = new Label(node.Content);
            label.enableRichText = false;
            label.style.whiteSpace = WhiteSpace.Pre;
            wrapper.Add(label);

            return wrapper;
        }

        private VisualElement RenderHorizontalRule()
        {
            var hr = new VisualElement();
            hr.style.height = 1;
            hr.style.marginTop = 8;
            hr.style.marginBottom = 8;
            hr.style.backgroundColor = BorderColor;
            return hr;
        }

        private VisualElement RenderSpacer()
        {
            var spacer = new VisualElement();
            spacer.style.height = 8;
            return spacer;
        }

        private VisualElement RenderUnorderedList(UnorderedListNode node)
        {
            var container = new VisualElement();
            foreach (ListItemNode item in node.Items)
                container.Add(RenderListItem("•", item));
            return container;
        }

        private VisualElement RenderOrderedList(OrderedListNode node)
        {
            var container = new VisualElement();
            for (int i = 0; i < node.Items.Count; i++)
                container.Add(RenderListItem(node.Markers[i], node.Items[i]));
            return container;
        }

        private VisualElement RenderListItem(string bullet, ListItemNode item)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 2;
            row.style.marginLeft = 8;

            var bulletLabel = new Label(bullet);
            bulletLabel.enableRichText = false;
            bulletLabel.style.minWidth = 16;
            bulletLabel.style.marginRight = 4;
            row.Add(bulletLabel);

            var content = RenderInline(item.Inline);
            content.style.flexShrink = 1;
            content.style.flexGrow = 1;
            row.Add(content);

            return row;
        }

        private VisualElement RenderTable(TableNode node)
        {
            var wrapper = new VisualElement();
            wrapper.style.marginTop = 8;
            wrapper.style.marginBottom = 8;
            wrapper.style.borderTopWidth = 1;
            wrapper.style.borderBottomWidth = 1;
            wrapper.style.borderLeftWidth = 1;
            wrapper.style.borderRightWidth = 1;
            wrapper.style.borderTopColor = BorderColor;
            wrapper.style.borderBottomColor = BorderColor;
            wrapper.style.borderLeftColor = BorderColor;
            wrapper.style.borderRightColor = BorderColor;
            wrapper.style.borderTopLeftRadius = 4;
            wrapper.style.borderTopRightRadius = 4;
            wrapper.style.borderBottomLeftRadius = 4;
            wrapper.style.borderBottomRightRadius = 4;
            wrapper.style.overflow = Overflow.Hidden;

            bool alternateData = false;
            int visualRowIndex = 0;

            foreach (TableRowNode tableRow in node.Rows)
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;

                if (tableRow.IsHeader)
                    row.style.backgroundColor = HeaderRowBackground;
                else if (alternateData)
                    row.style.backgroundColor = AlternateRowBackground;

                if (!tableRow.IsHeader)
                    alternateData = !alternateData;

                for (int colIndex = 0; colIndex < tableRow.Cells.Count; colIndex++)
                {
                    var cell = new VisualElement();
                    cell.style.flexGrow = 1;
                    cell.style.flexBasis = 0;
                    cell.style.paddingTop = 4;
                    cell.style.paddingBottom = 4;
                    cell.style.paddingLeft = 6;
                    cell.style.paddingRight = 6;

                    if (colIndex < tableRow.Cells.Count - 1)
                    {
                        cell.style.borderRightWidth = 1;
                        cell.style.borderRightColor = BorderColor;
                    }

                    if (visualRowIndex > 0)
                    {
                        cell.style.borderTopWidth = 1;
                        cell.style.borderTopColor = BorderColor;
                    }

                    var content = RenderInline(tableRow.Cells[colIndex].Inline);
                    if (tableRow.IsHeader)
                        content.style.unityFontStyleAndWeight = FontStyle.Bold;
                    cell.Add(content);
                    row.Add(cell);
                }

                wrapper.Add(row);
                visualRowIndex++;
            }

            return wrapper;
        }

        private VisualElement RenderInline(List<InlineNode> nodes)
        {
            bool hasCodeSpan = nodes.Exists(n => n is CodeSpanNode);

            if (!hasCodeSpan)
            {
                var label = new Label(InlineToRichText(nodes));
                label.enableRichText = true;
                label.style.whiteSpace = WhiteSpace.Normal;
                return label;
            }

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;

            foreach (InlineNode node in nodes)
            {
                if (node is CodeSpanNode cs)
                {
                    row.Add(RenderCodeSpan(cs.Content));
                }
                else
                {
                    string rich = InlineNodeToRichText(node);
                    if (rich.Length > 0)
                    {
                        var label = new Label(rich);
                        label.enableRichText = true;
                        label.style.whiteSpace = WhiteSpace.Normal;
                        row.Add(label);
                    }
                }
            }

            return row;
        }

        private VisualElement RenderCodeSpan(string code)
        {
            var wrapper = new VisualElement();
            wrapper.style.backgroundColor = CodeBackground;
            wrapper.style.paddingLeft = 3;
            wrapper.style.paddingRight = 3;
            wrapper.style.borderTopLeftRadius = 3;
            wrapper.style.borderTopRightRadius = 3;
            wrapper.style.borderBottomLeftRadius = 3;
            wrapper.style.borderBottomRightRadius = 3;

            var label = new Label(code);
            label.enableRichText = false;
            wrapper.Add(label);

            return wrapper;
        }

        private string InlineToRichText(List<InlineNode> nodes)
        {
            var sb = new System.Text.StringBuilder();
            foreach (InlineNode node in nodes)
                sb.Append(InlineNodeToRichText(node));
            return sb.ToString();
        }

        private string InlineNodeToRichText(InlineNode node)
        {
            switch (node)
            {
                case TextNode t:      return t.Content;
                case BoldNode b:      return "<b>" + InlineToRichText(b.Children) + "</b>";
                case ItalicNode it:   return "<i>" + InlineToRichText(it.Children) + "</i>";
                case CodeSpanNode cs: return cs.Content;
                default:              return string.Empty;
            }
        }

        private string InlineToPlainText(List<InlineNode> nodes)
        {
            var sb = new System.Text.StringBuilder();
            foreach (InlineNode node in nodes)
                sb.Append(InlineNodeToPlainText(node));
            return sb.ToString();
        }

        private string InlineNodeToPlainText(InlineNode node)
        {
            switch (node)
            {
                case TextNode t:      return t.Content;
                case BoldNode b:      return InlineToPlainText(b.Children);
                case ItalicNode it:   return InlineToPlainText(it.Children);
                case CodeSpanNode cs: return cs.Content;
                default:              return string.Empty;
            }
        }
    }
}
