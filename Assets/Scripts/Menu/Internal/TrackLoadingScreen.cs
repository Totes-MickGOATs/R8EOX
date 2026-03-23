using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    internal class TrackLoadingScreen : MenuScreen
    {
        [Header("Progress Bar")]
        [SerializeField] private Image progressFill;

        [Header("Labels")]
        [SerializeField] private TMP_Text progressLabel;

        [Tooltip("Rotates through racing tips while the track loads.")]
        [SerializeField] private TMP_Text tipLabel;

        private readonly string[] tips =
        {
            "Brake before the corner, accelerate out",
            "Use the terrain to your advantage",
            "Watch your suspension — big jumps can damage your buggy",
            "Draft behind opponents for a speed boost",
            "Take the inside line on tight corners",
            "Smooth inputs are faster than jerky ones"
        };

        private float tipRotateInterval = 4f;
        private float tipTimer;
        private int currentTipIndex;

        internal override void OnEnter()
        {
            currentTipIndex = Random.Range(0, tips.Length);
            tipTimer = 0f;
            ShowTip(currentTipIndex);
        }

        private void Update()
        {
            tipTimer += Time.unscaledDeltaTime;
            if (tipTimer >= tipRotateInterval)
            {
                currentTipIndex = (currentTipIndex + 1) % tips.Length;
                ShowTip(currentTipIndex);
                tipTimer = 0f;
            }
        }

        internal void UpdateProgress(float progress01)
        {
            if (progressFill != null)
                progressFill.fillAmount = progress01;

            if (progressLabel != null)
                progressLabel.text = $"Loading... {Mathf.RoundToInt(progress01 * 100)}%";
        }

        internal void ResetProgress()
        {
            if (progressFill != null)
                progressFill.fillAmount = 0f;

            if (progressLabel != null)
                progressLabel.text = "Loading...";

            currentTipIndex = Random.Range(0, tips.Length);
            tipTimer = 0f;
            ShowTip(currentTipIndex);
        }

        private void ShowTip(int index)
        {
            if (tipLabel != null)
                tipLabel.text = tips[index];
        }
    }
}
