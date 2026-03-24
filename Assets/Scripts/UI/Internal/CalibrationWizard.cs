using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    /// <summary>
    /// Modal calibration wizard. Builds its own Canvas (sortOrder=250).
    /// States: Release -> SampleNeutral -> MoveExtremes -> Results.
    /// </summary>
    internal class CalibrationWizard : MonoBehaviour
    {
        private const int StateRelease      = 0;
        private const int StateSampleNeutral = 1;
        private const int StateMoveExtremes = 2;
        private const int StateResults      = 3;
        private int _state;
        private Action<float[], float[,]> _onComplete;
        private Action _onCancel;

        private float[] _neutralAccum;
        private int     _sampleCount;
        private float[] _mins;
        private float[] _maxs;
        private float   _extremesTimer;
        private float[] _axisBuffer = new float[6];

        private const int   AxisCount       = 6;
        private const int   NeutralFrames   = 60;
        private const float ExtremesSeconds = 10f;

        private TextMeshProUGUI _instructionText;
        private TextMeshProUGUI _progressText;
        private Button          _okButton;
        private Button          _saveButton;
        private CanvasGroup     _canvasGroup;

        private static readonly Color BackdropColor = new Color(0f, 0f, 0f, 0.75f);

        internal static CalibrationWizard Show(
            Action<float[], float[,]> onComplete, Action onCancel)
        {
            var go     = new GameObject("CalibrationWizard");
            DontDestroyOnLoad(go);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 250;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            go.AddComponent<GraphicRaycaster>();

            var wizard         = go.AddComponent<CalibrationWizard>();
            wizard._onComplete = onComplete;
            wizard._onCancel   = onCancel;
            wizard.Build();
            wizard.EnterState(StateRelease);
            SceneManager.sceneLoaded += wizard.OnSceneLoaded;
            return wizard;
        }

        private void Build()
        {
            var rt    = GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;

            // Backdrop
            var bd  = Child("Backdrop", transform); bd.AddComponent<Image>().color = BackdropColor;
            Stretch(bd.GetComponent<RectTransform>());

            // Panel
            var panel = Child("Panel", transform);
            panel.AddComponent<Image>().color = UIColors.PanelBg;
            var oline = panel.AddComponent<Outline>();
            oline.effectColor = UIColors.BorderStrong; oline.effectDistance = new Vector2(1f, -1f);
            var prt   = panel.GetComponent<RectTransform>();
            prt.anchorMin = prt.anchorMax = prt.pivot = new Vector2(0.5f, 0.5f);
            prt.sizeDelta = new Vector2(480f, 270f); prt.anchoredPosition = Vector2.zero;
            var vg = panel.AddComponent<VerticalLayoutGroup>();
            vg.childForceExpandWidth = true; vg.childForceExpandHeight = false;
            vg.spacing = 12f; vg.padding = new RectOffset(24, 24, 24, 20);

            // Title
            var titleGo = Child("Title", panel.transform);
            titleGo.AddComponent<LayoutElement>().minHeight = 28f;
            var title  = titleGo.AddComponent<TextMeshProUGUI>();
            title.text = "CONTROLLER CALIBRATION"; title.fontSize = 20f;
            title.color = UIColors.Primary; title.fontStyle = FontStyles.Bold;
            title.alignment = TextAlignmentOptions.Center;

            // Instruction
            var instrGo = Child("Instruction", panel.transform);
            instrGo.AddComponent<LayoutElement>().minHeight = 66f;
            _instructionText = instrGo.AddComponent<TextMeshProUGUI>();
            _instructionText.fontSize = 16f; _instructionText.color = Color.white;
            _instructionText.alignment = TextAlignmentOptions.Center;
            _instructionText.textWrappingMode = TMPro.TextWrappingModes.Normal;

            // Progress
            var progGo = Child("Progress", panel.transform);
            progGo.AddComponent<LayoutElement>().minHeight = 24f;
            _progressText = progGo.AddComponent<TextMeshProUGUI>();
            _progressText.fontSize = 13f; _progressText.color = UIColors.SubtleText;
            _progressText.alignment = TextAlignmentOptions.Center;
            _progressText.textWrappingMode = TMPro.TextWrappingModes.Normal;

            // Buttons
            var row = Child("Buttons", panel.transform);
            var hg  = row.AddComponent<HorizontalLayoutGroup>();
            hg.childForceExpandWidth = false; hg.childForceExpandHeight = false;
            hg.spacing = 16f; hg.childAlignment = TextAnchor.MiddleCenter;
            row.AddComponent<LayoutElement>().minHeight = 44f;

            MakeBtn(row.transform, "CANCEL", UIColors.Danger,   Cancel, out _);
            MakeBtn(row.transform, "OK",     UIColors.Primary,  OnOkPressed, out _okButton);
            MakeBtn(row.transform, "SAVE",   UIColors.Valid,    OnSave, out _saveButton);
            _saveButton.gameObject.SetActive(false);

            StartCoroutine(FadeIn());
        }

        private void EnterState(int next)
        {
            _state = next;
            if (_state == StateRelease)
            {
                _instructionText.text = "Release all sticks and triggers.\nMake sure the controller is idle.";
                _progressText.text    = "";
                _okButton.gameObject.SetActive(true);
                _saveButton.gameObject.SetActive(false);
            }
            else if (_state == StateSampleNeutral)
            {
                _instructionText.text = "Do not touch the controller.\nSampling neutral position...";
                _neutralAccum = new float[AxisCount];
                _sampleCount  = 0;
                _okButton.gameObject.SetActive(false);
            }
            else if (_state == StateMoveExtremes)
            {
                _instructionText.text = "Move both sticks to all extremes.\nPull both triggers fully.";
                _mins = new float[AxisCount]; _maxs = new float[AxisCount];
                for (int i = 0; i < AxisCount; i++) { _mins[i] = float.MaxValue; _maxs[i] = float.MinValue; }
                _extremesTimer = ExtremesSeconds;
                _okButton.gameObject.SetActive(false);
            }
            else if (_state == StateResults)
            {
                bool valid = Validate(out string summary);
                _instructionText.text = summary;
                _progressText.text    = valid ? "Calibration looks good!" : "Try again for better results.";
                _progressText.color   = valid ? UIColors.Valid : UIColors.Danger;
                _saveButton.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            var pad = Gamepad.current;
            if (_state == StateSampleNeutral) TickNeutral(pad);
            else if (_state == StateMoveExtremes) TickExtremes(pad);
        }

        private void TickNeutral(Gamepad pad)
        {
            if (pad == null) { _progressText.text = $"Sampling... {_sampleCount}/{NeutralFrames} (no controller)"; return; }
            ReadAxes(pad, _axisBuffer);
            for (int i = 0; i < AxisCount; i++) _neutralAccum[i] += _axisBuffer[i];
            _sampleCount++;
            _progressText.text = $"Sampling... {_sampleCount}/{NeutralFrames}";
            if (_sampleCount >= NeutralFrames)
            {
                for (int i = 0; i < AxisCount; i++) _neutralAccum[i] /= NeutralFrames;
                EnterState(StateMoveExtremes);
            }
        }

        private void TickExtremes(Gamepad pad)
        {
            if (pad != null)
            {
                ReadAxes(pad, _axisBuffer);
                for (int i = 0; i < AxisCount; i++)
                {
                    if (_axisBuffer[i] < _mins[i]) _mins[i] = _axisBuffer[i];
                    if (_axisBuffer[i] > _maxs[i]) _maxs[i] = _axisBuffer[i];
                }
            }
            _extremesTimer -= Time.unscaledDeltaTime;
            _progressText.text = $"Time: {Mathf.CeilToInt(Mathf.Max(0f, _extremesTimer))}s";
            if (_extremesTimer <= 0f) EnterState(StateResults);
        }

        private void OnOkPressed() => EnterState(_state + 1);

        private void OnSave()
        {
            var offsets = new[] { _neutralAccum[0], _neutralAccum[1], _neutralAccum[2], _neutralAccum[3] };
            var ranges  = new float[AxisCount, 2];
            for (int i = 0; i < AxisCount; i++) { ranges[i, 0] = _mins[i]; ranges[i, 1] = _maxs[i]; }
            _onComplete?.Invoke(offsets, ranges);
            Destroy(gameObject);
        }

        private void OnSceneLoaded(Scene s, LoadSceneMode m) => Cancel();

        private void Cancel() { SceneManager.sceneLoaded -= OnSceneLoaded; _onCancel?.Invoke(); Destroy(gameObject); }

        private static void ReadAxes(Gamepad pad, float[] buffer)
        {
            var ls = pad.leftStick.ReadValue();
            var rs = pad.rightStick.ReadValue();
            buffer[0] = ls.x;
            buffer[1] = ls.y;
            buffer[2] = rs.x;
            buffer[3] = rs.y;
            buffer[4] = pad.leftTrigger.ReadValue();
            buffer[5] = pad.rightTrigger.ReadValue();
        }

        private bool Validate(out string summary)
        {
            float lsSpan  = Mathf.Min(Mathf.Abs(_mins[0]), _maxs[0]) + Mathf.Min(Mathf.Abs(_mins[1]), _maxs[1]);
            float rsSpan  = Mathf.Min(Mathf.Abs(_mins[2]), _maxs[2]) + Mathf.Min(Mathf.Abs(_mins[3]), _maxs[3]);
            float ltRange = _maxs[4] - Mathf.Max(0f, _mins[4]);
            float rtRange = _maxs[5] - Mathf.Max(0f, _mins[5]);
            bool lsOk = lsSpan >= 1.0f, rsOk = rsSpan >= 1.0f, ltOk = ltRange >= 0.3f, rtOk = rtRange >= 0.3f;
            summary = $"Left stick: {(lsOk ? "OK" : "LOW")}  Right stick: {(rsOk ? "OK" : "LOW")}\n" +
                      $"LT: {(ltOk ? "OK" : "LOW")}  RT: {(rtOk ? "OK" : "LOW")}";
            return lsOk && rsOk && ltOk && rtOk;
        }

        private static void MakeBtn(Transform parent, string label, Color border,
            UnityEngine.Events.UnityAction onClick, out Button btn)
        {
            var go  = Child(label + "Btn", parent);
            var img = go.AddComponent<Image>(); img.color = UIColors.ButtonFill;
            var ol  = go.AddComponent<Outline>(); ol.effectColor = border; ol.effectDistance = new Vector2(1f, -1f);
            var le  = go.AddComponent<LayoutElement>(); le.minWidth = 140f; le.preferredWidth = 140f; le.minHeight = 40f;
            btn = go.AddComponent<Button>(); btn.targetGraphic = img; btn.onClick.AddListener(onClick);
            var tGo = Child("Text", go.transform);
            var tmp = tGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label; tmp.fontSize = 14f; tmp.color = border;
            tmp.fontStyle = FontStyles.Bold; tmp.alignment = TextAlignmentOptions.Center;
            var trt = tGo.GetComponent<RectTransform>(); Stretch(trt);
        }

        private static GameObject Child(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        private IEnumerator FadeIn()
        {
            const float dur = 0.15f; float e = 0f;
            while (e < dur) { e += Time.unscaledDeltaTime; _canvasGroup.alpha = Mathf.Clamp01(e / dur); yield return null; }
            _canvasGroup.alpha = 1f;
        }
    }
}
