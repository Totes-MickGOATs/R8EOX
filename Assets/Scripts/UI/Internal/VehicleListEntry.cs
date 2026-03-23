using System;
using UnityEngine;

namespace R8EOX.UI.Internal
{
    internal class VehicleListEntry : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image thumbnailImage;
        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        [SerializeField] private TMPro.TextMeshProUGUI categoryText;
        [SerializeField] private UnityEngine.UI.Image highlightBackground;
        [SerializeField] private Color normalColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        [SerializeField] private Color selectedColor = new Color(0.2f, 0.4f, 0.8f, 1f);
        [SerializeField] private Color brokenColor = new Color(0.8f, 0.2f, 0.2f, 1f);

        private Action<int> onClicked;
        private int index;

        internal void Configure(int entryIndex, VehicleDefinition definition, Action<int> clickCallback, bool isBroken = false)
        {
            index = entryIndex;
            onClicked = clickCallback;

            if (definition == null) return;

            if (nameText != null)
            {
                nameText.text = isBroken
                    ? $"{definition.DisplayName} [Missing Prefab]"
                    : definition.DisplayName;
                if (isBroken) nameText.color = brokenColor;
            }

            if (categoryText != null)
                categoryText.text = definition.Category.ToString();

            if (thumbnailImage != null)
            {
                if (definition.Thumbnail != null)
                {
                    thumbnailImage.sprite = definition.Thumbnail;
                    thumbnailImage.enabled = true;
                }
                else
                {
                    thumbnailImage.enabled = false;
                }
            }

            SetSelected(false);
        }

        internal void SetSelected(bool selected)
        {
            if (highlightBackground != null)
                highlightBackground.color = selected ? selectedColor : normalColor;
        }

        public void OnButtonClicked()
        {
            onClicked?.Invoke(index);
        }
    }
}
