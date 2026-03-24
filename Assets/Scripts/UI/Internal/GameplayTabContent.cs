using UnityEngine;
using R8EOX.Settings;

namespace R8EOX.UI.Internal
{
    internal class GameplayTabContent : MonoBehaviour
    {
        private SettingsManager settingsManager;

        internal void Initialize(SettingsManager settings)
        {
            settingsManager = settings;
            BuildUI();
        }

        private void BuildUI()
        {
            OptionsUIFactory.SetupTabLayout(gameObject);

            var gameplay = settingsManager.GetGameplaySettings();

            OptionsUIFactory.CreateSectionHeader(transform, "HUD Overlays");
            OptionsUIFactory.CreateCheckboxRow(transform, "Show HUD",
                gameplay.ShowHUD, v => OnToggleChanged("hud", v));
            OptionsUIFactory.CreateCheckboxRow(transform, "Show Debug Overlay",
                gameplay.ShowDebugOverlay, v => OnToggleChanged("debug", v));
        }

        private void OnToggleChanged(string key, bool value)
        {
            settingsManager.SetGameplaySettings(g =>
            {
                switch (key)
                {
                    case "hud": g.ShowHUD = value; break;
                    case "debug": g.ShowDebugOverlay = value; break;
                }
            });
        }
    }
}
