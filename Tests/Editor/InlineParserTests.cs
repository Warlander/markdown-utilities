using System.Collections.Generic;
using NUnit.Framework;

namespace Warlogic.Utils.Markdown.Tests
{
    [TestFixture]
    public class InlineParserTests
    {
        private MarkdownInlineParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new MarkdownInlineParser();
        }

        [Test]
        public void Parse_EmptyString_ReturnsEmptyList()
        {
            // Arrange
            string input = "";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Parse_PlainText_ReturnsSingleTextNode()
        {
            // Arrange
            string input = "hello";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsInstanceOf<TextNode>(result[0]);
            Assert.AreEqual("hello", ((TextNode)result[0]).Content);
        }

        [Test]
        public void Parse_Bold_ReturnsBoldNodeWithTextChild()
        {
            // Arrange
            string input = "**bold**";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(1, result.Count);
            var bold = result[0] as BoldNode;
            Assert.IsNotNull(bold);
            Assert.AreEqual(1, bold.Children.Count);
            Assert.AreEqual("bold", ((TextNode)bold.Children[0]).Content);
        }

        [Test]
        public void Parse_Italic_ReturnsItalicNodeWithTextChild()
        {
            // Arrange
            string input = "*italic*";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(1, result.Count);
            var italic = result[0] as ItalicNode;
            Assert.IsNotNull(italic);
            Assert.AreEqual(1, italic.Children.Count);
            Assert.AreEqual("italic", ((TextNode)italic.Children[0]).Content);
        }

        [Test]
        public void Parse_InlineCode_ReturnsCodeSpanNodeWithContent()
        {
            // Arrange
            string input = "`code`";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(1, result.Count);
            var codeSpan = result[0] as CodeSpanNode;
            Assert.IsNotNull(codeSpan);
            Assert.AreEqual("code", codeSpan.Content);
        }

        [Test]
        public void Parse_TextAroundBold_ReturnsTextBoldTextNodes()
        {
            // Arrange
            string input = "a **b** c";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsInstanceOf<TextNode>(result[0]);
            Assert.IsInstanceOf<BoldNode>(result[1]);
            Assert.IsInstanceOf<TextNode>(result[2]);
        }

        [Test]
        public void Parse_BoldInsideItalic_ProducesNestedStructure()
        {
            // Arrange
            string input = "*hello **world** foo*";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(1, result.Count);
            var italic = result[0] as ItalicNode;
            Assert.IsNotNull(italic);
            Assert.AreEqual(3, italic.Children.Count);
            Assert.IsInstanceOf<TextNode>(italic.Children[0]);
            Assert.IsInstanceOf<BoldNode>(italic.Children[1]);
            Assert.IsInstanceOf<TextNode>(italic.Children[2]);
        }

        [Test]
        public void Parse_MultipleBoldSpans_ParsesAll()
        {
            // Arrange
            string input = "**a** and **b**";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsInstanceOf<BoldNode>(result[0]);
            Assert.IsInstanceOf<TextNode>(result[1]);
            Assert.IsInstanceOf<BoldNode>(result[2]);
        }

        [Test]
        public void Parse_CodeSpanPreservesInnerAsterisks()
        {
            // Arrange
            string input = "`**raw**`";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(1, result.Count);
            var codeSpan = result[0] as CodeSpanNode;
            Assert.IsNotNull(codeSpan);
            Assert.AreEqual("**raw**", codeSpan.Content);
        }

        [Test]
        public void Parse_UnclosedBacktick_TreatsBacktickAsLiteralText()
        {
            // Arrange
            string input = "`code";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            Assert.AreEqual(1, result.Count);
            var text = result[0] as TextNode;
            Assert.IsNotNull(text);
            Assert.IsTrue(text.Content.Contains("`"));
        }

        [Test]
        public void Parse_BoldContentPreservedInChildren()
        {
            // Arrange
            string input = "**hello**";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            var bold = result[0] as BoldNode;
            Assert.IsNotNull(bold);
            Assert.AreEqual("hello", ((TextNode)bold.Children[0]).Content);
        }

        [Test]
        public void Parse_ItalicContentPreservedInChildren()
        {
            // Arrange
            string input = "*hello*";

            // Act
            List<InlineNode> result = _parser.Parse(input);

            // Assert
            var italic = result[0] as ItalicNode;
            Assert.IsNotNull(italic);
            Assert.AreEqual("hello", ((TextNode)italic.Children[0]).Content);
        }
    }
}
