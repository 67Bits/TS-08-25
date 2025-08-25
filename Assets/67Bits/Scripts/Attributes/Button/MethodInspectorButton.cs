using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[ExecuteInEditMode]
public class MethodInspectorButton
{
    [SerializeField] private string name;

    public string MethodName
    {
        get => name;
    }

    public MethodInspectorButton(string name)
    {
        this.name = name;
    }

    public static implicit operator MethodInspectorButton(string name)
    {
        return new MethodInspectorButton(name);
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(MethodInspectorButton))]
public class InspectorButtonDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var btn = (MethodInspectorButton)fieldInfo.GetValue(property.serializedObject.targetObject);

        // EditorGUI.PropertyField(position, property, label, true);

        // Get the target object (the class instance that owns the property)
        object targetObject = property.serializedObject.targetObject;

        // Use reflection to find and call a method on the target class
        MethodInfo methodInfo = targetObject.GetType().GetMethod(btn.MethodName);

        if (methodInfo == null)
        {
            Debug.Log($"Method with name {btn.MethodName} not found on object to be used in button.", property.serializedObject.targetObject);
        }
        else
        {
            // Call the method if it exists
            if (GUILayout.Button(btn.MethodName))
            {
                methodInfo.Invoke(targetObject, null);
            }
        }

        EditorGUILayout.Space();
    }
}

#endif