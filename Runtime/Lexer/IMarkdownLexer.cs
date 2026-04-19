namespace Warlogic.Utils.Markdown
{
    public interface IMarkdownLexer
    {
        LexerLine[] Tokenize(string text);
    }
}
