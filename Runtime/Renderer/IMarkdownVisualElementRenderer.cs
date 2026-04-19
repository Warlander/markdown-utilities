using UnityEngine.UIElements;

namespace Warlogic.Utils.Markdown
{
    public interface IMarkdownVisualElementRenderer
    {
        VisualElement Render(DocumentNode doc);
    }
}
