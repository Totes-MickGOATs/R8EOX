using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.Session.Internal
{
    internal class SetupErrorOverlay : MonoBehaviour
    {
        // ---- Internal state ----
        private readonly List<(string system, string message, string fix)> _errors = new();
        private readonly List<(string system, string message)> _warnings = new();

        private float _warningAutoHideTime = 30f;
        private float _createdAt = -1f;

        // ---- Style cache ----
        private bool _stylesInit;
        private GUIStyle _panelStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _errorTitleStyle;
        private GUIStyle _warningTitleStyle;
        private GUIStyle _errorStyle;
        private GUIStyle _warningStyle;

        private const float PanelWidth = 500f;
        private const float PanelPadding = 10f;

        // ---- Public API ----

        internal bool HasErrors => _errors.Count > 0;

        internal bool HasWarnings => _warnings.Count > 0;

        internal void AddError(string system, string message, string fix)
        {
            StampCreatedTime();
            _errors.Add((system, message, fix));
            Debug.LogError($"[{system}] {message} — Fix: {fix}");
        }

        internal void AddWarning(string system, string message)
        {
            StampCreatedTime();
            _warnings.Add((system, message));
            Debug.LogWarning($"[{system}] {message}");
        }

        // ---- Unity callbacks ----

        private void Update()
        {
            if (!HasErrors && !HasWarnings)
            {
                Destroy(this);
                return;
            }

            if (!HasErrors && HasWarnings)
            {
                if (Time.realtimeSinceStartup - _createdAt > _warningAutoHideTime)
                    Destroy(this);
            }
        }

        private void OnGUI()
        {
            if (!HasErrors && !HasWarnings) return;

            InitStyles();

            float panelX = (Screen.width - PanelWidth) * 0.5f;
            float contentHeight = EstimateContentHeight();

            GUILayout.BeginArea(
                new Rect(panelX, PanelPadding, PanelWidth, contentHeight),
                _panelStyle);

            if (HasErrors)
                DrawErrors();

            if (HasWarnings)
                DrawWarnings();

            GUILayout.EndArea();
        }

        // ---- Rendering ----

        private void DrawErrors()
        {
            GUILayout.Label("SETUP ERRORS", _errorTitleStyle);
            GUILayout.Space(4f);

            foreach (var (system, message, fix) in _errors)
            {
                GUILayout.Label($"[{system}] {message}", _errorStyle);
                GUILayout.Label($"  Fix: {fix}", _errorStyle);
                GUILayout.Space(2f);
            }

            GUILayout.Space(6f);
        }

        private void DrawWarnings()
        {
            GUILayout.Label("WARNINGS", _warningTitleStyle);
            GUILayout.Space(4f);

            foreach (var (system, message) in _warnings)
            {
                GUILayout.Label($"[{system}] {message}", _warningStyle);
                GUILayout.Space(2f);
            }
        }

        // ---- Style init (once) ----

        private void InitStyles()
        {
            if (_stylesInit) return;
            _stylesInit = true;

            _panelStyle = new GUIStyle(GUI.skin.box);
            _panelStyle.normal.background = MakeTex(4, 4, new Color(0f, 0f, 0f, 0.65f));
            _panelStyle.padding = new RectOffset(6, 6, 6, 6);

            _titleStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            _titleStyle.normal.textColor = Color.white;

            _errorTitleStyle = new GUIStyle(_titleStyle) { fontSize = 16 };
            _errorTitleStyle.normal.textColor = new Color(1f, 0.3f, 0.3f);

            _warningTitleStyle = new GUIStyle(_titleStyle) { fontSize = 14 };
            _warningTitleStyle.normal.textColor = Color.yellow;

            _errorStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
            _errorStyle.normal.textColor = new Color(1f, 0.4f, 0.4f);

            _warningStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
            _warningStyle.normal.textColor = Color.yellow;
        }

        // ---- Helpers ----

        private void StampCreatedTime()
        {
            if (_createdAt < 0f)
                _createdAt = Time.realtimeSinceStartup;
        }

        private float EstimateContentHeight()
        {
            float height = 12f; // panel padding
            if (HasErrors)
            {
                height += 24f; // title
                height += _errors.Count * 42f; // two lines + spacing per error
                height += 6f; // bottom spacing
            }

            if (HasWarnings)
            {
                height += 22f; // title
                height += _warnings.Count * 22f; // one line + spacing per warning
            }

            return Mathf.Min(height, Screen.height - 20f);
        }

        private static Texture2D MakeTex(int w, int h, Color col)
        {
            var pix = new Color[w * h];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            var tex = new Texture2D(w, h);
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
    }
}
