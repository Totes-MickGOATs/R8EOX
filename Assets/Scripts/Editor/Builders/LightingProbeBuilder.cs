#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace R8EOX.Editor.Builders
{
    internal static class LightingProbeBuilder
    {
        internal static void SetupLightingProbes(
            float terrainWidth, float terrainLength, float terrainHeight)
        {
            PlaceReflectionProbe(terrainWidth, terrainLength, terrainHeight);
            PlaceLightProbeGrid(terrainWidth, terrainLength);
        }

        private static void PlaceReflectionProbe(float w, float l, float h)
        {
            GameObject go = GameObject.Find("TrackReflectionProbe");
            if (go == null)
                go = new GameObject("TrackReflectionProbe");

            if (!go.TryGetComponent<ReflectionProbe>(out ReflectionProbe probe))
                probe = go.AddComponent<ReflectionProbe>();

            go.transform.position = new Vector3(w / 2f, h / 2f, l / 2f);

            probe.mode          = ReflectionProbeMode.Baked;
            probe.resolution    = 256;
            probe.size          = new Vector3(w + 50f, h + 50f, l + 50f);
            probe.boxProjection = true;
            probe.importance    = 1;

            EditorUtility.SetDirty(go);
            Debug.Log($"[LightingProbeBuilder] Reflection probe placed ({w}x{l}m coverage)");
        }

        private static void PlaceLightProbeGrid(float w, float l)
        {
            GameObject go = GameObject.Find("TrackLightProbes");
            if (go == null)
                go = new GameObject("TrackLightProbes");

            if (!go.TryGetComponent<LightProbeGroup>(out LightProbeGroup lightProbeGroup))
                lightProbeGroup = go.AddComponent<LightProbeGroup>();

            int countX = Mathf.Max(2, Mathf.RoundToInt(w / 15f) + 1);
            int countZ = Mathf.Max(2, Mathf.RoundToInt(l / 15f) + 1);

            Vector3[] positions = new Vector3[countX * countZ * 2];
            int index = 0;

            for (int zi = 0; zi < countZ; zi++)
            {
                for (int xi = 0; xi < countX; xi++)
                {
                    float x = xi * (w / (countX - 1));
                    float z = zi * (l / (countZ - 1));
                    positions[index++] = new Vector3(x, 0.5f, z);
                    positions[index++] = new Vector3(x, 3.0f, z);
                }
            }

            lightProbeGroup.probePositions = positions;

            EditorUtility.SetDirty(go);
            Debug.Log($"[LightingProbeBuilder] Light probe grid placed " +
                      $"({positions.Length} probes, {countX}x{countZ}x2 layers)");
        }
    }
}
#endif
