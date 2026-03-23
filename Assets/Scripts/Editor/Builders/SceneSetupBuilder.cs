#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Places all manager GameObjects into the active track scene and
    /// wires their references into SessionBootstrapper. Idempotent —
    /// safe to run multiple times on the same scene.
    /// </summary>
    internal static class SceneSetupBuilder
    {
        private const string ManagersParentName  = "[Managers]";
        private const string BootstrapParentName = "[Bootstrap]";

        // ---- Menu Item -------------------------------------------------------

        [MenuItem("R8EOX/Scene Setup/Place Managers")]
        private static void PlaceManagersMenuItem() => PlaceManagers();

        // ---- Public API ------------------------------------------------------

        internal static void PlaceManagers()
        {
            var managersParent = FindOrCreateParent(ManagersParentName);

            var cameraManager = FindOrAdd<R8EOX.Camera.CameraManager>(
                managersParent, "CameraManager");
            var raceManager   = FindOrAdd<R8EOX.Race.RaceManager>(
                managersParent, "RaceManager");
            var uiManager     = FindOrAdd<R8EOX.UI.UIManager>(
                managersParent, "UIManager");
            var audioManager  = FindOrAdd<R8EOX.Audio.AudioManager>(
                managersParent, "AudioManager");
            var vfxManager    = FindOrAdd<R8EOX.VFX.VFXManager>(
                managersParent, "VFXManager");
            var aiManager     = FindOrAdd<R8EOX.AI.AIManager>(
                managersParent, "AIManager");

            WireCameraMainRef(cameraManager);

            var trackManager = FindOrAdd<R8EOX.Track.TrackManager>(
                managersParent, "TrackManager");
            WireTrackConfig(trackManager);
            EnsureSpawnGrid(trackManager);

            var bootstrapper = FindOrCreateBootstrapper();

            WireBootstrapper(
                bootstrapper,
                trackManager,
                cameraManager,
                raceManager,
                uiManager,
                audioManager,
                vfxManager,
                aiManager);

            EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            LogSummary(trackManager, cameraManager, raceManager,
                uiManager, audioManager, vfxManager, aiManager);
        }

        // ---- Manager placement -----------------------------------------------

        private static T FindOrAdd<T>(GameObject parent, string childName)
            where T : Component
        {
            var existing = Object.FindAnyObjectByType<T>();
            if (existing != null)
                return existing;

            var go = new GameObject(childName);
            go.transform.SetParent(parent.transform, false);
            return go.AddComponent<T>();
        }

        private static void WireCameraMainRef(R8EOX.Camera.CameraManager cameraManager)
        {
            var mainCam = UnityEngine.Camera.main;
            if (mainCam == null)
                return;

            var so   = new SerializedObject(cameraManager);
            var prop = so.FindProperty("mainCamera");
            if (prop != null && prop.objectReferenceValue == null)
            {
                prop.objectReferenceValue = mainCam;
                so.ApplyModifiedProperties();
            }
        }

        private static void WireTrackConfig(R8EOX.Track.TrackManager trackManager)
        {
            if (trackManager == null)
                return;

            var so = new SerializedObject(trackManager);
            var prop = so.FindProperty("config");
            if (prop == null || prop.objectReferenceValue != null)
                return;

            // Global fallback: find any TrackConfig in the project
            var guids = AssetDatabase.FindAssets("t:TrackConfig");
            if (guids.Length == 0)
            {
                Debug.LogWarning(
                    "[SceneSetupBuilder] No TrackConfig asset found. " +
                    "Create one via Assets > Create > R8EOX > TrackConfig");
                return;
            }

            prop.objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));
            so.ApplyModifiedProperties();
        }

        private static void EnsureSpawnGrid(R8EOX.Track.TrackManager trackManager)
        {
            if (trackManager == null)
                return;

            var existing =
                trackManager.GetComponentInChildren<R8EOX.Track.Internal.SpawnGrid>();
            if (existing != null)
                return;

            var go = new GameObject("SpawnGrid");
            go.transform.SetParent(trackManager.transform, false);
            go.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            go.AddComponent<R8EOX.Track.Internal.SpawnGrid>();
        }

        // ---- Bootstrapper ----------------------------------------------------

        private static R8EOX.Session.Internal.SessionBootstrapper FindOrCreateBootstrapper()
        {
            var existing =
                Object.FindAnyObjectByType<R8EOX.Session.Internal.SessionBootstrapper>();
            if (existing != null)
                return existing;

            var go = new GameObject(BootstrapParentName);
            return go.AddComponent<R8EOX.Session.Internal.SessionBootstrapper>();
        }

        private static void WireBootstrapper(
            R8EOX.Session.Internal.SessionBootstrapper bootstrapper,
            R8EOX.Track.TrackManager   trackManager,
            R8EOX.Camera.CameraManager cameraManager,
            R8EOX.Race.RaceManager     raceManager,
            R8EOX.UI.UIManager         uiManager,
            R8EOX.Audio.AudioManager   audioManager,
            R8EOX.VFX.VFXManager       vfxManager,
            R8EOX.AI.AIManager         aiManager)
        {
            var so = new SerializedObject(bootstrapper);

            so.FindProperty("trackManager").objectReferenceValue  = trackManager;
            so.FindProperty("cameraManager").objectReferenceValue = cameraManager;
            so.FindProperty("raceManager").objectReferenceValue   = raceManager;
            so.FindProperty("uiManager").objectReferenceValue     = uiManager;
            so.FindProperty("audioManager").objectReferenceValue  = audioManager;
            so.FindProperty("vfxManager").objectReferenceValue    = vfxManager;
            so.FindProperty("aiManager").objectReferenceValue     = aiManager;

            WireSessionChannel(so);
            WireDefaultVehicle(so);

            so.ApplyModifiedProperties();
        }

        private static void WireSessionChannel(SerializedObject so)
        {
            var prop = so.FindProperty("sessionChannel");
            if (prop == null || prop.objectReferenceValue != null)
                return;

            var guids = AssetDatabase.FindAssets("t:SessionChannel");
            if (guids.Length == 0)
                return;

            prop.objectReferenceValue = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        private static void WireDefaultVehicle(SerializedObject so)
        {
            var prop = so.FindProperty("defaultVehiclePrefab");
            if (prop == null || prop.objectReferenceValue != null)
                return;

            var defGuids = AssetDatabase.FindAssets("t:VehicleDefinition");
            if (defGuids.Length > 0)
            {
                var def = AssetDatabase.LoadAssetAtPath<VehicleDefinition>(
                    AssetDatabase.GUIDToAssetPath(defGuids[0]));
                if (def != null && def.VehiclePrefab != null)
                {
                    prop.objectReferenceValue = def.VehiclePrefab;
                    return;
                }
            }

            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
            if (prefabGuids.Length > 0)
            {
                prop.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(
                    AssetDatabase.GUIDToAssetPath(prefabGuids[0]));
            }
        }

        // ---- Helpers ---------------------------------------------------------

        private static GameObject FindOrCreateParent(string parentName)
        {
            var existing = GameObject.Find(parentName);
            if (existing != null)
                return existing;

            return new GameObject(parentName);
        }

        private static void LogSummary(
            R8EOX.Track.TrackManager   trackManager,
            R8EOX.Camera.CameraManager cameraManager,
            R8EOX.Race.RaceManager     raceManager,
            R8EOX.UI.UIManager         uiManager,
            R8EOX.Audio.AudioManager   audioManager,
            R8EOX.VFX.VFXManager       vfxManager,
            R8EOX.AI.AIManager         aiManager)
        {
            string track = trackManager  != null ? "placed" : "missing";
            string cam   = cameraManager != null ? "placed" : "missing";
            string race  = raceManager   != null ? "placed" : "missing";
            string ui    = uiManager     != null ? "placed" : "missing";
            string audio = audioManager  != null ? "placed" : "missing";
            string vfx   = vfxManager    != null ? "placed" : "missing";
            string ai    = aiManager     != null ? "placed" : "missing";

            Debug.Log(
                $"[SceneSetupBuilder] Managers placed.\n" +
                $"  TrackManager:  {track}\n" +
                $"  CameraManager: {cam}\n" +
                $"  RaceManager:   {race}\n" +
                $"  UIManager:     {ui}\n" +
                $"  AudioManager:  {audio}\n" +
                $"  VFXManager:    {vfx}\n" +
                $"  AIManager:     {ai}");
        }
    }
}
#endif
