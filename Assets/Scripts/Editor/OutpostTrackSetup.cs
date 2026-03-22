#if UNITY_EDITOR
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo("R8EOX.Tests.EditMode")]

namespace R8EOX.Editor
{
    /// <summary>
    /// Editor tool to build the Outpost test track terrain.
    /// Delegates to TrackBuilder.Build for convention-based track setup.
    /// Legacy constants retained for reference / fallback.
    /// Use: Menu -> R8EOX -> Build Outpost Track
    /// </summary>
    public static class OutpostTrackSetup
    {
        const string k_TrackFolder = "Assets/Tracks/Outpost";

        [MenuItem("R8EOX/Build Outpost Track")]
        static void BuildOutpostTrack() => BuildOutpostTrackInternal();

        // Internal entry point -- accessible to EditMode tests
        internal static void BuildOutpostTrackInternal()
        {
            Debug.Log("[OutpostTrack] Building Outpost track...");
            TrackBuilder.Build(k_TrackFolder);
            Debug.Log("[OutpostTrack] Outpost track built successfully!");
        }
    }
}
#endif
