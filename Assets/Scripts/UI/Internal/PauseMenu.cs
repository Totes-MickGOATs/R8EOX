using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class PauseMenu : MonoBehaviour
    {
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
            // TODO: Return to main menu
        }
    }
}
