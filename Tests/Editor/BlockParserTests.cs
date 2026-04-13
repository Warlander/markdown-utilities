using NUnit.Framework;

namespace Warlogic.Utils.Markdown.Tests
{
    [TestFixture]
    public class BlockParserTests
    {
        private MarkdownLexer _lexer;
        private MarkdownBlockParser _parser;

        [SetUp]
        public void SetUp()
        {
            _lexer = new MarkdownLexer();
            _parser = new MarkdownBlockParser(new MarkdownInlineParser());
        }

        private DocumentNode ParseMarkdown(string markdown)
        {
            LexerLine[] lines = _lexer.Tokenize(markdown);
            return _parser.Parse(lines);
        }

        // ── Basic blocks ──────────────────────────────────────────────────────

        [Test]
        public void Parse_EmptyString_ProducesSingleBlankLineNode()
        {
            // Arrange
            string markdown = "";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            Assert.AreEqual(1, doc.Children.Count);
            Assert.IsInstanceOf<BlankLineNode>(doc.Children[0]);
        }

        [Test]
        public void Parse_BlankLine_ProducesBlankLineNode()
        {
            // Arrange
            string markdown = "\n";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            Assert.IsTrue(doc.Children.Exists(n => n is BlankLineNode));
        }

        [Test]
        public void Parse_H1_ProducesHeadingNodeWithLevel1()
        {
            // Arrange
            string markdown = "# Hello";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            Assert.AreEqual(1, doc.Children.Count);
            var heading = doc.Children[0] as HeadingNode;
            Assert.IsNotNull(heading);
            Assert.AreEqual(1, heading.Level);
        }

        [Test]
        public void Parse_H2_ProducesHeadingNodeWithLevel2()
        {
            // Arrange
            string markdown = "## Sub";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var heading = doc.Children[0] as HeadingNode;
            Assert.IsNotNull(heading);
            Assert.AreEqual(2, heading.Level);
        }

        [Test]
        public void Parse_H3_ProducesHeadingNodeWithLevel3()
        {
            // Arrange
            string markdown = "### Deep";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var heading = doc.Children[0] as HeadingNode;
            Assert.IsNotNull(heading);
            Assert.AreEqual(3, heading.Level);
        }

        [Test]
        public void Parse_Paragraph_ProducesParagraphNode()
        {
            // Arrange
            string markdown = "some text";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            Assert.AreEqual(1, doc.Children.Count);
            Assert.IsInstanceOf<ParagraphNode>(doc.Children[0]);
        }

        [Test]
        public void Parse_HorizontalRule_ProducesHorizontalRuleNode()
        {
            // Arrange
            string markdown = "---";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            Assert.AreEqual(1, doc.Children.Count);
            Assert.IsInstanceOf<HorizontalRuleNode>(doc.Children[0]);
        }

        // ── Code blocks ───────────────────────────────────────────────────────

        [Test]
        public void Parse_CodeBlockNoLanguage_ProducesCodeBlockWithEmptyLanguage()
        {
            // Arrange
            string markdown = "```\ncode\n```";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var codeBlock = doc.Children[0] as CodeBlockNode;
            Assert.IsNotNull(codeBlock);
            Assert.AreEqual("", codeBlock.Language);
        }

        [Test]
        public void Parse_CodeBlockWithLanguage_CapturesLanguageIdentifier()
        {
            // Arrange
            string markdown = "```csharp\ncode\n```";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var codeBlock = doc.Children[0] as CodeBlockNode;
            Assert.IsNotNull(codeBlock);
            Assert.AreEqual("csharp", codeBlock.Language);
        }

        [Test]
        public void Parse_CodeBlockContent_IsStoredRawWithoutInlineParsing()
        {
            // Arrange
            string markdown = "```\n**raw**\n```";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var codeBlock = doc.Children[0] as CodeBlockNode;
            Assert.IsNotNull(codeBlock);
            Assert.IsTrue(codeBlock.Content.Contains("**raw**"));
        }

        // ── Lists ─────────────────────────────────────────────────────────────

        [Test]
        public void Parse_UnorderedList_ProducesListNodeWithCorrectItemCount()
        {
            // Arrange
            string markdown = "- a\n- b";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var list = doc.Children[0] as UnorderedListNode;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Items.Count);
        }

        [Test]
        public void Parse_OrderedList_ProducesListNodeWithMarkersPerItem()
        {
            // Arrange
            string markdown = "1. first\n2. second";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var list = doc.Children[0] as OrderedListNode;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Items.Count);
            Assert.AreEqual("1.", list.Markers[0]);
            Assert.AreEqual("2.", list.Markers[1]);
        }

        // ── Tables ────────────────────────────────────────────────────────────

        [Test]
        public void Parse_Table_ProducesTableNode()
        {
            // Arrange
            string markdown = "| H1 |\n| -- |\n| v1 |";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            Assert.AreEqual(1, doc.Children.Count);
            Assert.IsInstanceOf<TableNode>(doc.Children[0]);
        }

        [Test]
        public void Parse_TableFirstRow_IsMarkedAsHeader()
        {
            // Arrange
            string markdown = "| H1 |\n| -- |\n| v1 |";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var table = doc.Children[0] as TableNode;
            Assert.IsNotNull(table);
            Assert.IsTrue(table.Rows[0].IsHeader);
        }

        [Test]
        public void Parse_TableSeparatorRow_IsSkipped()
        {
            // Arrange
            string markdown = "| H1 |\n| -- |\n| v1 |";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var table = doc.Children[0] as TableNode;
            Assert.IsNotNull(table);
            Assert.AreEqual(2, table.Rows.Count);
        }

        [Test]
        public void Parse_TableRow_CellsAreParsedCorrectly()
        {
            // Arrange
            string markdown = "| a | b |";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var table = doc.Children[0] as TableNode;
            Assert.IsNotNull(table);
            Assert.AreEqual(2, table.Rows[0].Cells.Count);
        }

        // ── Multi-block ───────────────────────────────────────────────────────

        [Test]
        public void Parse_MultipleBlocks_AllProducedInOrder()
        {
            // Arrange
            string markdown = "# Title\n---\nsome text";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            Assert.AreEqual(3, doc.Children.Count);
            Assert.IsInstanceOf<HeadingNode>(doc.Children[0]);
            Assert.IsInstanceOf<HorizontalRuleNode>(doc.Children[1]);
            Assert.IsInstanceOf<ParagraphNode>(doc.Children[2]);
        }

        [Test]
        public void Parse_ParagraphWithBoldText_InlineContainsBoldNode()
        {
            // Arrange
            string markdown = "hello **world**";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var para = doc.Children[0] as ParagraphNode;
            Assert.IsNotNull(para);
            Assert.IsTrue(para.Inline.Exists(n => n is BoldNode));
        }

        [Test]
        public void Parse_HeadingWithInlineFormatting_InlineIsParsed()
        {
            // Arrange
            string markdown = "# Hello *world*";

            // Act
            DocumentNode doc = ParseMarkdown(markdown);

            // Assert
            var heading = doc.Children[0] as HeadingNode;
            Assert.IsNotNull(heading);
            Assert.IsTrue(heading.Inline.Exists(n => n is ItalicNode));
        }
    }
}
