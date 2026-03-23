#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UCamera = UnityEngine.Camera;

namespace R8EOX.Editor.Builders
{
    internal static class MenuSceneBuilder
    {
        private static readonly Color PanelColor     = new Color(0.078f, 0.082f, 0.102f);
        private static readonly Color BackgroundColor = new Color(0.051f, 0.055f, 0.071f);

        [MenuItem("R8EOX/Build Menu Scene")]
        private static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            var canvas = CreateCanvas();

            var menuManagerGo = new GameObject("[MenuManager]");
            menuManagerGo.transform.SetParent(canvas.transform, false);
            var manager = menuManagerGo.AddComponent<R8EOX.Menu.MenuManager>();

            var splashGo  = CreatePanel("SplashPanel",      canvas.transform);
            var mainGo    = CreatePanel("MainMenuPanel",    canvas.transform);
            var modeGo    = CreatePanel("ModeSelectPanel",  canvas.transform);
            var trackGo   = CreatePanel("TrackSelectPanel", canvas.transform);
            var loadingGo = CreatePanel("LoadingPanel",     canvas.transform);

            var splash      = splashGo.AddComponent<R8EOX.Menu.Internal.SplashScreen>();
            var mainMenu    = mainGo.AddComponent<R8EOX.Menu.Internal.MainMenuScreen>();
            var modeSelect  = modeGo.AddComponent<R8EOX.Menu.Internal.ModeSelectScreen>();
            var trackSelect = trackGo.AddComponent<R8EOX.Menu.Internal.TrackSelectScreen>();
            var loading     = loadingGo.AddComponent<R8EOX.Menu.Internal.TrackLoadingScreen>();

            BuildSplashPanel(splashGo.transform, splash);
            BuildMainMenuPanel(mainGo.transform, mainMenu);
            BuildModeSelectPanel(modeGo.transform, modeSelect);
            BuildTrackSelectPanel(trackGo.transform, trackSelect);
            BuildLoadingPanel(loadingGo.transform, loading);

            Wire(manager, "splashScreen",      splash);
            Wire(manager, "mainMenuScreen",    mainMenu);
            Wire(manager, "modeSelectScreen",  modeSelect);
            Wire(manager, "trackSelectScreen", trackSelect);
            Wire(manager, "loadingScreen",     loading);

            foreach (var p in new[] { splashGo, mainGo, modeGo, trackGo, loadingGo })
                p.SetActive(false);

            const string scenePath = "Assets/Scenes/MainMenu.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            UpdateBuildSettings(scenePath);
            Debug.Log("[MenuSceneBuilder] MainMenu scene saved to " + scenePath);
        }

