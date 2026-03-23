using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace R8EOX.Settings.Internal
{
    internal static class VideoApplier
    {
        /// <summary>
        /// Apply all video settings. Caller must supply the post-process Volume
        /// to avoid banned scene-search APIs. Pass null to skip
        /// motion blur toggling (graceful degradation).
        /// </summary>
        internal static void Apply(VideoSettings settings, Volume postProcessVolume)
        {
            ApplyQualityTier(settings.QualityTier, postProcessVolume);
            ApplyWindowMode(settings.WindowMode, settings.ResolutionWidth, settings.ResolutionHeight);
            ApplyVSync(settings.VSync);
            ApplyFpsCap(settings.FpsCap);
            // Apply render scale last — overrides the tier default with the user's stored value.
            ApplyRenderScale(settings.RenderScale);
        }

        // -------------------------------------------------------------------------
        // Quality tier
        // -------------------------------------------------------------------------

        private static void ApplyQualityTier(QualityTier tier, Volume postProcessVolume)
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null)
            {
                Debug.LogWarning("[VideoApplier] No URP asset found — skipping quality tier.");
                return;
            }

            switch (tier)
            {
                case QualityTier.Ultra:
                    urpAsset.mainLightShadowmapResolution = 4096;
                    urpAsset.shadowDistance = 70f;
                    urpAsset.renderScale = 1.0f;
                    SetSsaoEnabled(urpAsset, true);
                    SetMotionBlurEnabled(postProcessVolume, true);
                    break;

                case QualityTier.High:
                    urpAsset.mainLightShadowmapResolution = 2048;
                    urpAsset.shadowDistance = 40f;
                    urpAsset.renderScale = 1.0f;
                    SetSsaoEnabled(urpAsset, true);
                    SetMotionBlurEnabled(postProcessVolume, true);
                    break;

                case QualityTier.Balanced:
                    urpAsset.mainLightShadowmapResolution = 1024;
                    urpAsset.shadowDistance = 25f;
                    urpAsset.renderScale = 0.85f;
                    SetSsaoEnabled(urpAsset, false);
                    SetMotionBlurEnabled(postProcessVolume, false);
                    break;

                case QualityTier.Performance:
                    urpAsset.mainLightShadowmapResolution = 512;
                    urpAsset.shadowDistance = 15f;
                    urpAsset.renderScale = 0.7f;
                    SetSsaoEnabled(urpAsset, false);
                    SetMotionBlurEnabled(postProcessVolume, false);
                    break;
            }
        }

        // -------------------------------------------------------------------------
        // SSAO — renderer feature, lives on ScriptableRendererData
        // -------------------------------------------------------------------------

        private static void SetSsaoEnabled(UniversalRenderPipelineAsset urpAsset, bool enabled)
        {
            var dataList = urpAsset.rendererDataList;
            bool found = false;

            foreach (var rendererData in dataList)
            {
                if (rendererData == null) continue;
                if (!rendererData.TryGetRendererFeature<ScreenSpaceAmbientOcclusion>(out var ssao)) continue;
                ssao.SetActive(enabled);
                found = true;
            }

            if (!found)
            {
                Debug.LogWarning("[VideoApplier] SSAO renderer feature not found — skipping.");
            }
        }

        // -------------------------------------------------------------------------
        // Motion blur — volume component, lives on a Volume in the scene
        // -------------------------------------------------------------------------

        private static void SetMotionBlurEnabled(Volume postProcessVolume, bool enabled)
        {
            // Volume reference is injected by the caller (SettingsManager).
            // This sidesteps banned scene-search APIs in runtime scripts.
            if (postProcessVolume == null || postProcessVolume.profile == null)
            {
                Debug.LogWarning("[VideoApplier] No post-process Volume supplied — skipping motion blur toggle.");
                return;
            }

            if (!postProcessVolume.profile.TryGet<MotionBlur>(out var motionBlur))
            {
                Debug.LogWarning("[VideoApplier] MotionBlur not found in Volume profile — skipping.");
                return;
            }

            motionBlur.active = enabled;
        }

        // -------------------------------------------------------------------------
        // Window mode
        // -------------------------------------------------------------------------

        private static void ApplyWindowMode(WindowMode mode, int width, int height)
        {
            FullScreenMode fullScreenMode = mode switch
            {
                WindowMode.Fullscreen          => FullScreenMode.ExclusiveFullScreen,
                WindowMode.BorderlessFullscreen => FullScreenMode.FullScreenWindow,
                WindowMode.Windowed            => FullScreenMode.Windowed,
                _                              => FullScreenMode.FullScreenWindow
            };

            Screen.SetResolution(width, height, fullScreenMode);
        }

        // -------------------------------------------------------------------------
        // VSync
        // -------------------------------------------------------------------------

        private static void ApplyVSync(bool vSync)
        {
            QualitySettings.vSyncCount = vSync ? 1 : 0;
        }

        // -------------------------------------------------------------------------
        // FPS cap
        // -------------------------------------------------------------------------

        private static void ApplyFpsCap(int fpsCap)
        {
            Application.targetFrameRate = fpsCap <= 0 ? -1 : fpsCap;
        }

        // -------------------------------------------------------------------------
        // Render scale — applied after quality tier so user value wins
        // -------------------------------------------------------------------------

        private static void ApplyRenderScale(float renderScale)
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null) return;
            urpAsset.renderScale = Mathf.Clamp(renderScale, 0.25f, 2.0f);
        }
    }
}
