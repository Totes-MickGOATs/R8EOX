using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    [RequireComponent(typeof(Button))]
    internal class TrackListEntry : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private TMP_Text nameLabel;

        [Header("Status")]
        [Tooltip("Green = playable, yellow = partial, red = locked or missing scene")]
        [SerializeField] private Image statusIcon;

        [Header("Selection")]
        [SerializeField] private Image highlightOverlay;

        private static readonly Color ColorPlayable = new Color(0.18f, 0.80f, 0.24f);
        private static readonly Color ColorWarning   = new Color(0.95f, 0.80f, 0.10f);
        private static readonly Color ColorLocked    = new Color(0.85f, 0.18f, 0.18f);

        private int entryIndex;
        private Action<int> onClicked;

        internal void Configure(int index, TrackDefinition track, Action<int> clickCallback)
        {
            entryIndex = index;
            onClicked  = clickCallback;

            nameLabel.text = track.DisplayName;

            if (thumbnailImage != null && track.Thumbnail != null)
            {
                thumbnailImage.sprite = track.Thumbnail;
            }

            if (statusIcon != null)
            {
                statusIcon.color = ResolveStatusColor(track);
            }

            GetComponent<Button>().onClick.AddListener(HandleClick);

            SetSelected(false);
        }

        internal void SetSelected(bool selected)
        {
            if (highlightOverlay == null)
            {
                return;
            }

            Color c = highlightOverlay.color;
            c.a = selected ? 0.2f : 0f;
            highlightOverlay.color = c;
        }

        private static Color ResolveStatusColor(TrackDefinition track)
        {
            if (track.IsLocked || string.IsNullOrEmpty(track.SceneName))
            {
                return ColorLocked;
            }

            return track.IsPlayable ? ColorPlayable : ColorWarning;
        }

        private void HandleClick()
        {
            onClicked?.Invoke(entryIndex);
        }

        private void OnDestroy()
        {
            var button = GetComponent<Button>();
            if (button != null)
                button.onClick.RemoveListener(HandleClick);
        }
    }
}
