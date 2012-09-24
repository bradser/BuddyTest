using System;
using System.IO.IsolatedStorage;

namespace BuddyTest
{
    public class Settings
    {
        private const string NameKeyName = "Name";

        private const string PasswordKeyName = "Password";
        
        private Settings()
        {
        }

        public static Settings GetInstance()
        {
            return new Settings();
        }

        public string Name
        {
            get
            {
                return this.GetValueOrDefault<string>(NameKeyName, null);
            }

            set
            {
                if (this.AddOrUpdateValue(NameKeyName, value))
                {
                    this.Save();
                }
            }
        }

        public string Password
        {
            get
            {
                return this.GetValueOrDefault<string>(PasswordKeyName, null);
            }

            set
            {
                if (this.AddOrUpdateValue(PasswordKeyName, value))
                {
                    this.Save();
                }
            }
        }

        private T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

#if !DESIGNTIME
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                value = (T)IsolatedStorageSettings.ApplicationSettings[key];
            }
            else
#endif
            {
                value = defaultValue;
            }

            return value;
        }

        private bool AddOrUpdateValue<T>(string key, T value) where T : IEquatable<T>
        {
            bool valueChanged = false;

#if !DESIGNTIME
            // If the key exists
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                // If the value has changed
                if (!((T)IsolatedStorageSettings.ApplicationSettings[key]).Equals(value))
                {
                    // Store the new value
                    IsolatedStorageSettings.ApplicationSettings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                // Otherwise create the key.
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
                valueChanged = true;
            }
#endif
            return valueChanged;
        }

        private void Save()
        {
#if !DESIGNTIME
            IsolatedStorageSettings.ApplicationSettings.Save();
#endif
        }
    }
}