        private static void CreateCamera()
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<UCamera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = BackgroundColor;
        }

        private static GameObject CreateCanvas()
        {
            var go = new GameObject("[MenuCanvas]");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        private static void BuildSplashPanel(Transform p, R8EOX.Menu.Internal.SplashScreen s)
        {
            var titleGo = CreateLabel("TitleText", "R8EO-X", p);
            var titleTmp = titleGo.GetComponent<TextMeshProUGUI>();
            titleTmp.fontSize = 72f;
            CenterAnchor(titleGo.GetComponent<RectTransform>());

            var promptGo = CreateLabel("PromptText", "PRESS ANY KEY", p);
            var promptCg = promptGo.AddComponent<CanvasGroup>();

            var so = new SerializedObject(s);
            so.FindProperty("titleTransform").objectReferenceValue = titleGo.GetComponent<RectTransform>();
            so.FindProperty("promptGroup").objectReferenceValue    = promptCg;
            so.ApplyModifiedProperties();
        }

        private static void BuildMainMenuPanel(Transform p, R8EOX.Menu.Internal.MainMenuScreen s)
        {
            var playGo    = CreateButton("PlayButton",    "PLAY",    p);
            var optionsGo = CreateButton("OptionsButton", "OPTIONS", p);
            var quitGo    = CreateButton("QuitButton",   "QUIT",    p);
            var so = new SerializedObject(s);
            so.FindProperty("playButton").objectReferenceValue    = playGo.GetComponent<Button>();
            so.FindProperty("optionsButton").objectReferenceValue = optionsGo.GetComponent<Button>();
            so.FindProperty("quitButton").objectReferenceValue    = quitGo.GetComponent<Button>();
            so.ApplyModifiedProperties();
        }

        private static void BuildModeSelectPanel(Transform p, R8EOX.Menu.Internal.ModeSelectScreen s)
        {
            var testGo  = CreateButton("TestingButton",     "TESTING SESSION", p);
            var raceGo  = CreateButton("RaceButton",        "RACE",            p);
            var multiGo = CreateButton("MultiplayerButton", "MULTIPLAYER",     p);
            var backGo  = CreateButton("BackButton",        "BACK",            p);
            var so = new SerializedObject(s);
            so.FindProperty("testingButton").objectReferenceValue     = testGo.GetComponent<Button>();
            so.FindProperty("raceButton").objectReferenceValue        = raceGo.GetComponent<Button>();
            so.FindProperty("multiplayerButton").objectReferenceValue = multiGo.GetComponent<Button>();
            so.FindProperty("backButton").objectReferenceValue        = backGo.GetComponent<Button>();
            so.ApplyModifiedProperties();
        }

        private static void BuildTrackSelectPanel(Transform p, R8EOX.Menu.Internal.TrackSelectScreen s)
        {
            var listAreaGo = CreatePanel("TrackListArea", p);
            var tlp = listAreaGo.AddComponent<R8EOX.Menu.Internal.TrackListPanel>();
            var listContentGo = new GameObject("ListContent");
            listContentGo.transform.SetParent(listAreaGo.transform, false);
            StretchFill(listContentGo.AddComponent<RectTransform>());

            var previewAreaGo = CreatePanel("TrackPreviewArea", p);
            var tpp = previewAreaGo.AddComponent<R8EOX.Menu.Internal.TrackPreviewPanel>();
            BuildTrackPreviewContent(previewAreaGo.transform, tpp);

            var startGo = CreateButton("StartButton",      "START", p);
            var backGo  = CreateButton("BackButton_Track", "BACK",  p);

            var soList = new SerializedObject(tlp);
            soList.FindProperty("listContent").objectReferenceValue = listContentGo.GetComponent<Transform>();
            soList.ApplyModifiedProperties();

            var so = new SerializedObject(s);
            so.FindProperty("trackListPanel").objectReferenceValue    = tlp;
            so.FindProperty("trackPreviewPanel").objectReferenceValue = tpp;
            so.FindProperty("startButton").objectReferenceValue       = startGo.GetComponent<Button>();
            so.FindProperty("backButton").objectReferenceValue        = backGo.GetComponent<Button>();
            so.ApplyModifiedProperties();
        }

        private static void BuildTrackPreviewContent(Transform p, R8EOX.Menu.Internal.TrackPreviewPanel pp)
        {
            var imgGo = new GameObject("PreviewImage");
            imgGo.transform.SetParent(p, false);
            StretchFill(imgGo.AddComponent<RectTransform>());
            var previewImg = imgGo.AddComponent<Image>();

            var nameGo   = CreateLabel("TrackNameLabel",   string.Empty, p);
            var typeGo   = CreateLabel("TrackTypeLabel",   string.Empty, p);
            var descGo   = CreateLabel("DescriptionLabel", string.Empty, p);
            var statusGo = CreateLabel("StatusLabel",      string.Empty, p);

            var indGo = new GameObject("StatusIndicator");
            indGo.transform.SetParent(p, false);
            indGo.AddComponent<RectTransform>().sizeDelta = new Vector2(16f, 16f);
            var indImg = indGo.AddComponent<Image>();
            indImg.color = Color.green;

            var emptyGo = new GameObject("EmptyState");
            emptyGo.transform.SetParent(p, false);
            StretchFill(emptyGo.AddComponent<RectTransform>());
            var emptyTmp = emptyGo.AddComponent<TextMeshProUGUI>();
            emptyTmp.text = "Select a track";
            emptyTmp.alignment = TextAlignmentOptions.Center;

            var so = new SerializedObject(pp);
            so.FindProperty("previewImage").objectReferenceValue      = previewImg;
            so.FindProperty("trackNameLabel").objectReferenceValue    = nameGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("trackTypeLabel").objectReferenceValue    = typeGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("descriptionLabel").objectReferenceValue  = descGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("statusLabel").objectReferenceValue       = statusGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("statusIndicator").objectReferenceValue   = indImg;
            so.FindProperty("emptyState").objectReferenceValue        = emptyGo;
            so.ApplyModifiedProperties();
        }

        private static void BuildLoadingPanel(Transform p, R8EOX.Menu.Internal.TrackLoadingScreen s)
        {
            var fillGo = new GameObject("ProgressFill");
            fillGo.transform.SetParent(p, false);
            StretchFill(fillGo.AddComponent<RectTransform>());
            var fillImg = fillGo.AddComponent<Image>();
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillAmount = 0f;
            fillImg.color = new Color(0.2f, 0.6f, 1f);

            var progressGo = CreateLabel("ProgressLabel", "Loading...", p);
            var tipGo      = CreateLabel("TipLabel",      string.Empty, p);

            var so = new SerializedObject(s);
            so.FindProperty("progressFill").objectReferenceValue  = fillImg;
            so.FindProperty("progressLabel").objectReferenceValue = progressGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("tipLabel").objectReferenceValue      = tipGo.GetComponent<TextMeshProUGUI>();
            so.ApplyModifiedProperties();
        }

        // ------------------------------------------------------------------ //
        // Helpers
        // ------------------------------------------------------------------ //

        private static GameObject CreatePanel(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            StretchFill(go.AddComponent<RectTransform>());
            var cg = go.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            return go;
        }

        private static GameObject CreateButton(string name, string label, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            StretchFill(go.AddComponent<RectTransform>());
            go.AddComponent<Image>().color = PanelColor;
            go.AddComponent<Button>();

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            StretchFill(textGo.AddComponent<RectTransform>());
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 24f;
            return go;
        }

        private static GameObject CreateLabel(string name, string text, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            StretchFill(go.AddComponent<RectTransform>());
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 24f;
            return go;
        }

        private static void StretchFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void CenterAnchor(RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
        }

        private static void Wire(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedProperties();
        }

        private static void UpdateBuildSettings(string menuPath)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.RemoveAll(s => s.path == menuPath);
            int insertAt = scenes.Count > 0 ? 1 : 0;
            scenes.Insert(insertAt, new EditorBuildSettingsScene(menuPath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[MenuSceneBuilder] Build Settings updated. MainMenu at index " + insertAt);
        }
    }
}
#endif
