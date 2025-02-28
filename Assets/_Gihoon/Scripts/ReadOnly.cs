using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // 수정 불가능하게 설정
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;  // 다시 활성화
    }
}
#endif