using System.Reflection;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonAttribute : PropertyAttribute
{
    public string ButtonText;
    public string Method;

    public ButtonAttribute(string buttonText, string method)
    {
        ButtonText = buttonText;
        Method = method;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;

        EditorGUI.PropertyField(position, property, label, true);

        var btn = (MethodInspectorButton)fieldInfo.GetValue(property.serializedObject.targetObject);
        if (GUILayout.Button(buttonAttribute.ButtonText))
        {

        }

        // Get the target object (the class instance that owns the property)
        object targetObject = property.serializedObject.targetObject;

        // Use reflection to find and call a method on the target class
        MethodInfo methodInfo = targetObject.GetType().GetMethod(buttonAttribute.Method);

        if (methodInfo != null)
        {
            // Call the method if it exists
            if (GUILayout.Button(buttonAttribute.ButtonText))
            {
                methodInfo.Invoke(targetObject, null);
            }
        }

        EditorGUILayout.Space();
    }
}

#endif