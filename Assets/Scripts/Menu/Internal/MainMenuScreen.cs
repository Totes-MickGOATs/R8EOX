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
        private Action onOptionsPressed;
        private Action onQuitPressed;

        // -------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------

        internal void Initialize(Action playCallback, Action optionsCallback, Action quitCallback)
        {
            onPlayPressed = playCallback;
            onOptionsPressed = optionsCallback;
            onQuitPressed = quitCallback;

            playButton.onClick.AddListener(HandlePlay);
            optionsButton.onClick.AddListener(HandleOptions);
            optionsButton.interactable = true;
            quitButton.onClick.AddListener(HandleQuit);
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

            if (optionsButton != null)
                optionsButton.onClick.RemoveListener(HandleOptions);

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

        private void HandleOptions()
        {
            onOptionsPressed?.Invoke();
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
