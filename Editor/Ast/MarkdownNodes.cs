using System.Collections.Generic;

namespace Warlogic.Utils.Markdown
{
    public abstract class MarkdownNode { }

    // ── Inline nodes ─────────────────────────────────────────────────────────

    public abstract class InlineNode : MarkdownNode { }

    public class TextNode : InlineNode
    {
        public string Content;
    }

    public class BoldNode : InlineNode
    {
        public List<InlineNode> Children;
    }

    public class ItalicNode : InlineNode
    {
        public List<InlineNode> Children;
    }

    public class CodeSpanNode : InlineNode
    {
        public string Content;
    }

    // ── Block nodes ───────────────────────────────────────────────────────────

    public class DocumentNode : MarkdownNode
    {
        public List<MarkdownNode> Children = new List<MarkdownNode>();
    }

    public class HeadingNode : MarkdownNode
    {
        public int Level;
        public List<InlineNode> Inline;
    }

    public class ParagraphNode : MarkdownNode
    {
        public List<InlineNode> Inline;
    }

    public class CodeBlockNode : MarkdownNode
    {
        public string Language;
        public string Content;
    }

    public class HorizontalRuleNode : MarkdownNode { }

    public class BlankLineNode : MarkdownNode { }

    public class UnorderedListNode : MarkdownNode
    {
        public List<ListItemNode> Items = new List<ListItemNode>();
    }

    public class OrderedListNode : MarkdownNode
    {
        public List<ListItemNode> Items = new List<ListItemNode>();
        public List<string> Markers = new List<string>();
    }

    public class ListItemNode : MarkdownNode
    {
        public List<InlineNode> Inline;
    }

    public class TableNode : MarkdownNode
    {
        public List<TableRowNode> Rows = new List<TableRowNode>();
    }

    public class TableRowNode : MarkdownNode
    {
        public bool IsHeader;
        public List<TableCellNode> Cells = new List<TableCellNode>();
    }

    public class TableCellNode : MarkdownNode
    {
        public List<InlineNode> Inline;
    }
}
