using UnityEngine;
using R8EOX.Vehicle;

namespace R8EOX.PhysicsTest.Internal
{
    internal class DebugOverlay : MonoBehaviour
    {
        // ---- State ----
        private PlaybackController _playback;
        private PathFollower _follower;
        private VehicleManager _vehicle;
        private string[] _segmentNames;
        private System.Action<int> _onJumpToSegment;

        private int _selectedSegment;
        private bool _showForceVectors;
        private bool _showPath;
        private R8EOX.Vehicle.Internal.RaycastWheel[] _wheels;

        // ---- Style cache ----
        private bool _stylesInit;
        private GUIStyle _panelStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _activeButtonStyle;

        private static readonly string[] k_WheelNames = { "FL", "FR", "RL", "RR" };
        private const float k_BarWidth  = 80f;
        private const float k_BarHeight = 10f;
        private const float k_RestLen   = 0.25f;  // nominal rest spring length for normalisation

        // ---- API ----
        internal void Initialize(
            PlaybackController playback,
            PathFollower follower,
            VehicleManager vehicle,
            string[] segmentNames,
            System.Action<int> onJumpToSegment)
        {
            _playback        = playback;
            _follower        = follower;
            _vehicle         = vehicle;
            _segmentNames    = segmentNames ?? System.Array.Empty<string>();
            _onJumpToSegment = onJumpToSegment;
            _wheels          = vehicle != null
                ? vehicle.GetAllWheels()
                : System.Array.Empty<R8EOX.Vehicle.Internal.RaycastWheel>();
        }

        internal bool ShowPath => _showPath;

        // ---- Unity ----
        private void OnGUI()
        {
            InitStyles();
            DrawPlaybackPanel();
            DrawVehiclePanel();
            DrawWheelPanel();
        }

        // ---- Style init (once) ----
        private void InitStyles()
        {
            if (_stylesInit) return;
            _stylesInit = true;

            _panelStyle = new GUIStyle(GUI.skin.box);
            _panelStyle.normal.background = MakeTex(4, 4, new Color(0f, 0f, 0f, 0.65f));
            _panelStyle.padding = new RectOffset(6, 6, 6, 6);

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
            _titleStyle.normal.textColor = Color.white;

            _activeButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold
            };
            _activeButtonStyle.normal.textColor = Color.yellow;
        }

        // ---- Panel: Playback (top-left) ----
        private void DrawPlaybackPanel()
        {
            GUILayout.BeginArea(new Rect(10f, 10f, 220f, 230f), _panelStyle);
            GUILayout.Label("PLAYBACK", _titleStyle);

            if (_playback != null)
            {
                DrawPlayPauseRow();
                DrawSpeedRow();
            }

            DrawSegmentRow();

            if (_follower != null)
                GUILayout.Label($"Lap: {_follower.CurrentLap}  |  Progress: {_follower.PathProgress:P0}");

            GUILayout.Space(4f);
            DrawToggleRow();
            GUILayout.EndArea();
        }

