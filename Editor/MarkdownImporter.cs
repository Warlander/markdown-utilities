using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Warlogic.Utils.Markdown
{
    [ScriptedImporter(1, null, new[] { "md" })]
    public class MarkdownImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string text = File.ReadAllText(ctx.assetPath);
            var asset = new TextAsset(text);
            ctx.AddObjectToAsset("main", asset);
            ctx.SetMainObject(asset);
        }
    }
}
