#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Null-guarded helpers for wiring serialized references on
    /// <see cref="R8EOX.Session.Internal.SessionBootstrapper"/> from editor builders.
    /// </summary>
    internal static class BootstrapWirer
    {
        /// <summary>
        /// Assigns all manager references onto <paramref name="bootstrapper"/> via
        /// SerializedObject, then wires the session channel and default vehicle.
        /// Caller is responsible for calling <c>so.ApplyModifiedProperties()</c> is NOT
        /// required — this method applies internally.
        /// </summary>
        internal static void WireBootstrapper(
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

            SetRef(so, bootstrapper, "trackManager",  trackManager);
            SetRef(so, bootstrapper, "cameraManager", cameraManager);
            SetRef(so, bootstrapper, "raceManager",   raceManager);
            SetRef(so, bootstrapper, "uiManager",     uiManager);
            SetRef(so, bootstrapper, "audioManager",  audioManager);
            SetRef(so, bootstrapper, "vfxManager",    vfxManager);
            SetRef(so, bootstrapper, "aiManager",     aiManager);

            WireSessionChannel(so, bootstrapper);
            WireDefaultVehicle(so, bootstrapper);

            so.ApplyModifiedProperties();
        }

        // ---- private helpers ----

        private static void WireSessionChannel(
            SerializedObject so, Object ctx)
        {
            var prop = so.FindProperty("sessionChannel");
            if (prop == null)
            {
                Debug.LogError("[BootstrapWirer] Property 'sessionChannel' not found on " +
                    ctx.GetType().Name);
                return;
            }
            if (prop.objectReferenceValue != null)
                return;

            var guids = AssetDatabase.FindAssets("t:SessionChannel");
            if (guids.Length == 0)
                return;

            prop.objectReferenceValue = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        private static void WireDefaultVehicle(
            SerializedObject so, Object ctx)
        {
            var prop = so.FindProperty("defaultVehiclePrefab");
            if (prop == null)
            {
                Debug.LogError("[BootstrapWirer] Property 'defaultVehiclePrefab' not found on " +
                    ctx.GetType().Name);
                return;
            }
            if (prop.objectReferenceValue != null)
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

            var prefabGuids = AssetDatabase.FindAssets("t:Prefab",
                new[] { "Assets/Prefabs" });
            if (prefabGuids.Length > 0)
            {
                prop.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(
                    AssetDatabase.GUIDToAssetPath(prefabGuids[0]));
            }
        }

        private static void SetRef(SerializedObject so, Object ctx,
            string propName, Object value)
        {
            var prop = so.FindProperty(propName);
            if (prop == null)
            {
                Debug.LogError($"[BootstrapWirer] Property '{propName}' not found on " +
                    ctx.GetType().Name);
                return;
            }
            prop.objectReferenceValue = value;
        }
    }
}
#endif
