namespace R8EOX.Race.Internal
{
    internal class RaceState
    {
        private RacePhase currentPhase = RacePhase.PreRace;
        private float countdownTimer;

        internal RacePhase CurrentPhase => currentPhase;
        internal float CountdownRemaining => countdownTimer;

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
                    countdownTimer = 0f;
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
            countdownTimer = 0f;
        }
    }
}
