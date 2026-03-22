using System;
using UnityEngine;

namespace R8EOX.Track.Internal
{
    [Serializable]
    internal struct GridGrouping
    {
        [Tooltip("Size of each group. Must sum to the grid dimension (columns or rows).")]
        [SerializeField] internal int[] groupSizes;

        [Tooltip("Extra spacing inserted between adjacent groups.")]
        [SerializeField] internal float gapBetweenGroups;
    }
}
