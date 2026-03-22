using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewLayerSettings", menuName = "R8EOX/LayerSettings")]
    public class LayerSettings : ScriptableObject
    {
        [Header("Tile")]
        [SerializeField] private float tileSize = 25f;

        [Header("Surface")]
        [SerializeField] private float metallic = 0f;
        [SerializeField] private float smoothness = 0f;
        [SerializeField] private float normalScale = 1.0f;

        public float TileSize => tileSize;
        public float Metallic => metallic;
        public float Smoothness => smoothness;
        public float NormalScale => normalScale;
    }
}
