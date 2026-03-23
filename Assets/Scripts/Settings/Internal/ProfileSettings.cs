using System;
using System.Collections.Generic;

namespace R8EOX.Settings.Internal
{
    [Serializable]
    internal class ProfileSettings
    {
        private static readonly char[] InvalidChars = { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };

        [UnityEngine.SerializeField] private string activeProfileName = "Default";
        [UnityEngine.SerializeField] private List<string> profileNames = new List<string> { "Default" };

        public string ActiveProfileName { get => activeProfileName; set => activeProfileName = value; }

        public List<string> ProfileNames { get => profileNames; set => profileNames = value; }

        public static ProfileSettings CreateDefault()
        {
            return new ProfileSettings
            {
                activeProfileName = "Default",
                profileNames = new List<string> { "Default" }
            };
        }

        public ProfileSettings Clone()
        {
            return new ProfileSettings
            {
                activeProfileName = activeProfileName,
                profileNames = new List<string>(profileNames)
            };
        }

        public bool IsDefaultProfile(string name)
        {
            return name == "Default";
        }

        public bool IsValidProfileName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (name.Length < 1 || name.Length > 32)
            {
                return false;
            }

            if (name.IndexOfAny(InvalidChars) >= 0)
            {
                return false;
            }

            return true;
        }

        public void AddProfile(string name)
        {
            if (!IsValidProfileName(name))
            {
                return;
            }

            if (!profileNames.Contains(name))
            {
                profileNames.Add(name);
            }
        }

        public void RemoveProfile(string name)
        {
            if (IsDefaultProfile(name))
            {
                return;
            }

            profileNames.Remove(name);
        }
    }
}
