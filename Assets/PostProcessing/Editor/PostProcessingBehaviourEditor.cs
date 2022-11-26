using System;
using System.Linq.Expressions;

namespace UnityEditor.PostProcessing
{
    [CustomEditor(typeof(PostProcessingBehaviourEditor))]
    public class PostProcessingBehaviourEditor : Editor
    {
        SerializedProperty m_Profile;

        public void OnEnable()
        {
            m_Profile = FindSetting((PostProcessingBehaviourEditor x) => x.m_Profile);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Profile);

            serializedObject.ApplyModifiedProperties();
        }

        SerializedProperty FindSetting<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            return serializedObject.FindProperty(ReflectionUtils.GetFieldPath(expr));
        }
    }
}
