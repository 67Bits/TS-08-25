// ShowIfAttribute
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Use this PropertyAttribute to show / hide a field in the Inspector given a condition.
/// </summary>
public class ShowIfAttribute : PropertyAttribute
{
    public string Condition
    {
        get; private set;
    }

    public ShowIfAttribute(string condition)
    {
        Condition = condition;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ConditionalShowPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute conditionalShow = (ShowIfAttribute)attribute;

        if (EvaluateCondition(property, conditionalShow.Condition))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute conditionalShow = (ShowIfAttribute)attribute;

        if (EvaluateCondition(property, conditionalShow.Condition))
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        return -EditorGUIUtility.standardVerticalSpacing;
    }

    private bool EvaluateCondition(SerializedProperty property, string condition)
    {
        string[] tokens = Regex.Split(condition, @"(\(|\)|\&\&|\|\||==|!=|>=|<=|>|<|\s+)").Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

        Stack<bool> valueStack = new Stack<bool>();
        Stack<string> operatorStack = new Stack<string>();

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i].Trim();

            if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    ApplyOperator(valueStack, operatorStack.Pop());
                }
                if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                    operatorStack.Pop();
            }
            else if (IsLogicalOperator(token))
            {
                while (operatorStack.Count > 0 && Precedence(operatorStack.Peek()) >= Precedence(token))
                {
                    ApplyOperator(valueStack, operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
            else
            {
                if (i + 2 < tokens.Length && IsComparisonOperator(tokens[i + 1]))
                {
                    bool result = EvaluateComparison(property, tokens[i], tokens[i + 1], tokens[i + 2]);
                    valueStack.Push(result);
                    i += 2;
                }
                else
                {
                    bool result = EvaluateSingleValue(property, token);
                    valueStack.Push(result);
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            ApplyOperator(valueStack, operatorStack.Pop());
        }

        return valueStack.Count > 0 && valueStack.Pop();
    }

    private bool EvaluateComparison(SerializedProperty rootProperty, string left, string op, string right)
    {
        object leftValue = GetPropertyValue(rootProperty, left);
        Type leftType = leftValue?.GetType();
        object rightValue = ParseValue(right, leftType);

        if (leftValue == null && rightValue == null)
        {
            return op == "==";
        }
        else if (leftValue == null || rightValue == null)
        {
            return op == "!=";
        }

        // Ensure both values are of the same type for comparison
        if (leftValue is IConvertible && rightValue is IConvertible)
        {
            try
            {
                // Convert both to double for numeric comparisons
                double leftDouble = Convert.ToDouble(leftValue);
                double rightDouble = Convert.ToDouble(rightValue);

                switch (op)
                {
                    case "==":
                        return Math.Abs(leftDouble - rightDouble) < double.Epsilon;
                    case "!=":
                        return Math.Abs(leftDouble - rightDouble) >= double.Epsilon;
                    case ">":
                        return leftDouble > rightDouble;
                    case "<":
                        return leftDouble < rightDouble;
                    case ">=":
                        return leftDouble >= rightDouble;
                    case "<=":
                        return leftDouble <= rightDouble;
                }
            }
            catch (Exception)
            {
                string leftString = leftValue.ToString();
                string rightString = rightValue.ToString();

                switch (op)
                {
                    case "==":
                        return leftString == rightString;
                    case "!=":
                        return leftString != rightString;
                    case ">":
                        return string.Compare(leftString, rightString) > 0;
                    case "<":
                        return string.Compare(leftString, rightString) < 0;
                    case ">=":
                        return string.Compare(leftString, rightString) >= 0;
                    case "<=":
                        return string.Compare(leftString, rightString) <= 0;
                }
            }
        }
        else if (leftValue is IComparable comparable)
        {
            try
            {
                int comparison = comparable.CompareTo(rightValue);
                switch (op)
                {
                    case "==":
                        return comparison == 0;
                    case "!=":
                        return comparison != 0;
                    case ">":
                        return comparison > 0;
                    case "<":
                        return comparison < 0;
                    case ">=":
                        return comparison >= 0;
                    case "<=":
                        return comparison <= 0;
                }
            }
            catch (ArgumentException)
            {
                // If comparison fails, fall back to string comparison
                string leftString = leftValue.ToString();
                string rightString = rightValue.ToString();

                switch (op)
                {
                    case "==":
                        return leftString == rightString;
                    case "!=":
                        return leftString != rightString;
                    case ">":
                        return string.Compare(leftString, rightString) > 0;
                    case "<":
                        return string.Compare(leftString, rightString) < 0;
                    case ">=":
                        return string.Compare(leftString, rightString) >= 0;
                    case "<=":
                        return string.Compare(leftString, rightString) <= 0;
                }
            }
        }

        return false;
    }

    private bool EvaluateSingleValue(SerializedProperty rootProperty, string propertyName)
    {
        object value = GetPropertyValue(rootProperty, propertyName);
        return value is bool boolValue ? boolValue : value != null;
    }

    private object GetPropertyValue(SerializedProperty rootProperty, string propertyPath)
    {
        SerializedProperty prop = rootProperty.serializedObject.FindProperty(propertyPath);
        if (prop == null)
            return null;

        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                return prop.intValue;
            case SerializedPropertyType.Boolean:
                return prop.boolValue;
            case SerializedPropertyType.Float:
                return prop.floatValue;
            case SerializedPropertyType.String:
                return prop.stringValue;
            case SerializedPropertyType.Enum:
                return prop.enumNames[prop.enumValueIndex];
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue;
            default:
                return null;
        }
    }

    private object ParseValue(string value, Type targetType)
    {
        if (targetType == null)
            return value;

        if (targetType == typeof(int) && int.TryParse(value, out int intResult))
            return intResult;
        if (targetType == typeof(float) && float.TryParse(value, out float floatResult))
            return floatResult;
        if (targetType == typeof(bool) && bool.TryParse(value, out bool boolResult))
            return boolResult;
        if (targetType.IsEnum)
            return Enum.Parse(targetType, value);
        if (value == "null")
            return null;
        return value;
    }

    private bool IsLogicalOperator(string token)
    {
        return token == "&&" || token == "||";
    }

    private bool IsComparisonOperator(string token)
    {
        return token == "==" || token == "!=" || token == ">" || token == "<" || token == ">=" || token == "<=";
    }

    private int Precedence(string op)
    {
        switch (op)
        {
            case "||":
                return 1;
            case "&&":
                return 2;
            default:
                return 0;
        }
    }

    private void ApplyOperator(Stack<bool> values, string op)
    {
        bool right = values.Pop();
        bool left = values.Pop();
        switch (op)
        {
            case "&&":
                values.Push(left && right);
                break;
            case "||":
                values.Push(left || right);
                break;
        }
    }
}

#endif