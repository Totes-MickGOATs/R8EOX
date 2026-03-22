using R8EOX.Track;

namespace R8EOX.Session.Internal
{
    internal static class TrackValidator
    {
        internal static TrackReadiness Validate(TrackManager trackManager)
        {
            var result = new TrackReadiness();

            if (trackManager == null)
                return result;

            result.HasSpawnPoints = trackManager.GetSpawnPointCount() > 0;
            result.HasMultipleSpawnPoints = trackManager.GetSpawnPointCount() > 1;
            result.HasCheckpoints = trackManager.GetCheckpointCount() > 0;
            result.HasCenterline = trackManager.HasCenterline();
            result.HasFinishTrigger = trackManager.GetCheckpointCount() > 0;

            return result;
        }

        internal static SessionMode DegradeMode(
            SessionMode requestedMode,
            TrackReadiness readiness,
            TrackType trackType)
        {
            switch (requestedMode)
            {
                case SessionMode.Race:
                    if (readiness.IsRaceReady(trackType))
                        return SessionMode.Race;
                    if (readiness.IsTimeTrialReady(trackType))
                    {
                        UnityEngine.Debug.LogWarning(
                            "[TrackValidator] Race mode not available. "
                            + readiness.GetMissingReport(SessionMode.Race, trackType)
                            + ". Degrading to TimeTrial.");
                        return SessionMode.TimeTrial;
                    }
                    if (readiness.IsPlayable)
                    {
                        UnityEngine.Debug.LogWarning(
                            "[TrackValidator] Race/TimeTrial not available. "
                            + readiness.GetMissingReport(SessionMode.Race, trackType)
                            + ". Degrading to Practice.");
                        return SessionMode.Practice;
                    }
                    break;

                case SessionMode.TimeTrial:
                    if (readiness.IsTimeTrialReady(trackType))
                        return SessionMode.TimeTrial;
                    if (readiness.IsPlayable)
                    {
                        UnityEngine.Debug.LogWarning(
                            "[TrackValidator] TimeTrial not available. "
                            + readiness.GetMissingReport(SessionMode.TimeTrial, trackType)
                            + ". Degrading to Practice.");
                        return SessionMode.Practice;
                    }
                    break;

                case SessionMode.Practice:
                    if (readiness.IsPlayable)
                        return SessionMode.Practice;
                    break;
            }

            UnityEngine.Debug.LogError(
                "[TrackValidator] Track is not playable! "
                + readiness.GetMissingReport(requestedMode, trackType));
            return SessionMode.Practice;
        }
    }
}
