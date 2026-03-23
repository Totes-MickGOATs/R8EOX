using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class PauseMenu : MonoBehaviour
    {
        [SerializeField] private R8EOX.UI.UIManager uiManager;

        internal void OnChangeVehiclePressed()
        {
            Hide();
            if (uiManager != null)
                uiManager.RequestVehicleSwap();
        }

        internal void Show()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        internal void OnResumePressed()
        {
            Hide();
        }

        internal void OnQuitPressed()
        {
            Hide();
            if (uiManager != null)
                uiManager.RequestQuitToMenu();
        }
    }
}
