# Markdown Utilities

Utilities for importing and rendering Markdown (`.md`) files in Unity. The parsing and rendering pipeline runs at runtime and in the Editor; editor-specific features (inspector preview, asset importer, file creation menu) are Editor-only.

# Installation

## Via Git URL

Open **Window → Package Manager**, click **+**, and choose **Add package from git URL**.

To install the latest version:
```
https://github.com/Warlander/markdown-utilities.git
```

To install a specific release, append the tag:
```
https://github.com/Warlander/markdown-utilities.git#1.1.0
```

## Via Scoped Registry

Add the Warlogic registry to your `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "Warlogic",
      "url": "https://upm.maciejcyranowicz.com",
      "scopes": ["com.warlogic"]
    }
  ],
  "dependencies": {
    "com.warlogic.utils.markdown": "1.1.0"
  }
}
```

Alternatively, open **Window → Package Manager**, click **+**, choose
**Add package by name**, and enter `com.warlogic.utils.markdown`.

# Setup

No setup required. Once the package is installed, the `MarkdownImporter` ScriptedImporter activates automatically for all `.md` files in the project.

# Usage

**Viewing Markdown files**

Select any `.md` file in the Project window. The Inspector renders the file with full Markdown formatting: headings, bold/italic, inline code, fenced code blocks, bullet and numbered lists, tables, and horizontal rules.

**Creating a new Markdown file**

Use the menu item **Assets > Create > Markdown File** to create a new `.md` file pre-filled with a title template at the selected project location.

**Referencing Markdown as a TextAsset**

Imported `.md` files are available as `TextAsset` objects and can be loaded via `Resources.Load`, `AssetDatabase`, or assigned to a serialized `TextAsset` field.

**Parsing and rendering Markdown programmatically**

The parsing and rendering pipeline is fully accessible as a public API and works at both runtime and in the Editor:

```csharp
using Warlogic.Utils.Markdown;

string source = "# Hello\n\nSome **bold** text.";

var lexer       = new MarkdownLexer();
var tokens      = lexer.Tokenize(source);

var blockParser = new MarkdownBlockParser(new MarkdownInlineParser());
var doc         = blockParser.Parse(tokens);

var renderer  = new MarkdownVisualElementRenderer();
VisualElement element = renderer.Render(doc);
```

Individual pipeline stages — lexer, block parser, inline parser, AST nodes, and renderer — can be used independently or replaced with custom implementations via their respective interfaces (`IMarkdownLexer`, `IMarkdownBlockParser`, `IMarkdownInlineParser`, `IMarkdownVisualElementRenderer`).
