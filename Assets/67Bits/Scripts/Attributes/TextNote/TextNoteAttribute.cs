using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Use this PropertyAttribute to add an ajusted string field in the Inspector.
/// </summary>
public class TextNoteAttribute : PropertyAttribute
{
    public string text;
    public bool ReadOnly;

    public TextNoteAttribute(string text, bool readOnly = true)
    {
        this.text = text;
        this.ReadOnly = readOnly;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(TextNoteAttribute))]
public class TextNoteDecoratorDrawer : DecoratorDrawer
{
    private const float Padding = 5f;
    private const float HeaderHeight = 20f;
    private float cachedHeight = -1f;

    public override float GetHeight()
    {
        if (cachedHeight < 0)
        {
            // Return a default height initially
            return EditorGUIUtility.singleLineHeight * 3 + HeaderHeight;
        }
        return cachedHeight + HeaderHeight;
    }

    public override void OnGUI(Rect position)
    {
        TextNoteAttribute textNote = attribute as TextNoteAttribute;

        // Draw the "Note" header
        Rect headerRect = new Rect(position.x, position.y, position.width, HeaderHeight);
        EditorGUI.LabelField(headerRect, "Note:", EditorStyles.boldLabel);

        // Adjust the position for the text area
        position.y += HeaderHeight;
        position.height -= HeaderHeight;

        // Adjust the position to add some padding
        position.y += Padding;
        position.height -= 2 * Padding;
        position.x += Padding;
        position.width -= 2 * Padding;

        // Calculate the real height of the text
        GUIContent content = new GUIContent(textNote.text);
        float calculatedHeight = EditorStyles.textArea.CalcHeight(content, position.width);

        // Update the cached height if it has changed
        if (Mathf.Abs(calculatedHeight - (cachedHeight - 2 * Padding)) > 0.01f)
        {
            cachedHeight = calculatedHeight + 2 * Padding;
            EditorApplication.delayCall += () => EditorApplication.RepaintHierarchyWindow();
        }

        // false makes ReadOnly.
        GUI.enabled = !textNote.ReadOnly;

        // Draw the text area
        EditorGUI.TextArea(position, textNote.text, EditorStyles.textArea);
        
        GUI.enabled = true;

        // If the height has changed, force a repaint
        if (Event.current.type == EventType.Repaint && cachedHeight + HeaderHeight != position.height)
        {
            EditorApplication.delayCall += () => EditorApplication.RepaintHierarchyWindow();
        }
    }
}

#endif