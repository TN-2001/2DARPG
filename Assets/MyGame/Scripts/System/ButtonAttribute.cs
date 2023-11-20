using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonAttribute : PropertyAttribute
{
    public string methodName;

    public ButtonAttribute(string methodName)
    {
        this.methodName = methodName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;

        // ボタンを描画
        if (GUI.Button(position, buttonAttribute.methodName))
        {
            try
            {
                // メソッド名からメソッドを取得、実行
                MethodInfo method = property.serializedObject.targetObject.GetType().GetMethod(buttonAttribute.methodName);
                method.Invoke(property.serializedObject.targetObject, null);
            }
            catch
            {
                Debug.Log(buttonAttribute.methodName + " を実行できません");
            }
        }
    }
}
#endif
