using UnityEngine;

namespace R8EOX.Audio.Internal
{
    internal class TireSound : MonoBehaviour
    {
        [SerializeField] private AudioSource tireSource;

        internal void UpdateTireAudio(float slipAmount)
        {
            // TODO: Play/adjust skid sound based on tire slip
            if (tireSource != null)
            {
                tireSource.volume = Mathf.Clamp01(slipAmount);
            }
        }

        internal void SetSurface(string surfaceType)
        {
            // TODO: Switch tire audio clip based on surface type
        }
    }
}
