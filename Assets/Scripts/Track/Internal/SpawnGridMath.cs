using UnityEngine;

namespace R8EOX.Track.Internal
{
    internal static class SpawnGridMath
    {
        internal static SpawnPointData[] ComputeSpawnPoints(
            Vector3 origin,
            Quaternion rotation,
            int columns,
            int rows,
            float columnSpacing,
            float rowSpacing,
            FillDirection fillDirection,
            RowOrder rowOrder,
            StaggerMode staggerMode,
            float rowStagger,
            GridGrouping columnGrouping,
            GridGrouping rowGrouping)
        {
            float[] columnPositions = ComputeAxisPositions(columns, columnSpacing, columnGrouping);
            float[] rowPositions = ComputeAxisPositions(rows, rowSpacing, rowGrouping);

            // Negate row positions so row 0 is frontmost (positive forward).
            for (int i = 0; i < rowPositions.Length; i++)
            {
                rowPositions[i] = -rowPositions[i];
            }

            Vector3 right = rotation * Vector3.right;
            Vector3 forward = rotation * Vector3.forward;

            int totalSpawns = columns * rows;
            var results = new SpawnPointData[totalSpawns];
            int currentIndex = 0;

            int rowStart = rowOrder == RowOrder.FrontToBack ? 0 : rows - 1;
            int rowEnd = rowOrder == RowOrder.FrontToBack ? rows : -1;
            int rowStep = rowOrder == RowOrder.FrontToBack ? 1 : -1;

            int colStart = fillDirection == FillDirection.LeftToRight ? 0 : columns - 1;
            int colEnd = fillDirection == FillDirection.LeftToRight ? columns : -1;
            int colStep = fillDirection == FillDirection.LeftToRight ? 1 : -1;

            for (int r = rowStart; r != rowEnd; r += rowStep)
            {
                float stagger = ComputeStagger(r, staggerMode, rowStagger);

                for (int c = colStart; c != colEnd; c += colStep)
                {
                    Vector3 worldPos = origin
                        + right * (columnPositions[c] + stagger)
                        + forward * rowPositions[r];

                    results[currentIndex] = new SpawnPointData
                    {
                        Index = currentIndex,
                        Position = worldPos,
                        Rotation = rotation,
                        IsPlayerSpawn = currentIndex == 0
                    };
                    currentIndex++;
                }
            }

            return results;
        }

        internal static bool ValidateGrouping(GridGrouping grouping, int dimension)
        {
            if (grouping.groupSizes == null || grouping.groupSizes.Length == 0)
            {
                return true;
            }

            int sum = 0;
            for (int i = 0; i < grouping.groupSizes.Length; i++)
            {
                sum += grouping.groupSizes[i];
            }

            return sum == dimension;
        }

        private static float[] ComputeAxisPositions(
            int count, float spacing, GridGrouping grouping)
        {
            var positions = new float[count];
            bool hasGrouping = grouping.groupSizes != null && grouping.groupSizes.Length > 0;

            int groupIndex = 0;
            int usedInGroup = 0;
            float accumulated = 0f;

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    accumulated += spacing;

                    if (hasGrouping && groupIndex < grouping.groupSizes.Length)
                    {
                        if (usedInGroup >= grouping.groupSizes[groupIndex])
                        {
                            accumulated += grouping.gapBetweenGroups;
                            groupIndex++;
                            usedInGroup = 0;
                        }
                    }
                }

                positions[i] = accumulated;
                usedInGroup++;
            }

            // Center around zero.
            if (count > 1)
            {
                float midpoint = (positions[0] + positions[count - 1]) * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    positions[i] -= midpoint;
                }
            }

            return positions;
        }

        private static float ComputeStagger(int rowIndex, StaggerMode mode, float rowStagger)
        {
            if (mode == StaggerMode.Alternating)
            {
                return (rowIndex % 2 == 1) ? rowStagger : 0f;
            }

            return rowIndex * rowStagger;
        }
    }
}
