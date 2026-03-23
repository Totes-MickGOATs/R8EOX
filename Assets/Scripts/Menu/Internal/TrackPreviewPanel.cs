using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    internal class TrackPreviewPanel : MonoBehaviour
    {
        [Header("Preview Content")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TMP_Text trackNameLabel;
        [SerializeField] private TMP_Text trackTypeLabel;
        [SerializeField] private TMP_Text descriptionLabel;

        [Header("Status")]
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private Image statusIndicator;

        [Header("Empty State")]
        [SerializeField] private GameObject emptyState;

        private static readonly Color ColorReady  = new Color(0.2f,  0.8f,  0.2f);
        private static readonly Color ColorWarn   = new Color(0.9f,  0.8f,  0.2f);
        private static readonly Color ColorLocked = new Color(1f,    0.318f, 0.329f);

        private TrackDefinition currentTrack;

        internal void UpdatePreview(TrackDefinition track, SessionMode mode)
        {
            currentTrack = track;

            if (emptyState != null)
                emptyState.SetActive(false);

            if (trackNameLabel != null)
                trackNameLabel.text = track.DisplayName;

            if (trackTypeLabel != null)
                trackTypeLabel.text = track.TrackType.ToString();

            if (descriptionLabel != null)
                descriptionLabel.text = track.Description ?? string.Empty;

            if (previewImage != null)
            {
                if (track.Thumbnail != null)
                {
                    previewImage.sprite = track.Thumbnail;
                    previewImage.enabled = true;
                }
                else
                {
                    previewImage.enabled = false;
                }
            }

            UpdateStatus(track, mode);
        }

        internal void Clear()
        {
            currentTrack = null;

            if (emptyState != null)
                emptyState.SetActive(true);

            if (trackNameLabel != null)
                trackNameLabel.text = string.Empty;

            if (trackTypeLabel != null)
                trackTypeLabel.text = string.Empty;

            if (descriptionLabel != null)
                descriptionLabel.text = string.Empty;

            if (statusLabel != null)
                statusLabel.text = string.Empty;

            if (previewImage != null)
                previewImage.enabled = false;
        }

        private void UpdateStatus(TrackDefinition track, SessionMode mode)
        {
            if (track.IsLocked)
            {
                SetStatus(ColorLocked, "LOCKED");
            }
            else if (string.IsNullOrEmpty(track.SceneName))
            {
                SetStatus(ColorLocked, "Scene not in Build Settings\nAdd this scene in File > Build Settings");
            }
            else if (!track.SupportsMode(mode))
            {
                SetStatus(ColorWarn, $"Not available for {mode} mode");
            }
            else
            {
                SetStatus(ColorReady, "Ready");
            }
        }

        private void SetStatus(Color color, string message)
        {
            if (statusLabel != null)
                statusLabel.text = message;

            if (statusIndicator != null)
                statusIndicator.color = color;
        }
    }
}
