using System;
using UnityEngine;

public class StringIntListAttribute : PropertyAttribute
{
    public delegate string[] GetStringList();
    public StringIntListAttribute(params string[] list)
    {
        List = list;
    }

    public StringIntListAttribute(Type type, string methodName)
    {
        var method = type.GetMethod(methodName);

        if (method != null)
        {
            List = method.Invoke(null, null) as string[];
        }
        else
        {
            Debug.LogError($"NO SUCH METHOD:{methodName} ,type:{type}");
        }
    }

    public string[] List
    {
        get;
        private set;
    }
}



#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(StringIntListAttribute))]
public class StringInListDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {
        var stringInList = attribute as StringIntListAttribute;

        var list = stringInList.List;

        if (property.propertyType == UnityEditor.SerializedPropertyType.String)
        {
            int index = Mathf.Max(0, Array.IndexOf(list, property.stringValue));
            index = UnityEditor.EditorGUI.Popup(position, property.displayName, index, list);
            property.stringValue = list[index];
        }
        else if (property.propertyType == UnityEditor.SerializedPropertyType.Integer)
        {
            property.intValue = UnityEditor.EditorGUI.Popup(position, property.displayName, property.intValue, list);
        }
        else
        {
            base.OnGUI(position, property, label);
        }
    }
}
#endif