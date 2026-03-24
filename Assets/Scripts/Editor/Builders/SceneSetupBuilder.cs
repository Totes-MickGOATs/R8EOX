#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

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

            EnsureEventSystem();
            WireCameraMainRef(cameraManager);
            var trackManager = FindOrAddTrackManager(managersParent);
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

        private static R8EOX.Track.TrackManager FindOrAddTrackManager(
            GameObject parent)
        {
            var existing =
                Object.FindAnyObjectByType<R8EOX.Track.TrackManager>();
            if (existing != null)
            {
                WireTrackConfigIfNeeded(existing);
                return existing;
            }

            // Pre-find the config so we can assign it via SerializedObject
            // immediately after AddComponent — before OnValidate fires a warning.
            var configAsset = FindAnyTrackConfig();

            var go = new GameObject("TrackManager");
            go.transform.SetParent(parent.transform, false);
            var tm = go.AddComponent<R8EOX.Track.TrackManager>();

            if (configAsset != null)
            {
                var so = new SerializedObject(tm);
                var prop = so.FindProperty("config");
                if (prop != null)
                {
                    prop.objectReferenceValue = configAsset;
                    so.ApplyModifiedProperties();
                }
            }

            return tm;
        }

        private static void WireTrackConfigIfNeeded(
            R8EOX.Track.TrackManager trackManager)
        {
            var so = new SerializedObject(trackManager);
            var prop = so.FindProperty("config");
            if (prop == null || prop.objectReferenceValue != null)
                return;

            var configAsset = FindAnyTrackConfig();
            if (configAsset == null)
                return;

            prop.objectReferenceValue = configAsset;
            so.ApplyModifiedProperties();
        }

        private static ScriptableObject FindAnyTrackConfig()
        {
            var guids = AssetDatabase.FindAssets("t:TrackConfig");
            if (guids.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            return null;
        }

        private static void EnsureSpawnGrid(R8EOX.Track.TrackManager trackManager)
        {
            if (trackManager == null)
                return;

            var existing =
                trackManager.GetComponentInChildren<R8EOX.Track.Internal.SpawnGrid>();
            if (existing != null)
                return;

            // Sample terrain height at origin for spawn Y position
            float spawnY = 0.5f;
            var terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                spawnY = terrain.SampleHeight(Vector3.zero)
                    + terrain.transform.position.y + 0.3f;
            }

            var go = new GameObject("SpawnGrid");
            go.transform.SetParent(trackManager.transform, false);
            go.transform.localPosition = new Vector3(0f, spawnY, 0f);
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
            R8EOX.AI.AIManager         aiManager) =>
            BootstrapWirer.WireBootstrapper(
                bootstrapper,
                trackManager,
                cameraManager,
                raceManager,
                uiManager,
                audioManager,
                vfxManager,
                aiManager);

        // ---- Helpers ---------------------------------------------------------

        private static GameObject FindOrCreateParent(string parentName)
        {
            var existing = GameObject.Find(parentName);
            if (existing != null)
                return existing;

            return new GameObject(parentName);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null) return;
            var go = new GameObject("[EventSystem]");
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
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
            Debug.Log(
                $"[SceneSetupBuilder] Managers placed.\n" +
                $"  TrackManager:  {(trackManager  != null ? "placed" : "missing")}\n" +
                $"  CameraManager: {(cameraManager != null ? "placed" : "missing")}\n" +
                $"  RaceManager:   {(raceManager   != null ? "placed" : "missing")}\n" +
                $"  UIManager:     {(uiManager     != null ? "placed" : "missing")}\n" +
                $"  AudioManager:  {(audioManager  != null ? "placed" : "missing")}\n" +
                $"  VFXManager:    {(vfxManager    != null ? "placed" : "missing")}\n" +
                $"  AIManager:     {(aiManager     != null ? "placed" : "missing")}");
        }
    }
}
#endif
