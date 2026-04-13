using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Warlogic.Utils.Markdown
{
    [CustomEditor(typeof(MarkdownImporter))]
    public class MarkdownInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            string text = File.ReadAllText(((AssetImporter)target).assetPath);

            IMarkdownLexer lexer = new MarkdownLexer();
            IMarkdownInlineParser inlineParser = new MarkdownInlineParser();
            IMarkdownBlockParser blockParser = new MarkdownBlockParser(inlineParser);
            IMarkdownVisualElementRenderer renderer = new MarkdownVisualElementRenderer();

            LexerLine[] tokens = lexer.Tokenize(text);
            DocumentNode ast = blockParser.Parse(tokens);
            VisualElement root = renderer.Render(ast);
            root.style.paddingTop = 16;
            return root;
        }
    }
}
