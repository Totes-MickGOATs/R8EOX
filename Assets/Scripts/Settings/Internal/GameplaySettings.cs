using System;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class GameplaySettings
    {
        [UnityEngine.SerializeField] private bool showHUD = true;
        [UnityEngine.SerializeField] private bool showDebugOverlay = false;

        public bool ShowHUD { get => showHUD; set => showHUD = value; }

        public bool ShowDebugOverlay { get => showDebugOverlay; set => showDebugOverlay = value; }

        public static GameplaySettings CreateDefault()
        {
            return new GameplaySettings
            {
                showHUD = true,
                showDebugOverlay = false
            };
        }

        public GameplaySettings Clone()
        {
            return new GameplaySettings
            {
                showHUD = showHUD,
                showDebugOverlay = showDebugOverlay
            };
        }
    }
}