        private void DrawPlayPauseRow()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_playback.IsPaused ? "Play" : "Pause"))
                _playback.TogglePause();
            if (GUILayout.Button("Step"))
                _playback.Step();
            GUILayout.EndHorizontal();
        }

        private void DrawSpeedRow()
        {
            GUILayout.BeginHorizontal();
            float[] speeds = _playback.AvailableSpeeds;
            foreach (float spd in speeds)
            {
                bool active = Mathf.Approximately(_playback.Speed, spd);
                GUIStyle style = active ? _activeButtonStyle : GUI.skin.button;
                if (GUILayout.Button($"{spd}x", style))
                    _playback.SetSpeed(spd);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSegmentRow()
        {
            if (_segmentNames.Length == 0) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Seg:", GUILayout.Width(30f));
            _selectedSegment = Mathf.Clamp(_selectedSegment, 0, _segmentNames.Length - 1);
            _selectedSegment = GUILayout.SelectionGrid(
                _selectedSegment, _segmentNames, Mathf.Min(_segmentNames.Length, 3));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Jump to Segment"))
                _onJumpToSegment?.Invoke(_selectedSegment);
        }

        private void DrawToggleRow()
        {
            GUILayout.BeginHorizontal();
            bool newForce = GUILayout.Toggle(_showForceVectors, "Force Vectors");
            if (newForce != _showForceVectors)
            {
                _showForceVectors = newForce;
                SetWheelDebug(_showForceVectors);
            }
            _showPath = GUILayout.Toggle(_showPath, "Path Line");
            GUILayout.EndHorizontal();
        }

        // ---- Panel: Vehicle telemetry (top-right) ----
        private void DrawVehiclePanel()
        {
            float x = Screen.width - 260f;
            GUILayout.BeginArea(new Rect(x, 10f, 250f, 230f), _panelStyle);
            GUILayout.Label("VEHICLE", _titleStyle);

            if (_vehicle == null)
            {
                GUILayout.Label("No vehicle assigned.");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label($"Speed:   {_vehicle.GetSpeedKmh():F1} km/h");
            GUILayout.Label($"Forward: {_vehicle.GetForwardSpeedKmh():F1} km/h");

            DrawBar("Throttle", _vehicle.SmoothThrottle, 0f, 1f, Color.green);
            DrawBar("Brake   ", _vehicle.CurrentBrakeForce, 0f, _vehicle.BrakeForce, Color.red);
            DrawSteerBar("Steer   ", _vehicle.CurrentSteering, _vehicle.SteeringMax);

            GUI.color = _vehicle.IsAirborne ? Color.red : Color.green;
            GUILayout.Label(_vehicle.IsAirborne ? "Airborne: YES" : "Airborne: NO");
            GUI.color = Color.white;

            float tumble = _vehicle.TumbleFactor;
            GUI.color = tumble > 0.5f ? Color.red : tumble > 0.1f ? Color.yellow : Color.white;
            GUILayout.Label($"Tumble: {tumble:F2}  |  Tilt: {_vehicle.TiltAngle:F1}\u00b0");
            GUI.color = Color.white;

            float slip = _vehicle.GetSlip();
            GUI.color = slip > 0.3f ? Color.red : slip > 0.1f ? Color.yellow : Color.white;
            GUILayout.Label($"Engine: {_vehicle.CurrentEngineForce:F1} N  |  Slip: {slip:F3}");
            GUI.color = Color.white;

            GUILayout.EndArea();
        }

        // ---- Panel: Wheels (bottom, full width) ----
        private void DrawWheelPanel()
        {
            float panelY = Screen.height - 130f;
            float panelW = Screen.width - 20f;
            float colW   = panelW / 4f;

            GUILayout.BeginArea(new Rect(10f, panelY, panelW, 120f), _panelStyle);
            GUILayout.Label("WHEELS", _titleStyle);

            GUILayout.BeginHorizontal();
            for (int i = 0; i < 4; i++)
            {
                GUILayout.BeginVertical(GUILayout.Width(colW - 4f));
                string colLabel = i < k_WheelNames.Length ? k_WheelNames[i] : $"W{i}";
                if (_wheels != null && i < _wheels.Length && _wheels[i] != null)
                    DrawWheelColumn(colLabel, _wheels[i]);
                else
                    GUILayout.Label($"{colLabel}\n\u2014");
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawWheelColumn(string label, R8EOX.Vehicle.Internal.RaycastWheel w)
        {
            GUILayout.Label(label);

            GUI.color = w.IsOnGround ? Color.green : Color.red;
            GUILayout.Label(w.IsOnGround ? "Gnd: YES" : "Gnd: NO");
            GUI.color = Color.white;

            GUILayout.Label($"Spr: {w.LastSpringLen:F3}m");
            DrawMiniBar(Mathf.Clamp01(w.LastSpringLen / k_RestLen), Color.cyan);

            float gripNorm = Mathf.Clamp01(w.LastGripLoad);
            GUI.color = gripNorm < 0.3f ? Color.red : gripNorm < 0.6f ? Color.yellow : Color.green;
            GUILayout.Label($"Grp:{w.LastGripLoad:F2} Sl:{w.SlipRatio:F3}");
            GUI.color = Color.white;

            GUILayout.Label($"Sus:{w.SuspensionForce:F1}N");
            GUILayout.Label($"RPM:{w.WheelRpm:F0}");
        }

        // ---- Bar helpers ----
        private void DrawBar(string label, float value, float min, float max, Color fill)
        {
            float t = max > min ? Mathf.Clamp01((value - min) / (max - min)) : 0f;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(56f));
            Rect r = GUILayoutUtility.GetRect(k_BarWidth, k_BarHeight);
            GUI.Box(r, GUIContent.none);
            GUI.color = fill;
            GUI.Box(new Rect(r.x, r.y, r.width * t, r.height), GUIContent.none);
            GUI.color = Color.white;
            GUILayout.Label($"{value:F2}", GUILayout.Width(36f));
            GUILayout.EndHorizontal();
        }

        private void DrawSteerBar(string label, float value, float maxAngle)
        {
            float t = maxAngle > 0f ? Mathf.Clamp(value / maxAngle, -1f, 1f) : 0f;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(56f));
            Rect r   = GUILayoutUtility.GetRect(k_BarWidth, k_BarHeight);
            float mid = r.x + r.width * 0.5f;
            float hw  = r.width * 0.5f;
            GUI.Box(r, GUIContent.none);
            GUI.color = Color.cyan;
            if (t >= 0f)
                GUI.Box(new Rect(mid, r.y, hw * t, r.height), GUIContent.none);
            else
                GUI.Box(new Rect(mid + hw * t, r.y, hw * -t, r.height), GUIContent.none);
            GUI.color = Color.white;
            GUILayout.Label($"{value:F2}", GUILayout.Width(36f));
            GUILayout.EndHorizontal();
        }

        private void DrawMiniBar(float t, Color fill)
        {
            Rect r = GUILayoutUtility.GetRect(60f, 6f);
            GUI.Box(r, GUIContent.none);
            GUI.color = fill;
            GUI.Box(new Rect(r.x, r.y, r.width * t, r.height), GUIContent.none);
            GUI.color = Color.white;
        }

        // ---- Misc ----
        private void SetWheelDebug(bool show)
        {
            if (_wheels == null) return;
            foreach (R8EOX.Vehicle.Internal.RaycastWheel w in _wheels)
                if (w != null) w.ShowDebug = show;
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
