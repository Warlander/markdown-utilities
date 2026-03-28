using UnityEditor;

namespace Warlogic.Utils.Markdown
{
    public static class CreateMarkdownFile
    {
        [MenuItem("Assets/Create/Markdown File", priority = 80)]
        private static void CreateMd()
        {
            ProjectWindowUtil.CreateAssetWithContent("NewFile.md", "# Title\n\n");
        }
    }
}
