#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal class SpawnGrid : MonoBehaviour
    {
        [Header("Grid Dimensions")]
        [SerializeField] private int columns = 2;
        [SerializeField] private int rows = 3;

        [Header("Spacing")]
        [SerializeField] private float columnSpacing = 2.5f;
        [SerializeField] private float rowSpacing = 4f;

        [Header("Fill Order")]
        [SerializeField] private FillDirection fillDirection = FillDirection.LeftToRight;
        [SerializeField] private RowOrder rowOrder = RowOrder.FrontToBack;

        [Header("Stagger")]
        [SerializeField] private StaggerMode staggerMode = StaggerMode.Alternating;
        [SerializeField] private float rowStagger = 0f;

        [Header("Column Grouping")]
        [SerializeField] private GridGrouping columnGrouping;

        [Header("Row Grouping")]
        [SerializeField] private GridGrouping rowGrouping;

        private const float k_WarnThreshold = 0.5f;
        private const float k_ErrorThreshold = 2.0f;

        internal SpawnPointData[] ComputeSpawnPoints()
        {
            return SpawnGridMath.ComputeSpawnPoints(
                transform.position,
                transform.rotation,
                columns,
                rows,
                columnSpacing,
                rowSpacing,
                fillDirection,
                rowOrder,
                staggerMode,
                rowStagger,
                columnGrouping,
                rowGrouping);
        }

        private void OnValidate()
        {
            if (columns < 1) columns = 1;
            if (rows < 1) rows = 1;
            if (columnSpacing < 0f) columnSpacing = 0f;
            if (rowSpacing < 0f) rowSpacing = 0f;

            if (columnGrouping.groupSizes != null
                && columnGrouping.groupSizes.Length > 0
                && !SpawnGridMath.ValidateGrouping(columnGrouping, columns))
            {
                Debug.LogWarning(
                    $"[SpawnGrid] Column group sizes do not sum to {columns}.");
            }

            if (rowGrouping.groupSizes != null
                && rowGrouping.groupSizes.Length > 0
                && !SpawnGridMath.ValidateGrouping(rowGrouping, rows))
            {
                Debug.LogWarning(
                    $"[SpawnGrid] Row group sizes do not sum to {rows}.");
            }
        }

        private void OnDrawGizmos()
        {
            SpawnPointData[] points = ComputeSpawnPoints();
            if (points == null || points.Length == 0) return;

            Terrain terrain = Terrain.activeTerrain;

#if UNITY_EDITOR
            var labelStyle = new GUIStyle
            {
                normal = { textColor = Color.white },
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
#endif

            // Grid center marker.
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.3f);

            // Spawn point markers.
            for (int i = 0; i < points.Length; i++)
            {
                SpawnPointData point = points[i];

                Color pointColor = point.Index == 0 ? Color.green : Color.yellow;
                float delta = 0f;
                float terrainY = 0f;
                if (terrain != null)
                {
                    terrainY = terrain.SampleHeight(point.Position)
                        + terrain.transform.position.y;
                    delta = terrainY - point.Position.y;
                    if (delta > k_ErrorThreshold)
                        pointColor = Color.red;
                    else if (delta > k_WarnThreshold)
                        pointColor = new Color(1f, 0.5f, 0f);
                }

                Gizmos.color = pointColor;
                Gizmos.DrawWireSphere(point.Position, 0.4f);

                Vector3 forward = point.Rotation * Vector3.forward;
                Gizmos.DrawRay(point.Position, forward * 1.5f);

                if (delta > k_WarnThreshold)
                {
                    Vector3 terrainPos = new Vector3(
                        point.Position.x, terrainY, point.Position.z);
                    Gizmos.DrawLine(point.Position, terrainPos);
                }

#if UNITY_EDITOR
                Handles.Label(
                    point.Position + Vector3.up * 0.7f,
                    point.Index.ToString(),
                    labelStyle);
#endif
            }

            // Grid outline connecting corner spawn points.
            if (columns >= 1 && rows >= 1)
            {
                DrawGridOutline(points);
            }
        }

        private void DrawGridOutline(SpawnPointData[] points)
        {
            int totalPoints = points.Length;
            if (totalPoints < 2) return;

            Gizmos.color = new Color(1f, 1f, 1f, 0.3f);

            // Corner indices: first row first col, first row last col,
            // last row first col, last row last col.
            int topLeft = 0;
            int topRight = columns - 1;
            int bottomLeft = totalPoints - columns;
            int bottomRight = totalPoints - 1;

            // Top edge.
            Gizmos.DrawLine(points[topLeft].Position, points[topRight].Position);
            // Bottom edge.
            Gizmos.DrawLine(points[bottomLeft].Position, points[bottomRight].Position);
            // Left edge.
            Gizmos.DrawLine(points[topLeft].Position, points[bottomLeft].Position);
            // Right edge.
            Gizmos.DrawLine(points[topRight].Position, points[bottomRight].Position);
        }
    }
}
