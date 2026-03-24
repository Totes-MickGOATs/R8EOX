using System;
using System.IO;
using UnityEngine;

namespace R8EOX.Settings.Internal
{
    internal static class SettingsIO
    {
        private static string GlobalSettingsPath =>
            Path.Combine(Application.persistentDataPath, "settings_global.json");

        private static string ProfileSettingsPath(string profileName) =>
            Path.Combine(Application.persistentDataPath, "profiles", profileName, "settings.json");

        internal static void SaveGlobalSettings(SettingsData data)
        {
            var file = new GlobalSettingsFile
            {
                Video = data.video,
                Profiles = data.profiles
            };
            string json = JsonUtility.ToJson(file, prettyPrint: true);
            EnsureDirectory(GlobalSettingsPath);
            File.WriteAllText(GlobalSettingsPath, json);
        }

        internal static void SaveProfileSettings(SettingsData data, string profileName)
        {
            var file = new ProfileSettingsFile
            {
                Audio = data.audio,
                Controls = data.controls,
                Calibration = data.calibration,
                Gameplay = data.gameplay
            };
            string json = JsonUtility.ToJson(file, prettyPrint: true);
            string path = ProfileSettingsPath(profileName);
            EnsureDirectory(path);
            File.WriteAllText(path, json);
        }

        internal static SettingsData LoadAll(string profileName)
        {
            var result = SettingsData.CreateDefault();

            string globalPath = GlobalSettingsPath;
            if (File.Exists(globalPath))
            {
                try
                {
                    string json = File.ReadAllText(globalPath);
                    var file = JsonUtility.FromJson<GlobalSettingsFile>(json);
                    result.video = file.Video;
                    result.profiles = file.Profiles;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SettingsIO] Failed to load global settings: {ex.Message}");
                }
            }

            string profilePath = ProfileSettingsPath(profileName);
            if (File.Exists(profilePath))
            {
                try
                {
                    string json = File.ReadAllText(profilePath);
                    var file = JsonUtility.FromJson<ProfileSettingsFile>(json);
                    result.audio = file.Audio;
                    result.controls = file.Controls;
                    result.calibration = file.Calibration;
                    result.gameplay = file.Gameplay;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SettingsIO] Failed to load profile settings for '{profileName}': {ex.Message}");
                }
            }

            return result;
        }

        internal static void EnsureDirectory(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        internal static void DeleteProfile(string profileName)
        {
            string profileDir = Path.Combine(
                Application.persistentDataPath, "profiles", profileName);
            if (Directory.Exists(profileDir))
            {
                Directory.Delete(profileDir, recursive: true);
            }
        }
    }
}
