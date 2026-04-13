namespace Warlogic.Utils.Markdown
{
    public interface IMarkdownBlockParser
    {
        DocumentNode Parse(LexerLine[] lines);
    }
}
