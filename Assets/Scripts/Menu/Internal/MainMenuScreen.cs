using System;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    internal sealed class MainMenuScreen : MenuScreen
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;

        private Action onPlayPressed;
        private Action onQuitPressed;

        // -------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------

        internal void Initialize(Action playCallback, Action quitCallback)
        {
            onPlayPressed = playCallback;
            onQuitPressed = quitCallback;

            playButton.onClick.AddListener(HandlePlay);
            quitButton.onClick.AddListener(HandleQuit);
            optionsButton.interactable = false;
        }

        // -------------------------------------------------------------------
        // MenuScreen overrides
        // -------------------------------------------------------------------

        internal override void OnEnter()
        {
            base.OnEnter();
        }

        internal override void OnExit()
        {
            base.OnExit();
        }

        // -------------------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------------------

        private void OnDestroy()
        {
            if (playButton != null)
                playButton.onClick.RemoveListener(HandlePlay);

            if (quitButton != null)
                quitButton.onClick.RemoveListener(HandleQuit);
        }

        // -------------------------------------------------------------------
        // Private handlers
        // -------------------------------------------------------------------

        private void HandlePlay()
        {
            onPlayPressed?.Invoke();
        }

        private void HandleQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            onQuitPressed?.Invoke();
        }
    }
}
