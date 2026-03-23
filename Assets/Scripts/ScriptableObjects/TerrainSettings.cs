using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewTerrainSettings", menuName = "R8EOX/TerrainSettings")]
    public class TerrainSettings : ScriptableObject
    {
        [Header("Dimensions")]
        [SerializeField] private float terrainWidth = 100f;
        [SerializeField] private float terrainHeight = 2f;
        [SerializeField] private float terrainLength = 100f;

        [Header("Resolution")]
        [SerializeField] private int heightmapResolution = 2049;
        [SerializeField] private int alphamapResolution = 2048;
        [SerializeField] private int detailResolution = 1024;
        [SerializeField] private int baseMapResolution = 1024;

        [Header("Rendering")]
        [SerializeField] private float heightmapPixelError = 5f;
        [SerializeField] private float basemapDistance = 1000f;
        [SerializeField] private bool drawInstanced = true;

        public float TerrainWidth => terrainWidth;
        public float TerrainHeight => terrainHeight;
        public float TerrainLength => terrainLength;
        public int HeightmapResolution => heightmapResolution;
        public int AlphamapResolution => alphamapResolution;
        public int DetailResolution => detailResolution;
        public int BaseMapResolution => baseMapResolution;
        public float HeightmapPixelError => heightmapPixelError;
        public float BasemapDistance => basemapDistance;
        public bool DrawInstanced => drawInstanced;
    }
}
