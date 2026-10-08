using UnityEditor;
using UnityEngine;

namespace TennisHD2D.Editor
{
    /// <summary>
    /// Saves the preferred Quality level for the current machine in EditorPrefs.
    /// This prevents Git merge conflicts while letting your Laptop use Low settings 
    /// and your PC use High settings automatically.
    /// </summary>
    [InitializeOnLoad]
    public class QualityProfileSwitcher : EditorWindow
    {
        private const string EditorPrefKey = "TennisHD2D_PreferredQualityLevel";

        static QualityProfileSwitcher()
        {
            // Wait for Unity to fully load before applying
            EditorApplication.delayCall += ApplySavedQualityProfile;
        }

        [MenuItem("Tools/HD-2D Settings/Machine Quality Switcher")]
        public static void ShowWindow()
        {
            GetWindow<QualityProfileSwitcher>("Machine Profile");
        }

        private void OnGUI()
        {
            GUILayout.Label("Local Machine Performance Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("These settings are saved locally to EditorPrefs and will NOT be committed to Git. Safe to change on your laptop!", MessageType.Info);

            int currentLevel = QualitySettings.GetQualityLevel();
            string[] names = QualitySettings.names;

            GUILayout.Space(10);
            GUILayout.Label($"Current Active Profile: {names[currentLevel]}");

            GUILayout.Space(10);
            for (int i = 0; i < names.Length; i++)
            {
                if (GUILayout.Button($"Switch to: {names[i]}"))
                {
                    QualitySettings.SetQualityLevel(i, true);
                    EditorPrefs.SetInt(EditorPrefKey, i);
                    Debug.Log($"[HD-2D Settings] Switched quality profile to {names[i]} on this machine.");
                }
            }
        }

        private static void ApplySavedQualityProfile()
        {
            if (EditorPrefs.HasKey(EditorPrefKey))
            {
                int savedLevel = EditorPrefs.GetInt(EditorPrefKey);
                if (savedLevel >= 0 && savedLevel < QualitySettings.names.Length)
                {
                    QualitySettings.SetQualityLevel(savedLevel, true);
                    Debug.Log($"[HD-2D Settings] Auto-applied saved local quality profile: {QualitySettings.names[savedLevel]}");
                }
            }
            else
            {
                Debug.Log("[HD-2D Settings] No local quality profile saved yet. Open Tools > HD-2D Settings > Machine Quality Switcher to set one.");
            }
        }
    }
}
