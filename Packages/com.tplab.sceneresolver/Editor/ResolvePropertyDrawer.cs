using UnityEditor;
using UnityEngine;

namespace TpLab.SceneResolver.Editor
{
    [CustomPropertyDrawer(typeof(ResolveAttribute))]
    public class ResolvePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var resolveAttribute = (ResolveAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUI.LabelField(position, label, new GUIContent($"Resolve: {resolveAttribute.Source}"));
            }
            EditorGUI.EndProperty();
        }
    }
}
