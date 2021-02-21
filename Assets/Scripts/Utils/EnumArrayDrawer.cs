using UnityEngine;
using UnityEditor;
 
[CustomPropertyDrawer (typeof(EnumArrayAttribute))]
public class EnumArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        try {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.ObjectField(rect, property, new GUIContent(((EnumArrayAttribute)attribute).GetResult(pos)));
        } catch {
            EditorGUI.ObjectField(rect, property, label);
        }
    }
}