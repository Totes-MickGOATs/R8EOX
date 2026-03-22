using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class VehicleStatsDisplay : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image topSpeedFill;
        [SerializeField] private UnityEngine.UI.Image accelerationFill;
        [SerializeField] private UnityEngine.UI.Image handlingFill;
        [SerializeField] private UnityEngine.UI.Image weightFill;
        [SerializeField] private TMPro.TextMeshProUGUI descriptionText;

        internal void UpdateStats(VehicleDefinition definition)
        {
            if (definition == null)
            {
                ClearStats();
                return;
            }

            var stats = definition.Stats;

            if (topSpeedFill != null)
                topSpeedFill.fillAmount = stats.TopSpeed;

            if (accelerationFill != null)
                accelerationFill.fillAmount = stats.Acceleration;

            if (handlingFill != null)
                handlingFill.fillAmount = stats.Handling;

            if (weightFill != null)
                weightFill.fillAmount = stats.Weight;

            if (descriptionText != null)
                descriptionText.text = definition.Description;
        }

        private void ClearStats()
        {
            if (topSpeedFill != null)
                topSpeedFill.fillAmount = 0f;

            if (accelerationFill != null)
                accelerationFill.fillAmount = 0f;

            if (handlingFill != null)
                handlingFill.fillAmount = 0f;

            if (weightFill != null)
                weightFill.fillAmount = 0f;

            if (descriptionText != null)
                descriptionText.text = string.Empty;
        }
    }
}
