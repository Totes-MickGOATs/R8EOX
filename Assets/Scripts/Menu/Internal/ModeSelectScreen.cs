using System;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    [RequireComponent(typeof(CanvasGroup))]
    internal class ModeSelectScreen : MenuScreen
    {
        [Header("Buttons")]
        [SerializeField] private Button testingButton;
        [SerializeField] private Button raceButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private Button backButton;

        private Action<SessionMode> onModeSelected;
        private Action onBackPressed;

        // ----------------------------------------------------------------
        // Public API
        // ----------------------------------------------------------------

        internal void Initialize(Action<SessionMode> modeCallback, Action backCallback)
        {
            onModeSelected = modeCallback;
            onBackPressed  = backCallback;

            testingButton.onClick.AddListener(HandleTesting);
            raceButton.onClick.AddListener(HandleRace);
            multiplayerButton.onClick.AddListener(HandleMultiplayer);
            backButton.onClick.AddListener(HandleBack);

            raceButton.interactable        = false;
            multiplayerButton.interactable = false;
        }

        // ----------------------------------------------------------------
        // MenuScreen overrides
        // ----------------------------------------------------------------

        internal override void OnEnter()
        {
            // Reserved for enter animation (e.g. stagger-fade buttons in)
        }

        internal override void OnExit()
        {
            // Reserved for exit cleanup
        }

        // ----------------------------------------------------------------
        // Button handlers
        // ----------------------------------------------------------------

        private void HandleTesting()
        {
            onModeSelected?.Invoke(SessionMode.Practice);
        }

        private void HandleRace()
        {
            onModeSelected?.Invoke(SessionMode.Race);
        }

        private void HandleMultiplayer()
        {
            // Locked -- no-op until multiplayer is implemented
        }

        private void HandleBack()
        {
            onBackPressed?.Invoke();
        }

        // ----------------------------------------------------------------
        // Cleanup
        // ----------------------------------------------------------------

        private void OnDestroy()
        {
            if (testingButton != null)
                testingButton.onClick.RemoveListener(HandleTesting);
            if (raceButton != null)
                raceButton.onClick.RemoveListener(HandleRace);
            if (multiplayerButton != null)
                multiplayerButton.onClick.RemoveListener(HandleMultiplayer);
            if (backButton != null)
                backButton.onClick.RemoveListener(HandleBack);
        }
    }
}
