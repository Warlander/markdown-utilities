# Markdown Utilities

Editor utilities for importing and rendering Markdown (`.md`) files directly in the Unity Editor Inspector.

# Installation

## Via Git URL

Open **Window → Package Manager**, click **+**, and choose **Add package from git URL**.

To install the latest version:
```
https://github.com/Warlander/markdown-utilities.git
```

To install a specific release, append the tag:
```
https://github.com/Warlander/markdown-utilities.git#1.0.2
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
    "com.warlogic.utils.markdown": "1.0.2"
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
