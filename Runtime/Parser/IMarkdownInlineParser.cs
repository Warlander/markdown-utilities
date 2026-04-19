using System.Collections.Generic;

namespace Warlogic.Utils.Markdown
{
    public interface IMarkdownInlineParser
    {
        List<InlineNode> Parse(string text);
    }
}
