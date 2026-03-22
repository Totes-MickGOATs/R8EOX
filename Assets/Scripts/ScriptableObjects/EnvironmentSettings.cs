using UnityEngine;
using UnityEngine.Rendering;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewEnvironmentSettings", menuName = "R8EOX/EnvironmentSettings")]
    public class EnvironmentSettings : ScriptableObject
    {
        [Header("Skybox")]
        [SerializeField] private float skyboxExposure = 1.0f;

        [Header("Fog")]
        [SerializeField] private bool fogEnabled = true;
        [SerializeField] private FogMode fogMode = FogMode.Exponential;
        [SerializeField] private float fogDensity = 0.005f;
        [SerializeField] private Color fogColor = new(0.85f, 0.75f, 0.6f);

        [Header("Ambient Light")]
        [SerializeField] private AmbientMode ambientMode = AmbientMode.Trilight;
        [SerializeField] private Color ambientSkyColor = new(0.85f, 0.75f, 0.55f);
        [SerializeField] private Color ambientEquatorColor = new(0.70f, 0.60f, 0.45f);
        [SerializeField] private Color ambientGroundColor = new(0.35f, 0.28f, 0.18f);

        [Header("Sun")]
        [SerializeField] private Color sunColor = new(1.0f, 0.92f, 0.70f);
        [SerializeField] private float sunIntensity = 1.2f;

        public float SkyboxExposure => skyboxExposure;
        public bool FogEnabled => fogEnabled;
        public FogMode FogMode => fogMode;
        public float FogDensity => fogDensity;
        public Color FogColor => fogColor;
        public AmbientMode AmbientMode => ambientMode;
        public Color AmbientSkyColor => ambientSkyColor;
        public Color AmbientEquatorColor => ambientEquatorColor;
        public Color AmbientGroundColor => ambientGroundColor;
        public Color SunColor => sunColor;
        public float SunIntensity => sunIntensity;
    }
}
