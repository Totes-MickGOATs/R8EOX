using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    internal class PauseMenu : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;

        private System.Action _onOptionsPressed;
        private System.Action _onRestartPressed;
        private bool _isBuilt;

        // Colors
        private static readonly Color PanelBgColor  = new Color(20 / 255f, 20 / 255f, 26 / 255f, 0.97f);
        private static readonly Color TitleColor     = new Color(0f, 0.784f, 1f, 1f);
        private static readonly Color DividerColor   = new Color(0f, 0.784f, 1f, 0.20f);

        internal void Initialize(
            System.Action optionsCallback,
            System.Action restartCallback)
        {
            _onOptionsPressed = optionsCallback;
            _onRestartPressed = restartCallback;
        }

        internal void Show()
        {
            if (!_isBuilt) BuildUI();
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        // ── UI construction ───────────────────────────────────────────────────

        private void BuildUI()
        {
            _isBuilt = true;

            // Full-screen dark backdrop
            var backdropGo = new GameObject("Backdrop", typeof(RectTransform));
            backdropGo.transform.SetParent(transform, false);
            var backdropImg = backdropGo.AddComponent<Image>();
            backdropImg.color = new Color(0f, 0f, 0f, 0.65f);
            StretchFull(backdropGo.GetComponent<RectTransform>());

            // Centre panel
            var panelGo = new GameObject("Panel", typeof(RectTransform));
            panelGo.transform.SetParent(transform, false);
            var panelImg = panelGo.AddComponent<Image>();
            panelImg.color = PanelBgColor;
            var panelOutline = panelGo.AddComponent<Outline>();
            panelOutline.effectColor = new Color(0f, 0.784f, 1f, 0.25f);
            panelOutline.effectDistance = new Vector2(1f, -1f);

            var panelRt = panelGo.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.5f, 0.5f);
            panelRt.anchorMax = new Vector2(0.5f, 0.5f);
            panelRt.pivot     = new Vector2(0.5f, 0.5f);
            panelRt.sizeDelta = new Vector2(320f, 420f);
            panelRt.anchoredPosition = Vector2.zero;

            // Vertical layout inside panel
            var vg = panelGo.AddComponent<VerticalLayoutGroup>();
            vg.childForceExpandWidth  = true;
            vg.childForceExpandHeight = false;
            vg.spacing = 0f;
            vg.padding = new RectOffset(20, 20, 24, 24);

            panelGo.AddComponent<ContentSizeFitter>().verticalFit =
                ContentSizeFitter.FitMode.PreferredSize;

            BuildTitle(panelGo.transform);
            BuildHorizontalDivider(panelGo.transform, 12f);
            BuildButtonColumn(panelGo.transform);
        }

        private void BuildTitle(Transform parent)
        {
            var go = new GameObject("Title", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = "PAUSED";
            tmp.fontSize  = 28f;
            tmp.color     = TitleColor;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            var le = go.AddComponent<LayoutElement>();
            le.minHeight       = 44f;
            le.preferredHeight = 44f;
        }

        private static void BuildHorizontalDivider(Transform parent, float topPad)
        {
            if (topPad > 0f)
            {
                var spacer = new GameObject("Spacer", typeof(RectTransform));
                spacer.transform.SetParent(parent, false);
                var sle = spacer.AddComponent<LayoutElement>();
                sle.minHeight = topPad; sle.preferredHeight = topPad;
            }

            var go = new GameObject("Divider", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            go.AddComponent<Image>().color = DividerColor;
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 1f; le.preferredHeight = 1f;
        }

        private void BuildButtonColumn(Transform parent)
        {
            AddSpacer(parent, 16f);
            OptionsUIFactory.CreateActionButton(parent, "RESUME",
                OptionsUIFactory.STYLE_PRIMARY, OnResumePressed);
            AddSpacer(parent, 4f);
            OptionsUIFactory.CreateActionButton(parent, "OPTIONS",
                OptionsUIFactory.STYLE_SECONDARY, OnOptionsPressed);

            BuildHorizontalDivider(parent, 12f);
            AddSpacer(parent, 12f);

            OptionsUIFactory.CreateActionButton(parent, "RESTART",
                OptionsUIFactory.STYLE_SECONDARY, OnRestartPressed);
            AddSpacer(parent, 4f);
            OptionsUIFactory.CreateActionButton(parent, "CHANGE VEHICLE",
                OptionsUIFactory.STYLE_SECONDARY, OnChangeVehiclePressed);
            AddSpacer(parent, 4f);
            OptionsUIFactory.CreateActionButton(parent, "RETURN TO MENU",
                OptionsUIFactory.STYLE_SECONDARY, OnReturnToMenuPressed);
            AddSpacer(parent, 4f);
            OptionsUIFactory.CreateActionButton(parent, "QUIT TO DESKTOP",
                OptionsUIFactory.STYLE_DANGER, OnQuitToDesktopPressed);
        }

        private static void AddSpacer(Transform parent, float height)
        {
            var go = new GameObject("Spacer", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = height; le.preferredHeight = height;
        }

        private static void StretchFull(RectTransform rt)
        {
            rt.anchorMin  = Vector2.zero;
            rt.anchorMax  = Vector2.one;
            rt.offsetMin  = Vector2.zero;
            rt.offsetMax  = Vector2.zero;
        }

        // ── Button handlers ───────────────────────────────────────────────────

        private void OnResumePressed() => uiManager?.HidePauseMenu();

        private void OnOptionsPressed()
        {
            // Do not hide — options overlay renders on top of the pause menu.
            _onOptionsPressed?.Invoke();
        }

        private void OnRestartPressed()
        {
            ConfirmDialog.Show(
                "Restart Race?",
                "Restart the current race?",
                "RESTART",
                false,
                () => { uiManager?.HidePauseMenu(); _onRestartPressed?.Invoke(); });
        }

        private void OnChangeVehiclePressed()
        {
            uiManager?.HidePauseMenu();
            uiManager?.RequestVehicleSwap();
        }

        private void OnReturnToMenuPressed()
        {
            ConfirmDialog.Show(
                "Return to Menu?",
                "Progress will be lost.",
                "RETURN",
                true,
                () => { uiManager?.HidePauseMenu(); uiManager?.RequestQuitToMenu(); });
        }

        private void OnQuitToDesktopPressed()
        {
            ConfirmDialog.Show(
                "Quit to Desktop?",
                "Are you sure?",
                "QUIT",
                true,
                () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
        }
    }
}
