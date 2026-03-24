using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    internal class ConfirmDialog : MonoBehaviour
    {
        // Set while any ConfirmDialog instance is visible so other overlay
        // Escape-key handlers can skip their own dismissal logic.
        internal static bool IsDialogOpen { get; private set; }

        [Header("State")]
        [SerializeField] private bool isDanger;

        private Action _onConfirm;
        private Action _onCancel;
        private CanvasGroup _canvasGroup;

        // Colors
        private static readonly Color BackdropColor     = new Color(0f, 0f, 0f, 0.7f);
        private static readonly Color PanelBgColor      = new Color(20 / 255f, 20 / 255f, 26 / 255f, 0.95f);
        private static readonly Color PanelBorderColor  = new Color(0f, 0.784f, 1f, 0.3f);
        private static readonly Color TitleColor        = new Color(0f, 0.784f, 1f, 1f);
        private static readonly Color CancelFillColor   = new Color(0.078f, 0.082f, 0.102f, 1f);
        private static readonly Color CancelBorderColor = new Color(0f, 0.784f, 1f, 0.15f);
        private static readonly Color DangerColor       = new Color(1f, 0.318f, 0.329f, 1f);
        private static readonly Color PrimaryColor      = new Color(0f, 0.784f, 1f, 1f);

        internal static ConfirmDialog Show(
            string title,
            string message,
            string confirmText,
            bool isDanger,
            Action onConfirm,
            Action onCancel = null)
        {
            var go = new GameObject("ConfirmDialog");
            DontDestroyOnLoad(go);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            go.AddComponent<GraphicRaycaster>();

            var dialog = go.AddComponent<ConfirmDialog>();
            dialog.isDanger = isDanger;
            dialog._onConfirm = onConfirm;
            dialog._onCancel = onCancel;
            dialog.Build(title, message, confirmText);
            IsDialogOpen = true;
            return dialog;
        }

        private void Build(string title, string message, string confirmText)
        {
            var rt = gameObject.GetComponent<RectTransform>();
            if (rt == null)
                rt = gameObject.AddComponent<RectTransform>();

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;

            BuildBackdrop();
            BuildPanel(title, message, confirmText);
            StartCoroutine(FadeIn());
        }

        private void BuildBackdrop()
        {
            var backdropGo = new GameObject("Backdrop");
            backdropGo.transform.SetParent(transform, false);

            var img = backdropGo.AddComponent<Image>();
            img.color = BackdropColor;

            var backdropRt = backdropGo.GetComponent<RectTransform>();
            backdropRt.anchorMin = Vector2.zero;
            backdropRt.anchorMax = Vector2.one;
            backdropRt.offsetMin = Vector2.zero;
            backdropRt.offsetMax = Vector2.zero;

            var btn = backdropGo.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(Cancel);
        }

        private void BuildPanel(string title, string message, string confirmText)
        {
            var panelGo = new GameObject("Panel");
            panelGo.transform.SetParent(transform, false);

            var img = panelGo.AddComponent<Image>();
            img.color = PanelBgColor;

            var outline = panelGo.AddComponent<Outline>();
            outline.effectColor = PanelBorderColor;
            outline.effectDistance = new Vector2(1f, -1f);

            var panelRt = panelGo.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.5f, 0.5f);
            panelRt.anchorMax = new Vector2(0.5f, 0.5f);
            panelRt.pivot = new Vector2(0.5f, 0.5f);
            panelRt.sizeDelta = new Vector2(400f, 220f);
            panelRt.anchoredPosition = Vector2.zero;

            BuildTitle(panelGo.transform, title);
            BuildMessage(panelGo.transform, message);
            BuildButtons(panelGo.transform, confirmText);
        }

        private static void BuildTitle(Transform parent, string title)
        {
            var go = new GameObject("Title");
            go.transform.SetParent(parent, false);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = title;
            tmp.fontSize = 22f;
            tmp.color = TitleColor;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.offsetMin = new Vector2(16f, 0f);
            rt.offsetMax = new Vector2(-16f, 0f);
            rt.anchoredPosition = new Vector2(0f, -24f);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 32f);
        }

        private static void BuildMessage(Transform parent, string message)
        {
            var go = new GameObject("Message");
            go.transform.SetParent(parent, false);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = message;
            tmp.fontSize = 16f;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.textWrappingMode = TMPro.TextWrappingModes.Normal;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(1f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(24f, 0f);
            rt.offsetMax = new Vector2(-24f, 0f);
            rt.anchoredPosition = new Vector2(0f, 16f);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 80f);
        }

        private void BuildButtons(Transform parent, string confirmText)
        {
            var cancelPos = new Vector2(-108f, -76f);
            BuildButton(parent, "CancelBtn", "CANCEL", CancelFillColor, CancelBorderColor, Cancel, cancelPos);

            var confirmBorderColor = isDanger ? DangerColor : PrimaryColor;
            var confirmPos = new Vector2(108f, -76f);
            BuildButton(parent, "ConfirmBtn", confirmText, CancelFillColor, confirmBorderColor, Confirm, confirmPos);
        }

        private static void BuildButton(
            Transform parent,
            string goName,
            string label,
            Color fillColor,
            Color borderColor,
            UnityEngine.Events.UnityAction onClick,
            Vector2 anchoredPos)
        {
            var go = new GameObject(goName);
            go.transform.SetParent(parent, false);

            var img = go.AddComponent<Image>();
            img.color = fillColor;

            var outline = go.AddComponent<Outline>();
            outline.effectColor = borderColor;
            outline.effectDistance = new Vector2(1f, -1f);

            var btn = go.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            var colors = btn.colors;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.12f);
            colors.pressedColor = new Color(1f, 1f, 1f, 0.2f);
            btn.colors = colors;
            btn.onClick.AddListener(onClick);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(160f, 44f);
            rt.anchoredPosition = anchoredPos;

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);

            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 14f;
            tmp.color = new Color(borderColor.r, borderColor.g, borderColor.b, 1f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                Cancel();
        }

        private void Confirm()
        {
            IsDialogOpen = false;
            _onConfirm?.Invoke();
            Destroy(gameObject);
        }

        private void Cancel()
        {
            IsDialogOpen = false;
            _onCancel?.Invoke();
            Destroy(gameObject);
        }

        private IEnumerator FadeIn()
        {
            const float duration = 0.15f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;
        }
    }
}
