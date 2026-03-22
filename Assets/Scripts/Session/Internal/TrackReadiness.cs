namespace R8EOX.Session.Internal
{
    internal struct TrackReadiness
    {
        internal bool HasSpawnPoints;
        internal bool HasMultipleSpawnPoints;
        internal bool HasCheckpoints;
        internal bool HasCenterline;
        internal bool HasFinishTrigger;

        internal bool IsPlayable => HasSpawnPoints;

        internal bool IsTimeTrialReady(TrackType trackType)
        {
            if (trackType == TrackType.Circuit)
                return HasSpawnPoints && HasCheckpoints;
            return HasSpawnPoints && HasFinishTrigger;
        }

        internal bool IsRaceReady(TrackType trackType)
        {
            if (trackType == TrackType.Circuit)
                return HasSpawnPoints && HasMultipleSpawnPoints && HasCheckpoints && HasCenterline;
            return HasSpawnPoints && HasMultipleSpawnPoints && HasFinishTrigger && HasCenterline;
        }

        internal string GetMissingReport(SessionMode mode, TrackType trackType)
        {
            var missing = new System.Collections.Generic.List<string>();

            if (!HasSpawnPoints)
                missing.Add("SpawnPoint(s)");

            if (mode == SessionMode.Race && !HasMultipleSpawnPoints)
                missing.Add("multiple SpawnPoints (for AI)");

            if (mode != SessionMode.Practice)
            {
                if (trackType == TrackType.Circuit && !HasCheckpoints)
                    missing.Add("Checkpoints (circuit lap tracking)");
                if (trackType == TrackType.PointToPoint && !HasFinishTrigger)
                    missing.Add("Finish trigger (point-to-point)");
            }

            if (mode == SessionMode.Race && !HasCenterline)
                missing.Add("Centerline (AI pathing)");

            return missing.Count == 0
                ? "Track is ready"
                : "Missing: " + string.Join(", ", missing);
        }
    }
}
