using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace R8EOX.Editor
{
    public static class SaveAllUtility
    {
        [MenuItem("R8EOX/Save All")]
        public static void SaveAll()
        {
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            Debug.Log("[SaveAll] Scenes and assets saved.");
        }
    }
}
