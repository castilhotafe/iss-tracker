using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using WhereISSit.Resources.Styles;

namespace WhereISSit.Services
{
    public static class ThemeService
    {
        private const string ThemePreferenceKey = "selected_app_theme";
        private const string LightThemeValue = "Light";
        private const string DarkThemeValue = "Dark";

        public static void ApplyLight()
        {
            Application currentApplication = Application.Current!;

            if (currentApplication == null)
            {
                return;
            }

            ResourceDictionary resources = currentApplication.Resources;

            RemoveExistingThemeDictionaries(resources);

            LightThemeDictionary lightThemeDictionary = new LightThemeDictionary();
            resources.MergedDictionaries.Add(lightThemeDictionary);

            Preferences.Set(ThemePreferenceKey, LightThemeValue);
        }

        public static void ApplyDark()
        {
            Application currentApplication = Application.Current!;

            if (currentApplication == null)
            {
                return;
            }

            ResourceDictionary resources = currentApplication.Resources;

            
            RemoveExistingThemeDictionaries(resources);

            
            DarkThemeDictionary dark = new DarkThemeDictionary();
            resources.MergedDictionaries.Add(dark);

            
            Preferences.Set(ThemePreferenceKey, DarkThemeValue);
        }

        public static string GetSavedTheme()
        {
            string savedTheme = Preferences.Get(ThemePreferenceKey, LightThemeValue);

            if (savedTheme == DarkThemeValue)
            {
                return DarkThemeValue;
            }

            return LightThemeValue;
        }

        private static void RemoveExistingThemeDictionaries(ResourceDictionary resources)
        {
            // Remove Light theme if present
            foreach (var dictionary in resources.MergedDictionaries)
            {
                if (dictionary is LightThemeDictionary)
                {
                    resources.MergedDictionaries.Remove(dictionary);
                    break;
                }
            }

            // Remove Dark theme if present
            foreach (var dictionary in resources.MergedDictionaries)
            {
                if (dictionary is DarkThemeDictionary)
                {
                    resources.MergedDictionaries.Remove(dictionary);
                    break;
                }
            }
        }
    }
}