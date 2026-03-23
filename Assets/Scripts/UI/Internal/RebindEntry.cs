using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R8EOX.UI.Internal
{
    /// <summary>
    /// A single row in the key/button bindings list.
    /// Skeleton for future InputActionRebindingExtensions integration.
    /// ControlsTabContent will spawn and wire these when rebinding is implemented.
    /// </summary>
    internal class RebindEntry : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI actionLabel;
        [SerializeField] private TextMeshProUGUI bindingLabel;
        [SerializeField] private Button rebindButton;

        [Header("State")]
        [SerializeField] private string actionName;

        private bool _isRebinding;

        private static readonly Color LabelColor   = Color.white;
        private static readonly Color BindingColor = new Color(0f, 0.784f, 1f);
        private static readonly Color WaitingColor = new Color(0.91f, 0.722f, 0.286f);

        // ── Initialization ────────────────────────────────────────────────

        /// <summary>
        /// Sets up the row with a human-readable action name and its current
        /// binding display text (e.g. "RT", "W", "Space").
        /// </summary>
        internal void Initialize(string name, string currentBinding)
        {
            actionName = name;

            if (actionLabel != null)
            {
                actionLabel.text  = name;
                actionLabel.color = LabelColor;
            }

            SetBinding(currentBinding);

            if (rebindButton != null)
                rebindButton.onClick.AddListener(StartRebind);
        }

        // ── Rebind API ────────────────────────────────────────────────────

        /// <summary>
        /// Begins a rebind operation. Sets the binding label to a waiting state.
        /// Full InputActionRebindingExtensions integration is deferred.
        /// </summary>
        internal void StartRebind()
        {
            if (_isRebinding) return;
            _isRebinding = true;

            if (bindingLabel != null)
            {
                bindingLabel.text  = "Press key...";
                bindingLabel.color = WaitingColor;
            }

            if (rebindButton != null)
                rebindButton.interactable = false;

            // TODO: start InputActionRebindingExtensions.PerformInteractiveRebinding()
            // When complete, call SetBinding(newDisplayText) and reset _isRebinding.
        }

        /// <summary>
        /// Updates the binding display label with the provided text.
        /// Call this after a successful rebind or to reflect a loaded override.
        /// </summary>
        internal void SetBinding(string displayText)
        {
            _isRebinding = false;

            if (bindingLabel != null)
            {
                bindingLabel.text  = displayText;
                bindingLabel.color = BindingColor;
            }

            if (rebindButton != null)
                rebindButton.interactable = true;
        }

        // ── Properties ────────────────────────────────────────────────────

        /// <summary>The Input System action name this row represents.</summary>
        internal string ActionName => actionName;
    }
}
