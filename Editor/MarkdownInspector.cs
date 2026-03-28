using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Warlogic.Utils.Markdown
{
    [CustomEditor(typeof(MarkdownImporter))]
    public class MarkdownInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var importer = (AssetImporter)target;
            string text = File.ReadAllText(importer.assetPath);

            var root = new VisualElement();
            root.style.flexGrow = 1;

            RenderMarkdown(text, root);
            return root;
        }

        private void RenderMarkdown(string text, VisualElement container)
        {
            string[] lines = text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            bool inCodeBlock = false;
            var codeLines = new List<string>();
            bool inTable = false;
            var tableRows = new List<string>();

            foreach (string line in lines)
            {
                string trimmed = line.TrimEnd();

                if (trimmed.StartsWith("```"))
                {
                    if (inTable)
                    {
                        container.Add(CreateTable(tableRows));
                        tableRows.Clear();
                        inTable = false;
                    }

                    if (!inCodeBlock)
                    {
                        inCodeBlock = true;
                        codeLines.Clear();
                    }
                    else
                    {
                        inCodeBlock = false;
                        container.Add(CreateCodeBlock(codeLines));
                        codeLines.Clear();
                    }
                    continue;
                }

                if (inCodeBlock)
                {
                    codeLines.Add(line);
                    continue;
                }

                if (trimmed.StartsWith("|"))
                {
                    inTable = true;
                    tableRows.Add(trimmed);
                    continue;
                }

                if (inTable)
                {
                    container.Add(CreateTable(tableRows));
                    tableRows.Clear();
                    inTable = false;
                }

                if (trimmed == "---" || trimmed == "***" || trimmed == "___")
                {
                    container.Add(CreateHorizontalRule());
                    continue;
                }

                if (trimmed.StartsWith("# "))
                {
                    container.Add(CreateHeading(trimmed.Substring(2), 22));
                    continue;
                }

                if (trimmed.StartsWith("## "))
                {
                    container.Add(CreateHeading(trimmed.Substring(3), 18));
                    continue;
                }

                if (trimmed.StartsWith("### "))
                {
                    container.Add(CreateHeading(trimmed.Substring(4), 15));
                    continue;
                }

                if (trimmed.StartsWith("- ") || trimmed.StartsWith("* "))
                {
                    container.Add(CreateListItem("•", trimmed.Substring(2)));
                    continue;
                }

                var numberedMatch = Regex.Match(trimmed, @"^(\d+)\. (.+)$");
                if (numberedMatch.Success)
                {
                    container.Add(CreateListItem(numberedMatch.Groups[1].Value + ".", numberedMatch.Groups[2].Value));
                    continue;
                }

                if (trimmed.Length == 0)
                {
                    container.Add(CreateSpacer());
                    continue;
                }

                container.Add(CreateParagraph(trimmed));
            }

            if (inCodeBlock && codeLines.Count > 0)
                container.Add(CreateCodeBlock(codeLines));

            if (inTable && tableRows.Count > 0)
                container.Add(CreateTable(tableRows));
        }

        private Label CreateHeading(string text, int fontSize)
        {
            var label = new Label(text);
            label.enableRichText = false;
            label.style.fontSize = fontSize;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginTop = 16;
            label.style.marginBottom = 4;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        private VisualElement CreateParagraph(string text)
        {
            var el = CreateInlineElement(text);
            el.style.marginBottom = 4;
            return el;
        }

        private VisualElement CreateListItem(string bullet, string text)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 2;
            row.style.marginLeft = 8;

            var bulletLabel = new Label(bullet);
            bulletLabel.enableRichText = false;
            bulletLabel.style.minWidth = 16;
            bulletLabel.style.marginRight = 4;
            row.Add(bulletLabel);

            var content = CreateInlineElement(text);
            content.style.flexShrink = 1;
            content.style.flexGrow = 1;
            row.Add(content);

            return row;
        }

        private VisualElement CreateCodeBlock(List<string> lines)
        {
            var wrapper = new VisualElement();
            wrapper.style.backgroundColor = new Color(0f, 0f, 0f, 0.15f);
            wrapper.style.paddingTop = 8;
            wrapper.style.paddingBottom = 8;
            wrapper.style.paddingLeft = 8;
            wrapper.style.paddingRight = 8;
            wrapper.style.borderTopLeftRadius = 4;
            wrapper.style.borderTopRightRadius = 4;
            wrapper.style.borderBottomLeftRadius = 4;
            wrapper.style.borderBottomRightRadius = 4;
            wrapper.style.marginTop = 4;
            wrapper.style.marginBottom = 8;

            var label = new Label(string.Join("\n", lines));
            label.enableRichText = false;
            label.style.whiteSpace = WhiteSpace.Pre;
            wrapper.Add(label);

            return wrapper;
        }

        private VisualElement CreateHorizontalRule()
        {
            var hr = new VisualElement();
            hr.style.height = 1;
            hr.style.marginTop = 8;
            hr.style.marginBottom = 8;
            hr.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            return hr;
        }

        private VisualElement CreateSpacer()
        {
            var spacer = new VisualElement();
            spacer.style.height = 8;
            return spacer;
        }

        private VisualElement CreateInlineElement(string text)
        {
            string[] parts = text.Split('`');

            if (parts.Length == 1)
                return CreateRichTextLabel(ApplyBoldItalic(text));

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;

            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 0)
                {
                    if (parts[i].Length > 0)
                        row.Add(CreateRichTextLabel(ApplyBoldItalic(parts[i])));
                }
                else
                {
                    row.Add(CreateCodeSpanLabel(parts[i]));
                }
            }

            return row;
        }

        private Label CreateRichTextLabel(string richText)
        {
            var label = new Label(richText);
            label.enableRichText = true;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        private VisualElement CreateCodeSpanLabel(string code)
        {
            var wrapper = new VisualElement();
            wrapper.style.backgroundColor = new Color(0f, 0f, 0f, 0.15f);
            wrapper.style.paddingLeft = 3;
            wrapper.style.paddingRight = 3;
            wrapper.style.borderTopLeftRadius = 3;
            wrapper.style.borderTopRightRadius = 3;
            wrapper.style.borderBottomLeftRadius = 3;
            wrapper.style.borderBottomRightRadius = 3;

            var label = new Label(code);
            label.enableRichText = false;
            wrapper.Add(label);

            return wrapper;
        }

        private string ApplyBoldItalic(string text)
        {
            text = Regex.Replace(text, @"\*\*(.+?)\*\*", "<b>$1</b>");
            text = Regex.Replace(text, @"\*(.+?)\*", "<i>$1</i>");
            return text;
        }

        private List<string> ParseTableRow(string line)
        {
            var parts = line.Split('|');
            var cells = new List<string>();
            for (int i = 1; i < parts.Length - 1; i++)
                cells.Add(parts[i].Trim());
            return cells;
        }

        private bool IsSeparatorRow(List<string> cells)
            => cells.TrueForAll(c => Regex.IsMatch(c, @"^[-: ]+$"));

        private VisualElement CreateTable(List<string> rows)
        {
            var borderColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

            var wrapper = new VisualElement();
            wrapper.style.marginTop = 8;
            wrapper.style.marginBottom = 8;
            wrapper.style.borderTopWidth = 1;
            wrapper.style.borderBottomWidth = 1;
            wrapper.style.borderLeftWidth = 1;
            wrapper.style.borderRightWidth = 1;
            wrapper.style.borderTopColor = borderColor;
            wrapper.style.borderBottomColor = borderColor;
            wrapper.style.borderLeftColor = borderColor;
            wrapper.style.borderRightColor = borderColor;
            wrapper.style.borderTopLeftRadius = 4;
            wrapper.style.borderTopRightRadius = 4;
            wrapper.style.borderBottomLeftRadius = 4;
            wrapper.style.borderBottomRightRadius = 4;
            wrapper.style.overflow = Overflow.Hidden;

            bool firstDataRow = true;
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var cells = ParseTableRow(rows[rowIndex]);
                if (cells.Count == 0) continue;
                if (IsSeparatorRow(cells)) continue;

                bool isHeader = rowIndex == 0;
                bool isAlternate = !isHeader && !firstDataRow;
                if (!isHeader) firstDataRow = !firstDataRow;

                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                if (isHeader)
                    row.style.backgroundColor = new Color(0f, 0f, 0f, 0.2f);
                else if (isAlternate)
                    row.style.backgroundColor = new Color(0f, 0f, 0f, 0.05f);

                for (int colIndex = 0; colIndex < cells.Count; colIndex++)
                {
                    var cell = new VisualElement();
                    cell.style.flexGrow = 1;
                    cell.style.flexBasis = 0;
                    cell.style.paddingTop = 4;
                    cell.style.paddingBottom = 4;
                    cell.style.paddingLeft = 6;
                    cell.style.paddingRight = 6;

                    if (colIndex < cells.Count - 1)
                    {
                        cell.style.borderRightWidth = 1;
                        cell.style.borderRightColor = borderColor;
                    }

                    if (rowIndex > 0)
                    {
                        cell.style.borderTopWidth = 1;
                        cell.style.borderTopColor = borderColor;
                    }

                    var content = CreateInlineElement(cells[colIndex]);
                    if (isHeader)
                    {
                        content.style.unityFontStyleAndWeight = FontStyle.Bold;
                    }
                    cell.Add(content);
                    row.Add(cell);
                }

                wrapper.Add(row);
            }

            return wrapper;
        }
    }
}
