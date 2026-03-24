using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace R8EOX.Settings.Internal
{
    internal static class VideoApplier
    {
        /// <summary>Apply all video settings to the engine.</summary>
        internal static void Apply(VideoSettings settings)
        {
            ApplyQualityTier(settings.QualityTier);
            ApplyWindowMode(settings.WindowMode, settings.ResolutionWidth, settings.ResolutionHeight);
            ApplyVSync(settings.VSync);
            ApplyFpsCap(settings.FpsCap);
            // Apply render scale last — overrides the tier default with the user's stored value.
            ApplyRenderScale(settings.RenderScale);
        }

        // -------------------------------------------------------------------------
        // Quality tier
        // -------------------------------------------------------------------------

        private static void ApplyQualityTier(QualityTier tier)
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
                    break;

                case QualityTier.High:
                    urpAsset.mainLightShadowmapResolution = 2048;
                    urpAsset.shadowDistance = 40f;
                    urpAsset.renderScale = 1.0f;
                    SetSsaoEnabled(urpAsset, true);
                    break;

                case QualityTier.Balanced:
                    urpAsset.mainLightShadowmapResolution = 1024;
                    urpAsset.shadowDistance = 25f;
                    urpAsset.renderScale = 0.85f;
                    SetSsaoEnabled(urpAsset, false);
                    break;

                case QualityTier.Performance:
                    urpAsset.mainLightShadowmapResolution = 512;
                    urpAsset.shadowDistance = 15f;
                    urpAsset.renderScale = 0.7f;
                    SetSsaoEnabled(urpAsset, false);
                    break;
            }
        }

        // -------------------------------------------------------------------------
        // SSAO — renderer feature, lives on ScriptableRendererData
        // -------------------------------------------------------------------------

        private static void SetSsaoEnabled(UniversalRenderPipelineAsset urpAsset, bool enabled)
        {
            // FRAGILE: rendererDataList is an internal URP API (URP 17.4.0 / Unity 6000.4.0f1).
            // It is not part of the public URP surface and may be removed or renamed in future
            // versions. If this throws, SSAO toggling silently degrades rather than crashing.
            ScriptableRendererData[] dataList;
            try
            {
                dataList = urpAsset.rendererDataList;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[VideoApplier] rendererDataList inaccessible — SSAO toggle skipped. ({e.GetType().Name})");
                return;
            }

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

        // TODO: Motion blur toggling requires a scene-specific Volume reference. Implement
        // once a Volume injection point exists (e.g. via TrackSystem or a volume registry).

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
