#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace R8EOX.Diagnostics.Internal
{
    internal struct LifecycleRequest
    {
        public readonly Object Target;
        public readonly string Label;
        public readonly float RequestTime;
        public readonly int FrameRequested;

        public LifecycleRequest(Object target, string label, float requestTime)
        {
            Target = target;
            Label = label;
            RequestTime = requestTime;
            FrameRequested = Time.frameCount;
        }

        public bool IsTargetDestroyed => Target == null;
    }
}
#endif
