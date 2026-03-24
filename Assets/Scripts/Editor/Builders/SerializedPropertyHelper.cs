#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace R8EOX.Editor
{
    /// <summary>
    /// Null-guarded helpers for setting properties on a <see cref="SerializedObject"/>.
    /// Each method logs a descriptive error if the property is not found, preventing
    /// silent failures when field names drift between builders and MonoBehaviour fields.
    /// </summary>
    internal static class SerializedPropertyHelper
    {
        /// <summary>Sets an object-reference property. Logs an error if not found.</summary>
        internal static void SetRef(SerializedObject so, string propertyName, Object value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                Debug.LogError(
                    $"[SerializedPropertyHelper] Property '{propertyName}' not found on {so.targetObject}");
                return;
            }
            prop.objectReferenceValue = value;
        }

        /// <summary>Sets a float property. Logs an error if not found.</summary>
        internal static void SetFloat(SerializedObject so, string propertyName, float value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                Debug.LogError(
                    $"[SerializedPropertyHelper] Property '{propertyName}' not found on {so.targetObject}");
                return;
            }
            prop.floatValue = value;
        }

        /// <summary>Sets an enumeration property by index value. Logs an error if not found.</summary>
        internal static void SetEnum(SerializedObject so, string propertyName, int value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                Debug.LogError(
                    $"[SerializedPropertyHelper] Property '{propertyName}' not found on {so.targetObject}");
                return;
            }
            prop.enumValueIndex = value;
        }

        /// <summary>Sets a Vector3 property. Logs an error if not found.</summary>
        internal static void SetVec3(SerializedObject so, string propertyName, Vector3 value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                Debug.LogError(
                    $"[SerializedPropertyHelper] Property '{propertyName}' not found on {so.targetObject}");
                return;
            }
            prop.vector3Value = value;
        }

        /// <summary>Sets a bool property. Logs an error if not found.</summary>
        internal static void SetBool(SerializedObject so, string propertyName, bool value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                Debug.LogError(
                    $"[SerializedPropertyHelper] Property '{propertyName}' not found on {so.targetObject}");
                return;
            }
            prop.boolValue = value;
        }

        /// <summary>
        /// One-shot helper: creates a <see cref="SerializedObject"/> for <paramref name="target"/>,
        /// sets a single object-reference property, and applies. Ideal for callers that only
        /// need to wire one field without managing a SerializedObject themselves.
        /// </summary>
        internal static void WireRef(Object target, string propertyName, Object value)
        {
            var so = new SerializedObject(target);
            SetRef(so, propertyName, value);
            so.ApplyModifiedProperties();
        }
    }
}
#endif
