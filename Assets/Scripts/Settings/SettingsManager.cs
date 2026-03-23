using System;
using UnityEngine;
using AudioSettingsData = R8EOX.Settings.Internal.AudioSettings;
using R8EOX.Settings.Internal;

namespace R8EOX.Settings
{
    /// <summary>Top-level settings coordinator. Lives on [AppRoot] (DontDestroyOnLoad).</summary>
    public class SettingsManager : MonoBehaviour
    {
        private SettingsData currentData;
        private string activeProfileName = "Default";
        private bool initialized;

        /// <summary>Fired after any settings change is saved and applied.</summary>
        public event Action OnSettingsChanged;

        private void Awake() => Initialize();

        private void Initialize()
        {
            if (initialized) return;
            Load();
            initialized = true;
        }

        // ── Load / Save / Apply ────────────────────────────────────────────

        /// <summary>Reloads all settings from disk for the active profile.</summary>
        public void Load()
        {
            try
            {
                currentData = SettingsIO.LoadAll(activeProfileName);
                activeProfileName = currentData.profiles.ActiveProfileName;
                Debug.Log($"[SettingsManager] Loaded profile '{activeProfileName}'.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SettingsManager] Load failed, using defaults: {ex.Message}");
                currentData = SettingsData.CreateDefault();
            }
        }

        /// <summary>Saves current settings to disk (global + active profile).</summary>
        public void Save()
        {
            try
            {
                SettingsIO.SaveGlobalSettings(currentData);
                SettingsIO.SaveProfileSettings(currentData, activeProfileName);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SettingsManager] Save failed: {ex.Message}");
            }
        }

        /// <summary>Applies all settings to the engine.</summary>
        public void ApplyAll() { ApplyVideo(); ApplyAudio(); }

        // ── Getters ────────────────────────────────────────────────────────

        internal VideoSettings GetVideoSettings() => currentData.video.Clone();
        internal AudioSettingsData GetAudioSettings() => currentData.audio.Clone();
        internal ControlsSettings GetControlsSettings() => currentData.controls.Clone();
        internal CalibrationSettings GetCalibrationSettings() => currentData.calibration.Clone();
        internal GameplaySettings GetGameplaySettings() => currentData.gameplay.Clone();

        // ── Setters ────────────────────────────────────────────────────────

        internal void SetVideoSettings(Action<VideoSettings> modifier)
        {
            if (modifier == null) return;
            modifier(currentData.video);
            Save(); ApplyVideo(); OnSettingsChanged?.Invoke();
        }

        internal void SetAudioSettings(Action<AudioSettingsData> modifier)
        {
            if (modifier == null) return;
            modifier(currentData.audio);
            Save(); ApplyAudio(); OnSettingsChanged?.Invoke();
        }

        internal void SetControlsSettings(Action<ControlsSettings> modifier)
        {
            if (modifier == null) return;
            modifier(currentData.controls);
            Save(); OnSettingsChanged?.Invoke();
        }

        internal void SetCalibrationSettings(Action<CalibrationSettings> modifier)
        {
            if (modifier == null) return;
            modifier(currentData.calibration);
            Save(); OnSettingsChanged?.Invoke();
        }

        internal void SetGameplaySettings(Action<GameplaySettings> modifier)
        {
            if (modifier == null) return;
            modifier(currentData.gameplay);
            Save(); OnSettingsChanged?.Invoke();
        }

        // ── Profile Management ─────────────────────────────────────────────

        /// <summary>Name of the currently active profile.</summary>
        public string ActiveProfileName => activeProfileName;

        public string[] GetProfileNames() => currentData.profiles.ProfileNames.ToArray();

        /// <summary>Saves current profile, then loads and applies the named profile.</summary>
        public void SwitchProfile(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            Save();
            activeProfileName = name;
            currentData.profiles.ActiveProfileName = name;
            Load(); ApplyAll(); OnSettingsChanged?.Invoke();
        }

        /// <summary>Creates a new profile with defaults and switches to it.</summary>
        public void CreateProfile(string name)
        {
            if (!currentData.profiles.IsValidProfileName(name))
            {
                Debug.LogWarning($"[SettingsManager] Invalid profile name: '{name}'.");
                return;
            }
            if (currentData.profiles.ProfileNames.Contains(name))
            {
                Debug.LogWarning($"[SettingsManager] Profile '{name}' already exists.");
                return;
            }

            currentData.profiles.AddProfile(name);
            Save();
            activeProfileName = name;
            currentData.profiles.ActiveProfileName = name;
            currentData.audio = new AudioSettingsData();
            currentData.controls = new ControlsSettings();
            currentData.calibration = new CalibrationSettings();
            currentData.gameplay = new GameplaySettings();
            Save(); ApplyAll(); OnSettingsChanged?.Invoke();
        }

        /// <summary>Deletes a profile. Cannot delete "Default". Falls back to Default.</summary>
        public void DeleteProfile(string name)
        {
            if (currentData.profiles.IsDefaultProfile(name))
            {
                Debug.LogWarning("[SettingsManager] Cannot delete the Default profile.");
                return;
            }
            currentData.profiles.RemoveProfile(name);
            try { SettingsIO.DeleteProfile(name); }
            catch (Exception ex)
            {
                Debug.LogError($"[SettingsManager] Failed to delete profile '{name}': {ex.Message}");
            }
            if (activeProfileName == name) SwitchProfile("Default");
            else { Save(); OnSettingsChanged?.Invoke(); }
        }

        /// <summary>Renames an existing profile. Cannot rename "Default".</summary>
        public void RenameProfile(string oldName, string newName)
        {
            if (currentData.profiles.IsDefaultProfile(oldName))
            {
                Debug.LogWarning("[SettingsManager] Cannot rename the Default profile.");
                return;
            }
            if (!currentData.profiles.IsValidProfileName(newName))
            {
                Debug.LogWarning($"[SettingsManager] Invalid new profile name: '{newName}'.");
                return;
            }
            if (!currentData.profiles.ProfileNames.Contains(oldName))
            {
                Debug.LogWarning($"[SettingsManager] Profile '{oldName}' not found.");
                return;
            }

            bool wasActive = activeProfileName == oldName;
            currentData.profiles.RemoveProfile(oldName);
            currentData.profiles.AddProfile(newName);
            if (wasActive)
            {
                activeProfileName = newName;
                currentData.profiles.ActiveProfileName = newName;
            }
            Save(); OnSettingsChanged?.Invoke();
        }

        // ── Quality Tier ───────────────────────────────────────────────────

        /// <summary>Applies a preset video config for the given tier.</summary>
        internal void SetQualityTier(QualityTier tier)
        {
            SetVideoSettings(v =>
            {
                v.QualityTier = tier;
                switch (tier)
                {
                    case QualityTier.Ultra:
                        v.RenderScale = 1.0f; v.UpscalingMode = UpscalingMode.None; break;
                    case QualityTier.High:
                        v.RenderScale = 1.0f; v.UpscalingMode = UpscalingMode.None; break;
                    case QualityTier.Balanced:
                        v.RenderScale = 0.75f; v.UpscalingMode = UpscalingMode.FSR2; break;
                    case QualityTier.Performance:
                        v.RenderScale = 0.5f; v.UpscalingMode = UpscalingMode.FSR2; break;
                }
            });
        }

        // ── Private Apply Helpers ──────────────────────────────────────────

        private void ApplyVideo()
        {
            VideoApplier.Apply(currentData.video, null);
        }

        private void ApplyAudio()
        {
            AudioApplier.Apply(currentData.audio);
        }
    }
}
