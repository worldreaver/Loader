#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Worldreaver.EditorUtility;

namespace Worldreaver.Loading
{
    [CustomEditor(typeof(Loader), true)]
    [CanEditMultipleObjects]
    public class LoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            Repaint();
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        private void Draw()
        {
            var loadingType = EditorUtil.SerializeField(serializedObject, "loadingType");
            EditorUtil.SerializeField(serializedObject, "completeType");

            EditorUtil.DrawSeparator();
            EditorUtil.SerializeField(serializedObject, "value", loadingType.intValue == 1 ? "Time Load(s)" : "Speed Load");
            EditorUtil.SerializeField(serializedObject, "fadeInSpeed");
            EditorUtil.SerializeField(serializedObject, "fadeOutSpeed");

            EditorUtil.DrawSeparator();
            var isTip = EditorUtil.SerializeField(serializedObject, "isTip");
            if (isTip.boolValue)
            {
                GUILayout.Space(4);
                EditorUtil.SerializeField(serializedObject, "tipText", "Text");
                EditorUtil.SerializeField(serializedObject, "timePerTip");
                EditorUtil.SerializeField(serializedObject, "tipFadeSpeed");
            }

            EditorUtil.DrawSeparator();
            var isProcessBar = EditorUtil.SerializeField(serializedObject, "isProcessBar");
            if (isProcessBar.boolValue)
            {
                GUILayout.Space(4);
                EditorUtil.SerializeField(serializedObject, "processBar");
                var isDisplayTextProcess = EditorUtil.SerializeField(serializedObject, "isDisplayTextProcess", "Display Text");
                if (isDisplayTextProcess.boolValue)
                {
                    GUILayout.Space(4);
                    EditorUtil.SerializeField(serializedObject, "processText", "Text");
                    EditorUtil.SerializeField(serializedObject, "processTemplate", "Template");
                }
            }

            GUILayout.Space(4);
            EditorUtil.DrawSeparator();
            if (isProcessBar.boolValue)
            {
                EditorUtil.SerializeField(serializedObject, "canvasGroupProcessBar");
                EditorUtil.SerializeField(serializedObject, "timeFadeProcessBar", "Time Fade");
            }

            EditorUtil.SerializeField(serializedObject, "_loadComplete");

            EditorUtil.DrawSeparator();
            EditorUtil.SerializeField(serializedObject, "rootUi");
            EditorUtil.SerializeField(serializedObject, "fadeImageCanvas");
            EditorUtil.SerializeField(serializedObject, "canvasLoading", "Canvas");
        }
    }
}
#endif