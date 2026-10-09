using System;
using UnityEditor;
using UnityEngine;

namespace TennisHD2D.Editor
{
    /// <summary>
    /// Per-machine quality profile (e.g. PC vs Laptop), stored in EditorPrefs.
    /// The editor serializes the active level into ProjectSettings/QualitySettings.asset,
    /// so whenever that file is saved we temporarily switch back to the committed level
    /// and restore the local one right after. The file stays clean in git.
    /// </summary>
    [InitializeOnLoad]
    public class QualityProfileSwitcher : EditorWindow
    {
        private const string EditorPrefKey = "TennisHD2D_PreferredQualityName";
        private const string LegacyEditorPrefKey = "TennisHD2D_PreferredQualityLevel";

        /// <summary>Level that is written to QualitySettings.asset in git.</summary>
        private const string CommittedLevelName = "PC";

        static QualityProfileSwitcher()
        {
            EditorApplication.delayCall += ApplySavedQualityProfile;
        }

        [MenuItem("Tennis/Machine Quality Profile")]
        public static void ShowWindow()
        {
            GetWindow<QualityProfileSwitcher>("Machine Profile");
        }

        private void OnGUI()
        {
            GUILayout.Label("Local Machine Performance Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Saved to EditorPrefs on this machine only. QualitySettings.asset keeps '" +
                                    CommittedLevelName + "' in git.", MessageType.Info);

            string[] names = QualitySettings.names;
            GUILayout.Space(10);
            GUILayout.Label($"Current Active Profile: {names[QualitySettings.GetQualityLevel()]}");

            GUILayout.Space(10);
            foreach (string levelName in names)
            {
                if (GUILayout.Button($"Switch to: {levelName}"))
                {
                    EditorPrefs.SetString(EditorPrefKey, levelName);
                    SetLevel(levelName);
                    Debug.Log($"[Quality] Switched this machine's profile to {levelName}.");
                }
            }
        }

        private static void ApplySavedQualityProfile()
        {
            // Migrate the old index-based pref.
            if (!EditorPrefs.HasKey(EditorPrefKey) && EditorPrefs.HasKey(LegacyEditorPrefKey))
            {
                int index = EditorPrefs.GetInt(LegacyEditorPrefKey);
                if (index >= 0 && index < QualitySettings.names.Length)
                    EditorPrefs.SetString(EditorPrefKey, QualitySettings.names[index]);
                EditorPrefs.DeleteKey(LegacyEditorPrefKey);
            }

            if (!EditorPrefs.HasKey(EditorPrefKey))
            {
                Debug.Log("[Quality] No local quality profile saved yet. Use Tennis > Machine Quality Profile to pick one.");
                return;
            }

            string saved = EditorPrefs.GetString(EditorPrefKey);
            if (SetLevel(saved))
                Debug.Log($"[Quality] Applied local quality profile: {saved}");
            else
                Debug.LogWarning($"[Quality] Saved profile '{saved}' no longer exists. Pick one in Tennis > Machine Quality Profile.");
        }

        private static bool SetLevel(string levelName)
        {
            int index = Array.IndexOf(QualitySettings.names, levelName);
            if (index < 0) return false;
            if (QualitySettings.GetQualityLevel() != index)
                QualitySettings.SetQualityLevel(index, true);
            return true;
        }

        /// <summary>Keeps the local level out of QualitySettings.asset when project settings are saved.</summary>
        private class SaveGuard : UnityEditor.AssetModificationProcessor
        {
            private static string[] OnWillSaveAssets(string[] paths)
            {
                if (Array.IndexOf(paths, "ProjectSettings/QualitySettings.asset") < 0) return paths;

                string local = QualitySettings.names[QualitySettings.GetQualityLevel()];
                if (local == CommittedLevelName || !SetLevel(CommittedLevelName)) return paths;

                EditorApplication.delayCall += () => SetLevel(local);
                return paths;
            }
        }
    }
}
