using NUnit.Framework;

namespace Warlogic.Utils.Markdown.Tests
{
    [TestFixture]
    public class LexerTests
    {
        // ── LexerLine.Classify ────────────────────────────────────────────────

        [Test]
        public void Classify_EmptyLine_ReturnsBlank()
        {
            // Arrange
            string input = "";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.Blank, result.Type);
        }

        [Test]
        public void Classify_WhitespaceOnly_ReturnsBlank()
        {
            // Arrange
            string input = "   ";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.Blank, result.Type);
        }

        [Test]
        public void Classify_H1_ReturnsHeadingWithLevel1AndContent()
        {
            // Arrange
            string input = "# Title";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.Heading, result.Type);
            Assert.AreEqual(1, result.HeadingLevel);
            Assert.AreEqual("Title", result.Content);
        }

        [Test]
        public void Classify_H2_ReturnsHeadingWithLevel2()
        {
            // Arrange
            string input = "## Sub";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.Heading, result.Type);
            Assert.AreEqual(2, result.HeadingLevel);
        }

        [Test]
        public void Classify_H3_ReturnsHeadingWithLevel3()
        {
            // Arrange
            string input = "### Deep";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.Heading, result.Type);
            Assert.AreEqual(3, result.HeadingLevel);
        }

        [Test]
        public void Classify_H4_TreatedAsText()
        {
            // Arrange
            string input = "#### Four";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.Text, result.Type);
        }

        [Test]
        public void Classify_HrDashes_ReturnsHorizontalRule()
        {
            // Arrange
            string input = "---";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.HorizontalRule, result.Type);
        }

        [Test]
        public void Classify_HrAsterisks_ReturnsHorizontalRule()
        {
            // Arrange
            string input = "***";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.HorizontalRule, result.Type);
        }

        [Test]
        public void Classify_HrUnderscores_ReturnsHorizontalRule()
        {
            // Arrange
            string input = "___";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.HorizontalRule, result.Type);
        }

        [Test]
        public void Classify_CodeFence_ReturnsCodeFenceWithEmptyContent()
        {
            // Arrange
            string input = "```";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.CodeFence, result.Type);
            Assert.AreEqual("", result.Content);
        }

        [Test]
        public void Classify_CodeFenceWithLanguage_CapturesLanguageInContent()
        {
            // Arrange
            string input = "```csharp";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.CodeFence, result.Type);
            Assert.AreEqual("csharp", result.Content);
        }

        [Test]
        public void Classify_TableRow_ReturnsTableRow()
        {
            // Arrange
            string input = "| a | b |";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.TableRow, result.Type);
        }

        [Test]
        public void Classify_UnorderedListDash_ReturnsUnorderedListItemWithContent()
        {
            // Arrange
            string input = "- item";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.UnorderedListItem, result.Type);
            Assert.AreEqual("item", result.Content);
        }

        [Test]
        public void Classify_UnorderedListAsterisk_ReturnsUnorderedListItemWithContent()
        {
            // Arrange
            string input = "* item";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.UnorderedListItem, result.Type);
            Assert.AreEqual("item", result.Content);
        }

        [Test]
        public void Classify_OrderedListItem_ReturnsOrderedListItemWithContentAndMarker()
        {
            // Arrange
            string input = "1. first";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.OrderedListItem, result.Type);
            Assert.AreEqual("first", result.Content);
            Assert.AreEqual("1.", result.OrderedMarker);
        }

        [Test]
        public void Classify_OrderedListItemHigherNumber_CapturesMarkerCorrectly()
        {
            // Arrange
            string input = "42. answer";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.OrderedListItem, result.Type);
            Assert.AreEqual("42.", result.OrderedMarker);
        }

        [Test]
        public void Classify_RegularText_ReturnsText()
        {
            // Arrange
            string input = "hello world";

            // Act
            LexerLine result = LexerLine.Classify(input);

            // Assert
            Assert.AreEqual(LineType.Text, result.Type);
        }

        // ── MarkdownLexer.Tokenize ────────────────────────────────────────────

        [Test]
        public void Tokenize_MultilineText_ReturnsOneEntryPerLine()
        {
            // Arrange
            var lexer = new MarkdownLexer();

            // Act
            LexerLine[] result = lexer.Tokenize("a\nb\nc");

            // Assert
            Assert.AreEqual(3, result.Length);
        }

        [Test]
        public void Tokenize_WindowsCRLF_HandledCorrectly()
        {
            // Arrange
            var lexer = new MarkdownLexer();

            // Act
            LexerLine[] result = lexer.Tokenize("a\r\nb");

            // Assert
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void Tokenize_TrailingNewline_ProducesBlankLastLine()
        {
            // Arrange
            var lexer = new MarkdownLexer();

            // Act
            LexerLine[] result = lexer.Tokenize("a\n");

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(LineType.Blank, result[1].Type);
        }

        [Test]
        public void Tokenize_MixedLineTypes_ClassifiesEachLineCorrectly()
        {
            // Arrange
            var lexer = new MarkdownLexer();

            // Act
            LexerLine[] result = lexer.Tokenize("# Heading\n- item\ntext");

            // Assert
            Assert.AreEqual(LineType.Heading, result[0].Type);
            Assert.AreEqual(LineType.UnorderedListItem, result[1].Type);
            Assert.AreEqual(LineType.Text, result[2].Type);
        }
    }
}
