using System;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    internal class TrackSelectScreen : MenuScreen
    {
        [Header("Panels")]
        [SerializeField] private TrackListPanel trackListPanel;
        [SerializeField] private TrackPreviewPanel trackPreviewPanel;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button backButton;

        private Action<TrackDefinition> onTrackConfirmed;
        private Action onBackPressed;
        private TrackDefinition selectedTrack;
        private SessionMode currentMode;

        internal void Initialize(
            TrackRegistry registry,
            SessionMode mode,
            Action<TrackDefinition> confirmCallback,
            Action backCallback)
        {
            onTrackConfirmed = confirmCallback;
            onBackPressed = backCallback;
            currentMode = mode;

            TrackDefinition[] tracks = registry.GetAll();
            trackListPanel.Initialize(tracks, OnTrackSelected);

            startButton.onClick.AddListener(HandleStart);
            backButton.onClick.AddListener(HandleBack);

            if (tracks != null && tracks.Length > 0)
                OnTrackSelected(tracks[0]);
            else
                startButton.interactable = false;
        }

        internal override void OnExit()
        {
            trackListPanel.Teardown();
        }

        private void OnTrackSelected(TrackDefinition track)
        {
            selectedTrack = track;
            trackPreviewPanel.UpdatePreview(track, currentMode);
            startButton.interactable = track.IsPlayable && track.SupportsMode(currentMode);
        }

        private void HandleStart()
        {
            if (selectedTrack == null)
                return;

            if (!selectedTrack.IsPlayable || !selectedTrack.SupportsMode(currentMode))
                return;

            onTrackConfirmed?.Invoke(selectedTrack);
        }

        private void HandleBack()
        {
            onBackPressed?.Invoke();
        }

        private void OnDestroy()
        {
            if (startButton != null)
                startButton.onClick.RemoveListener(HandleStart);

            if (backButton != null)
                backButton.onClick.RemoveListener(HandleBack);
        }
    }
}
