using UnityEngine;

namespace R8EOX.AI.Internal
{
    internal class AIBehavior
    {
        private float aggression;
        private float consistency;
        private bool isDrafting;

        internal void Initialize(int difficulty)
        {
            aggression = Mathf.Clamp01(difficulty / 10f);
            consistency = Mathf.Clamp01(0.5f + difficulty / 20f);
        }

        internal float ModifyThrottle(float baseThrottle)
        {
            // TODO: Add slight randomness based on consistency
            return baseThrottle * consistency;
        }

        internal bool ShouldAttemptOvertake(float distanceToCarAhead)
        {
            // TODO: Decide based on aggression and distance
            return distanceToCarAhead < 10f && aggression > 0.5f;
        }

        internal bool ShouldDraft(float distanceToCarAhead)
        {
            return distanceToCarAhead < 20f && distanceToCarAhead > 5f;
        }
    }
}
