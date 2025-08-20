namespace SatProductions
{
    using UnityEngine;
    using UnityEditor;

#if UNITY_EDITOR
    [CustomEditor(typeof(BreakableObject))]
    [CanEditMultipleObjects]
    public class BreakableObjectEditor : Editor
    {
        private Texture2D logo;
        private const float logoWidth = 1024f;
        private const float logoHeight = 256f;
        private Rect logoRect;

        private void OnEnable()
        {
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sat Productions/Common/Breakable Props Packs/Common/Scripts/Editor/Resources/BreakableAssetInspectorBanner.png");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Logo
            if (logo)
            {
                GUILayout.Space(10);
                float inspectorWidth = EditorGUIUtility.currentViewWidth;
                float scaledWidth = Mathf.Min(inspectorWidth - 20, logoWidth);
                float scaledHeight = (scaledWidth / 1024f) * logoHeight;

                logoRect = GUILayoutUtility.GetRect(scaledWidth, scaledHeight, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(logoRect, logo, ScaleMode.ScaleToFit);

                if (Event.current.type == EventType.MouseDown && logoRect.Contains(Event.current.mousePosition))
                {
                    Application.OpenURL("https://satproductions.com/");
                    Event.current.Use();
                }

                GUILayout.Space(10);
            }

            // Explosion Settings
            EditorGUILayout.LabelField("💥 Explosion Settings", EditorStyles.boldLabel);
            SerializedProperty explosionPowerProp = serializedObject.FindProperty("explosionPower");
            if (explosionPowerProp != null)
                EditorGUILayout.PropertyField(explosionPowerProp, new GUIContent("Explosion Power"));
            else
                Debug.LogError("BreakableObjectEditor: Property 'explosionPower' not found!");

            GUILayout.Space(15);

            // Collision Settings
            EditorGUILayout.LabelField("💢 Collision Settings", EditorStyles.boldLabel);
            SerializedProperty breakOnHitProp = serializedObject.FindProperty("BreakOnHit");
            if (breakOnHitProp != null)
                EditorGUILayout.PropertyField(breakOnHitProp, new GUIContent("Break On Hit"));
            else
                Debug.LogError("BreakableObjectEditor: Property 'BreakOnHit' not found!");

            GUILayout.Space(15);

            // Audio Settings
            EditorGUILayout.LabelField("🔊 Audio Settings", EditorStyles.boldLabel);
            SerializedProperty breakSoundProp = serializedObject.FindProperty("breakSound");
            SerializedProperty spawnSoundProp = serializedObject.FindProperty("spawnSound");
            SerializedProperty volumeProp = serializedObject.FindProperty("volume");

            if (breakSoundProp != null)
                EditorGUILayout.PropertyField(breakSoundProp, new GUIContent("Break Sound"));
            if (spawnSoundProp != null)
                EditorGUILayout.PropertyField(spawnSoundProp, new GUIContent("Spawn Sound"));
            if (volumeProp != null)
                EditorGUILayout.PropertyField(volumeProp, new GUIContent("Volume"));

            GUILayout.Space(15);

            // Break and Fix Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Break Object", GUILayout.Height(30)))
            {
                foreach (BreakableObject breakable in targets)
                {
                    breakable.Break();
                }
            }
            if (GUILayout.Button("Fix Object", GUILayout.Height(30)))
            {
                foreach (BreakableObject breakable in targets)
                {
                    breakable.FixObject();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            // Debug & Gizmos
            EditorGUILayout.LabelField("🛠 Debug & Gizmos", EditorStyles.boldLabel);
            SerializedProperty showGizmosProp = serializedObject.FindProperty("showGizmos");
            if (showGizmosProp != null)
                EditorGUILayout.PropertyField(showGizmosProp, new GUIContent("Show Gizmos"));
            else
                Debug.LogError("BreakableObjectEditor: Property 'showGizmos' not found!");

            GUILayout.Space(15);

            // Destruction Settings
            EditorGUILayout.LabelField("⚡ Destruction Settings", EditorStyles.boldLabel);
            SerializedProperty destroyPiecesAfterDelayProp = serializedObject.FindProperty("destroyPiecesAfterDelay");
            if (destroyPiecesAfterDelayProp != null)
            {
                EditorGUILayout.PropertyField(destroyPiecesAfterDelayProp, new GUIContent("Deactive Pieces After Delay"));

                if (destroyPiecesAfterDelayProp.boolValue)
                {
                    SerializedProperty destroyDelayProp = serializedObject.FindProperty("destroyDelay");
                    SerializedProperty fadeDurationProp = serializedObject.FindProperty("fadeOutDuration");

                    if (destroyDelayProp != null)
                        EditorGUILayout.PropertyField(destroyDelayProp, new GUIContent("Delay Time (s)"));
                    else
                        Debug.LogError("BreakableObjectEditor: Property 'destroyDelay' not found!");

                    if (fadeDurationProp != null)
                        EditorGUILayout.PropertyField(fadeDurationProp, new GUIContent("Fade Duration (s)"));
                    else
                        Debug.LogError("BreakableObjectEditor: Property 'fadeOutDuration' not found!");
                }
            }
            else
                Debug.LogError("BreakableObjectEditor: Property 'destroyPiecesAfterDelay' not found!");

            GUILayout.Space(10);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}