using System;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class VideoSettings
    {
        [UnityEngine.SerializeField] private QualityTier qualityTier = QualityTier.High;
        [UnityEngine.SerializeField] private WindowMode windowMode = WindowMode.BorderlessFullscreen;
        [UnityEngine.SerializeField] private int resolutionWidth = 1920;
        [UnityEngine.SerializeField] private int resolutionHeight = 1080;
        [UnityEngine.SerializeField] private int monitorIndex = 0;
        [UnityEngine.SerializeField] private bool vSync = true;
        [UnityEngine.SerializeField] private int fpsCap = 0;
        [UnityEngine.SerializeField] private float renderScale = 1.0f;
        [UnityEngine.SerializeField] private UpscalingMode upscalingMode = UpscalingMode.None;

        public QualityTier QualityTier { get => qualityTier; set => qualityTier = value; }
        public WindowMode WindowMode { get => windowMode; set => windowMode = value; }
        public int ResolutionWidth { get => resolutionWidth; set => resolutionWidth = value; }
        public int ResolutionHeight { get => resolutionHeight; set => resolutionHeight = value; }
        public int MonitorIndex { get => monitorIndex; set => monitorIndex = value; }
        public bool VSync { get => vSync; set => vSync = value; }
        public int FpsCap { get => fpsCap; set => fpsCap = value; }
        public float RenderScale { get => renderScale; set => renderScale = value; }
        public UpscalingMode UpscalingMode { get => upscalingMode; set => upscalingMode = value; }

        public static VideoSettings CreateDefault()
        {
            return new VideoSettings();
        }

        public VideoSettings Clone()
        {
            return new VideoSettings
            {
                qualityTier = qualityTier,
                windowMode = windowMode,
                resolutionWidth = resolutionWidth,
                resolutionHeight = resolutionHeight,
                monitorIndex = monitorIndex,
                vSync = vSync,
                fpsCap = fpsCap,
                renderScale = renderScale,
                upscalingMode = upscalingMode
            };
        }
    }
}
