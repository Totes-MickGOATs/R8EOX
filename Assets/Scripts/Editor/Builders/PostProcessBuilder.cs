#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    internal static class PostProcessBuilder
    {
        private const string LogPrefix = "[PostProcessBuilder]";

        // ------------------------------------------------------------------ //
        // Entry point
        // ------------------------------------------------------------------ //

        internal static void SetupPostProcessing(
            string generatedFolder,
            string trackName,
            EnvironmentSettings settings)
        {
            string profilePath = $"{generatedFolder}/{trackName}VolumeProfile.asset";
            VolumeProfile profile = CreateOrLoadProfile(profilePath);

            ConfigureBloom(profile, settings);
            ConfigureColorAdjustments(profile, settings);
            ConfigureVignette(profile, settings);
            ConfigureWhiteBalance(profile, settings);
            ConfigureTonemapping(profile);

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();

            PlaceGlobalVolume(profile);

            Debug.Log($"{LogPrefix} Post-processing configured: {profilePath}");
        }

        // ------------------------------------------------------------------ //
        // Profile helpers
        // ------------------------------------------------------------------ //

        private static VolumeProfile CreateOrLoadProfile(string path)
        {
            VolumeProfile existing = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);

            if (existing != null)
            {
                // Clear old components before rebuilding
                for (int i = existing.components.Count - 1; i >= 0; i--)
                {
                    VolumeComponent component = existing.components[i];
                    Object.DestroyImmediate(component, true);
                }
                existing.components.Clear();
                Debug.Log($"{LogPrefix} Cleared existing profile: {path}");
                return existing;
            }

            VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);
            Debug.Log($"{LogPrefix} Created new profile: {path}");
            return profile;
        }

        private static T GetOrAdd<T>(VolumeProfile profile) where T : VolumeComponent
        {
            if (profile.TryGet<T>(out T existing))
                return existing;

            return profile.Add<T>(overrides: true);
        }

        // ------------------------------------------------------------------ //
        // Effect configurators
        // ------------------------------------------------------------------ //

        private static void ConfigureBloom(VolumeProfile profile, EnvironmentSettings settings)
        {
            Bloom bloom = GetOrAdd<Bloom>(profile);

            bloom.threshold.value = settings.BloomThreshold;
            bloom.threshold.overrideState = true;

            bloom.intensity.value = settings.BloomIntensity;
            bloom.intensity.overrideState = true;

            bloom.scatter.value = settings.BloomScatter;
            bloom.scatter.overrideState = true;
        }

        private static void ConfigureColorAdjustments(
            VolumeProfile profile, EnvironmentSettings settings)
        {
            ColorAdjustments ca = GetOrAdd<ColorAdjustments>(profile);

            ca.postExposure.value = settings.PostExposure;
            ca.postExposure.overrideState = true;

            ca.contrast.value = settings.Contrast;
            ca.contrast.overrideState = true;

            ca.saturation.value = settings.Saturation;
            ca.saturation.overrideState = true;
        }

        private static void ConfigureVignette(VolumeProfile profile, EnvironmentSettings settings)
        {
            Vignette vignette = GetOrAdd<Vignette>(profile);

            vignette.intensity.value = settings.VignetteIntensity;
            vignette.intensity.overrideState = true;
        }

        private static void ConfigureWhiteBalance(
            VolumeProfile profile, EnvironmentSettings settings)
        {
            WhiteBalance wb = GetOrAdd<WhiteBalance>(profile);

            wb.temperature.value = settings.WhiteBalanceTemperature;
            wb.temperature.overrideState = true;

            wb.tint.value = settings.WhiteBalanceTint;
            wb.tint.overrideState = true;
        }

        private static void ConfigureTonemapping(VolumeProfile profile)
        {
            Tonemapping tonemapping = GetOrAdd<Tonemapping>(profile);

            tonemapping.mode.value = TonemappingMode.ACES;
            tonemapping.mode.overrideState = true;
        }

        // ------------------------------------------------------------------ //
        // Scene placement
        // ------------------------------------------------------------------ //

        private static void PlaceGlobalVolume(VolumeProfile profile)
        {
            Volume volume = Object.FindAnyObjectByType<Volume>();

            if (volume == null)
            {
                GameObject go = new GameObject("Global Volume");
                volume = go.AddComponent<Volume>();
                Debug.Log($"{LogPrefix} Created new Global Volume GameObject.");
            }
            else
            {
                Debug.Log($"{LogPrefix} Using existing Volume on '{volume.gameObject.name}'.");
            }

            volume.isGlobal = true;
            volume.priority = 0f;
            volume.sharedProfile = profile;
        }
    }
}
#endif
