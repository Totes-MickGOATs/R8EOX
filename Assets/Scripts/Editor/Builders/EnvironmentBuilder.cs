#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace R8EOX.Editor.Builders
{
    /// <summary>
    /// Configures the scene environment: skybox, fog, ambient lighting, and sun.
    /// Reads overrides from an optional EnvironmentSettings ScriptableObject.
    /// </summary>
    internal static class EnvironmentBuilder
    {
        // -- Defaults matching EnvironmentSettings ScriptableObject defaults --
        const float k_DefaultExposure = 1.0f;
        const float k_DefaultFogDensity = 0.005f;
        const float k_DefaultSunIntensity = 1.2f;

        static readonly Color k_DefaultFogColor = new(0.85f, 0.75f, 0.6f);
        static readonly Color k_DefaultAmbientSky = new(0.85f, 0.75f, 0.55f);
        static readonly Color k_DefaultAmbientEquator = new(0.70f, 0.60f, 0.45f);
        static readonly Color k_DefaultAmbientGround = new(0.35f, 0.28f, 0.18f);
        static readonly Color k_DefaultSunColor = new(1.0f, 0.92f, 0.70f);

        /// <summary>
        /// Set up environment using convention-based paths and optional settings.
        /// Skybox material path = {generatedFolder}/{trackName}Skybox.mat
        /// </summary>
        internal static void SetupEnvironment(
            string skyboxHdrPath,
            string generatedFolder,
            string trackName,
            EnvironmentSettings settings)
        {
            string skyboxMaterialPath =
                $"{generatedFolder}/{trackName}Skybox.mat";

            float exposure = settings != null
                ? settings.SkyboxExposure : k_DefaultExposure;
            bool fogEnabled = settings != null
                ? settings.FogEnabled : true;
            FogMode fogMode = settings != null
                ? settings.FogMode : FogMode.Exponential;
            float fogDensity = settings != null
                ? settings.FogDensity : k_DefaultFogDensity;
            Color fogColor = settings != null
                ? settings.FogColor : k_DefaultFogColor;
            Color ambientSky = settings != null
                ? settings.AmbientSkyColor : k_DefaultAmbientSky;
            Color ambientEquator = settings != null
                ? settings.AmbientEquatorColor : k_DefaultAmbientEquator;
            Color ambientGround = settings != null
                ? settings.AmbientGroundColor : k_DefaultAmbientGround;
            Color sunColor = settings != null
                ? settings.SunColor : k_DefaultSunColor;
            float sunIntensity = settings != null
                ? settings.SunIntensity : k_DefaultSunIntensity;

            AmbientMode ambientMode = settings != null
                ? settings.AmbientMode : AmbientMode.Trilight;

            SetupSkybox(skyboxHdrPath, skyboxMaterialPath, exposure);
            SetupFog(fogEnabled, fogMode, fogColor, fogDensity);
            SetupAmbientLighting(ambientMode, ambientSky, ambientEquator, ambientGround);
            SetupDirectionalLight(sunColor, sunIntensity);
        }

        /// <summary>
        /// Legacy entry point kept for backwards compatibility.
        /// </summary>
        internal static void SetupDesertEnvironment(
            string skyboxHdriPath, string skyboxMaterialPath,
            float fogDensity, float sunIntensity)
        {
            SetupSkybox(skyboxHdriPath, skyboxMaterialPath, k_DefaultExposure);
            SetupFog(true, FogMode.Exponential, k_DefaultFogColor, fogDensity);
            SetupAmbientLighting(
                AmbientMode.Trilight,
                k_DefaultAmbientSky, k_DefaultAmbientEquator,
                k_DefaultAmbientGround);
            SetupDirectionalLight(k_DefaultSunColor, sunIntensity);
        }

        static void SetupSkybox(
            string skyboxHdriPath, string skyboxMaterialPath, float exposure)
        {
            var hdriImporter =
                AssetImporter.GetAtPath(skyboxHdriPath) as TextureImporter;
            if (hdriImporter != null)
            {
                bool needsReimport =
                    hdriImporter.textureShape != TextureImporterShape.Texture2D
                    || hdriImporter.sRGBTexture;
                if (needsReimport)
                {
                    hdriImporter.textureShape = TextureImporterShape.Texture2D;
                    hdriImporter.sRGBTexture = false;
                    hdriImporter.SaveAndReimport();
                }
            }

            Texture2D hdriTex =
                AssetDatabase.LoadAssetAtPath<Texture2D>(skyboxHdriPath);
            if (hdriTex == null)
            {
                Debug.LogWarning(
                    "[TrackBuilder] HDRI not found at " + skyboxHdriPath +
                    " -- skipping skybox setup.");
                return;
            }

            Material skyboxMat = new Material(Shader.Find("Skybox/Panoramic"));
            skyboxMat.SetTexture("_MainTex", hdriTex);
            skyboxMat.SetFloat("_Exposure", exposure);
            skyboxMat.SetFloat("_Mapping", 1f);
            skyboxMat.SetFloat("_ImageType", 0f);
            AssetHelper.SaveOrReplaceAsset(skyboxMat, skyboxMaterialPath);
            RenderSettings.skybox = skyboxMat;
            Debug.Log("[TrackBuilder] HDRI panoramic skybox applied.");
        }

        static void SetupFog(
            bool enabled, FogMode mode, Color color, float density)
        {
            RenderSettings.fog = enabled;
            RenderSettings.fogMode = mode;
            RenderSettings.fogColor = color;
            RenderSettings.fogDensity = density;
            Debug.Log(
                $"[TrackBuilder] Fog configured (mode={mode}, density={density}).");
        }

        static void SetupAmbientLighting(
            AmbientMode mode, Color sky, Color equator, Color ground)
        {
            RenderSettings.ambientMode = mode;
            RenderSettings.ambientSkyColor = sky;
            RenderSettings.ambientEquatorColor = equator;
            RenderSettings.ambientGroundColor = ground;
            Debug.Log("[TrackBuilder] Ambient trilight configured.");
        }

        static void SetupDirectionalLight(Color color, float intensity)
        {
            Light sun = FindDirectionalLight();

            if (sun == null)
            {
                var go = new GameObject("Directional Light");
                sun = go.AddComponent<Light>();
                sun.type = LightType.Directional;
                go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                Debug.Log("[EnvironmentBuilder] Created Directional Light.");
            }

            sun.color = color;
            sun.intensity = intensity;
            Debug.Log("[EnvironmentBuilder] Sun color/intensity applied.");
        }

        static Light FindDirectionalLight()
        {
            GameObject sunGO = GameObject.Find("Directional Light");
            if (sunGO != null)
            {
                Light light = sunGO.GetComponent<Light>();
                if (light != null && light.type == LightType.Directional)
                    return light;
            }

            // Fallback: search all lights in case named differently
            foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (light.type == LightType.Directional)
                    return light;
            }

            return null;
        }
    }
}
#endif
