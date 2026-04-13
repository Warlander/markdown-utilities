using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Warlogic.Utils.Markdown.Tests
{
    [TestFixture]
    public class RendererTests
    {
        private MarkdownVisualElementRenderer _renderer;

        [SetUp]
        public void SetUp()
        {
            _renderer = new MarkdownVisualElementRenderer();
        }

        private static DocumentNode DocWith(MarkdownNode node)
        {
            var doc = new DocumentNode();
            doc.Children.Add(node);
            return doc;
        }

        [Test]
        public void Render_EmptyDocument_ReturnsNonNullVisualElement()
        {
            // Arrange
            var doc = new DocumentNode();

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Render_Heading_ProducesChildElement()
        {
            // Arrange
            var doc = DocWith(new HeadingNode
            {
                Level = 1,
                Inline = new List<InlineNode> { new TextNode { Content = "Title" } }
            });

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.Greater(result.childCount, 0);
        }

        [Test]
        public void Render_Paragraph_ProducesChildElement()
        {
            // Arrange
            var doc = DocWith(new ParagraphNode
            {
                Inline = new List<InlineNode> { new TextNode { Content = "text" } }
            });

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.Greater(result.childCount, 0);
        }

        [Test]
        public void Render_HorizontalRule_ProducesChildElement()
        {
            // Arrange
            var doc = DocWith(new HorizontalRuleNode());

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.Greater(result.childCount, 0);
        }

        [Test]
        public void Render_CodeBlock_ProducesChildElement()
        {
            // Arrange
            var doc = DocWith(new CodeBlockNode { Language = "csharp", Content = "var x = 1;" });

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.Greater(result.childCount, 0);
        }

        [Test]
        public void Render_UnorderedList_ProducesChildElement()
        {
            // Arrange
            var list = new UnorderedListNode();
            list.Items.Add(new ListItemNode { Inline = new List<InlineNode> { new TextNode { Content = "item" } } });
            var doc = DocWith(list);

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.Greater(result.childCount, 0);
        }

        [Test]
        public void Render_OrderedList_ProducesChildElement()
        {
            // Arrange
            var list = new OrderedListNode();
            list.Items.Add(new ListItemNode { Inline = new List<InlineNode> { new TextNode { Content = "item" } } });
            list.Markers.Add("1.");
            var doc = DocWith(list);

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.Greater(result.childCount, 0);
        }

        [Test]
        public void Render_Table_ProducesChildElement()
        {
            // Arrange
            var table = new TableNode();
            var row = new TableRowNode { IsHeader = true };
            row.Cells.Add(new TableCellNode { Inline = new List<InlineNode> { new TextNode { Content = "cell" } } });
            table.Rows.Add(row);
            var doc = DocWith(table);

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.Greater(result.childCount, 0);
        }

        [Test]
        public void Render_FullPipeline_ProducesNonNullResult()
        {
            // Arrange
            var lexer = new MarkdownLexer();
            var blockParser = new MarkdownBlockParser(new MarkdownInlineParser());
            string markdown = "# Title\n\nSome **bold** text.\n\n- item 1\n- item 2";
            DocumentNode doc = blockParser.Parse(lexer.Tokenize(markdown));

            // Act
            VisualElement result = _renderer.Render(doc);

            // Assert
            Assert.IsNotNull(result);
            Assert.Greater(result.childCount, 0);
        }
    }
}
