namespace R8EOX.Session.Internal
{
    internal class SessionState
    {
        private SessionPhase currentPhase = SessionPhase.Idle;

        internal SessionPhase CurrentPhase => currentPhase;

        internal void BeginLoading()
        {
            if (currentPhase == SessionPhase.Idle)
                currentPhase = SessionPhase.Loading;
        }

        internal void BeginVehicleSelect()
        {
            if (currentPhase == SessionPhase.Loading)
                currentPhase = SessionPhase.VehicleSelect;
        }

        internal void EndVehicleSelect()
        {
            if (currentPhase == SessionPhase.VehicleSelect)
                currentPhase = SessionPhase.Spawning;
        }

        internal void BeginSpawning()
        {
            if (currentPhase == SessionPhase.Loading
                || currentPhase == SessionPhase.VehicleSelect)
                currentPhase = SessionPhase.Spawning;
        }

        internal void ReturnToVehicleSelect()
        {
            if (currentPhase == SessionPhase.Ready)
                currentPhase = SessionPhase.VehicleSelect;
        }

        internal void MarkReady()
        {
            if (currentPhase == SessionPhase.Spawning)
                currentPhase = SessionPhase.Ready;
        }

        internal void BeginTeardown()
        {
            if (currentPhase != SessionPhase.Idle && currentPhase != SessionPhase.Teardown)
                currentPhase = SessionPhase.Teardown;
        }

        internal void Reset()
        {
            currentPhase = SessionPhase.Idle;
        }
    }
}
