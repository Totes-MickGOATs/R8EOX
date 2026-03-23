using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    internal class OptionsOverlay : MonoBehaviour
    {
        private Settings.SettingsManager settingsManager;
        private Action onClosed;
        private CanvasGroup canvasGroup;

        private OptionsTabBar tabBar;
        private GameObject[] tabPanels;
        private OptionsTab activeTab;

        private VideoTabContent videoTab;
        private AudioTabContent audioTab;
        private ControlsTabContent controlsTab;
        private CalibrationTabContent calibrationTab;
        private GameplayTabContent gameplayTab;
        private ProfileTabContent profileTab;
        private R8EOX.UI.ToastManager toastRef;

        internal void Show(
            Settings.SettingsManager settings, Action onClosedCallback,
            R8EOX.UI.ToastManager toast = null)
        {
            settingsManager = settings;
            onClosed = onClosedCallback;
            toastRef = toast;
            BuildUI();
            InitializeTabs();
            gameObject.SetActive(true);
            StartCoroutine(AnimateOpen());
        }

        internal void Hide()
        {
            StartCoroutine(AnimateClose());
        }

        private void BuildUI()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();

            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            var backdrop = CreateChild("Backdrop");
            StretchFill(backdrop);
            backdrop.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

            var panel = CreateChild("Panel");
            var panelRT = panel.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.sizeDelta = new Vector2(840f, 620f);
            panel.AddComponent<Image>().color = new Color(0.078f, 0.082f, 0.102f);

            var vg = panel.AddComponent<VerticalLayoutGroup>();
            vg.padding = new RectOffset(24, 24, 20, 12);
            vg.spacing = 8f;
            vg.childForceExpandWidth = true;
            vg.childForceExpandHeight = false;

            BuildHeader(panel.transform);
            BuildTabBar(panel.transform);
            BuildContentArea(panel.transform);
            BuildBackButton(panel.transform);
        }

        private void BuildHeader(Transform parent)
        {
            var header = CreateChild("Header", parent);
            header.AddComponent<LayoutElement>().preferredHeight = 40f;
            var tmp = header.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = "OPTIONS";
            tmp.fontSize = 40f;
            tmp.color = Color.white;
        }

        private void BuildTabBar(Transform parent)
        {
            var tabBarGO = CreateChild("TabBar", parent);
            tabBarGO.AddComponent<LayoutElement>().preferredHeight = 36f;
            tabBar = tabBarGO.AddComponent<OptionsTabBar>();
        }

        private void BuildContentArea(Transform parent)
        {
            var contentArea = CreateChild("ContentArea", parent);
            contentArea.AddComponent<LayoutElement>().flexibleHeight = 1f;

            tabPanels = new GameObject[6];
            string[] panelNames = { "Video", "Audio", "Controls", "Calibration", "Gameplay", "Profile" };

            for (int i = 0; i < 6; i++)
            {
                tabPanels[i] = CreateChild(panelNames[i] + "Panel", contentArea.transform);
                StretchFill(tabPanels[i]);

                var scrollView = tabPanels[i].AddComponent<ScrollRect>();
                scrollView.vertical = true;
                scrollView.horizontal = false;

                var scrollContent = CreateChild("Content", tabPanels[i].transform);
                StretchFill(scrollContent);

                var fitter = scrollContent.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                var scrollVG = scrollContent.AddComponent<VerticalLayoutGroup>();
                scrollVG.childForceExpandWidth = true;
                scrollVG.childForceExpandHeight = false;
                scrollVG.spacing = 4f;

                scrollView.content = scrollContent.GetComponent<RectTransform>();
                tabPanels[i].SetActive(false);
            }

            var c0 = tabPanels[0].GetComponentInChildren<ScrollRect>().content.gameObject;
            var c1 = tabPanels[1].GetComponentInChildren<ScrollRect>().content.gameObject;
            var c2 = tabPanels[2].GetComponentInChildren<ScrollRect>().content.gameObject;
            var c3 = tabPanels[3].GetComponentInChildren<ScrollRect>().content.gameObject;
            var c4 = tabPanels[4].GetComponentInChildren<ScrollRect>().content.gameObject;
            var c5 = tabPanels[5].GetComponentInChildren<ScrollRect>().content.gameObject;

            videoTab       = c0.AddComponent<VideoTabContent>();
            audioTab       = c1.AddComponent<AudioTabContent>();
            controlsTab    = c2.AddComponent<ControlsTabContent>();
            calibrationTab = c3.AddComponent<CalibrationTabContent>();
            gameplayTab    = c4.AddComponent<GameplayTabContent>();
            profileTab     = c5.AddComponent<ProfileTabContent>();
        }

        private void BuildBackButton(Transform parent)
        {
            var backGO = CreateChild("BackButton", parent);
            backGO.AddComponent<LayoutElement>().preferredHeight = 52f;
            backGO.AddComponent<Image>().color = new Color(0.078f, 0.082f, 0.102f);

            var btn = backGO.AddComponent<Button>();
            btn.onClick.AddListener(OnBackPressed);

            var backTxt = CreateChild("Text", backGO.transform).AddComponent<TMPro.TextMeshProUGUI>();
            backTxt.text = "BACK";
            backTxt.fontSize = 20f;
            backTxt.color = Color.white;
            backTxt.alignment = TMPro.TextAlignmentOptions.Center;
            StretchFill(backTxt.gameObject);
        }

        private void InitializeTabs()
        {
            videoTab.Initialize(settingsManager);
            audioTab.Initialize(settingsManager);
            controlsTab.Initialize(settingsManager);
            calibrationTab.Initialize(settingsManager);
            gameplayTab.Initialize(settingsManager);
            profileTab.Initialize(settingsManager, toastRef);

            tabBar.Initialize(OnTabChanged);
            OnTabChanged(OptionsTab.Video);
        }

        private void OnTabChanged(OptionsTab tab)
        {
            activeTab = tab;
            for (int i = 0; i < tabPanels.Length; i++)
                tabPanels[i].SetActive(i == (int)tab);
        }

        private void OnBackPressed() => Hide();

        private void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
                OnBackPressed();
        }

        private IEnumerator AnimateOpen()
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;
            const float duration = 0.25f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private IEnumerator AnimateClose()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            float elapsed = 0f;
            const float duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            onClosed?.Invoke();
        }

        private GameObject CreateChild(string childName, Transform parent = null)
        {
            var go = new GameObject(childName, typeof(RectTransform));
            go.transform.SetParent(parent ?? transform, false);
            return go;
        }

        private static void StretchFill(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
