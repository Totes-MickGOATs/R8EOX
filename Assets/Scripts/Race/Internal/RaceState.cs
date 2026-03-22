namespace R8EOX.Race.Internal
{
    internal enum RacePhase
    {
        PreRace,
        Countdown,
        Racing,
        Finished
    }

    internal class RaceState
    {
        private RacePhase currentPhase = RacePhase.PreRace;
        private float countdownTimer;

        internal RacePhase CurrentPhase => currentPhase;

        internal void BeginCountdown(float duration)
        {
            currentPhase = RacePhase.Countdown;
            countdownTimer = duration;
        }

        internal void Tick(float deltaTime)
        {
            if (currentPhase == RacePhase.Countdown)
            {
                countdownTimer -= deltaTime;
                if (countdownTimer <= 0f)
                {
                    currentPhase = RacePhase.Racing;
                }
            }
        }

        internal void Finish()
        {
            currentPhase = RacePhase.Finished;
        }

        internal void Reset()
        {
            currentPhase = RacePhase.PreRace;
        }
    }
}
