using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace R8EOX.Menu.Internal
{
    internal class MenuButton : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text label;

        [Header("Style")]
        [SerializeField] private MenuButtonStyle style = MenuButtonStyle.Primary;

        private Color normalColor;
        private Color hoverColor;
        private Color textColor;
        private bool isLocked;

        internal MenuButtonStyle Style => style;

        internal void Configure(
            MenuButtonStyle buttonStyle,
            Color bgColor, Color hoverBgColor,
            Color labelColor, string text, bool locked = false)
        {
            style = buttonStyle;
            normalColor = bgColor;
            hoverColor = hoverBgColor;
            textColor = labelColor;
            isLocked = locked;

            if (backgroundImage != null)
                backgroundImage.color = normalColor;

            if (label != null)
            {
                label.color = textColor;
                label.text = text;
            }

            var button = GetComponent<Button>();
            if (button != null)
                button.interactable = !isLocked;
        }

        internal void SetText(string text)
        {
            if (label != null)
                label.text = text;
        }

        internal void SetLocked(bool locked)
        {
            isLocked = locked;

            var button = GetComponent<Button>();
            if (button != null)
                button.interactable = !isLocked;

            if (backgroundImage != null)
            {
                backgroundImage.color = isLocked
                    ? new Color(normalColor.r, normalColor.g, normalColor.b, 0.4f)
                    : normalColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isLocked) return;
            if (backgroundImage != null)
                backgroundImage.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = isLocked
                    ? new Color(normalColor.r, normalColor.g, normalColor.b, 0.4f)
                    : normalColor;
            }
        }
    }
}
