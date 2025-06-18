// AutoScriptableObjectDrawer.cs
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class AutoSOAssistantDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var objectRect = new Rect(position.x, position.y, position.width, position.height);
        float buttonWidth = 65f;

        if (property.objectReferenceValue == null)
        {
            objectRect.width -= buttonWidth + 4f;
            var buttonRect = new Rect(position.x + objectRect.width + 4f, position.y, buttonWidth, position.height);

            EditorGUI.PropertyField(objectRect, property, label);

            var type = fieldInfo.FieldType;

            if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                if (GUI.Button(buttonRect, "Create"))
                {
                    var path = EditorUtility.SaveFilePanelInProject(
                        $"Create {type.Name}",
                        $"New{type.Name}",
                        "asset",
                        $"Save a new {type.Name} asset");

                    if (!string.IsNullOrEmpty(path))
                    {
                        var instance = ScriptableObject.CreateInstance(type);
                        AssetDatabase.CreateAsset(instance, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        property.objectReferenceValue = instance;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            else
            {
                EditorGUI.HelpBox(buttonRect, "Invalid type", MessageType.Error);
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => EditorGUI.GetPropertyHeight(property, label, true);
}
