using Microsoft.Maui.Controls;

namespace WhereISSit.Services
{
    public static class Theme
    {
        private const string ThemeKey = "AppTheme";

        public static void SetTheme(AppTheme theme)
        {
            Preferences.Set(ThemeKey, theme.ToString());
            Application.Current!.UserAppTheme = theme;
        }

        public static AppTheme GetTheme()
        {
            string saved = Preferences.Get(ThemeKey, AppTheme.Unspecified.ToString());

            if (Enum.TryParse(saved, out AppTheme result))
                return result;

            return AppTheme.Unspecified;
        }
    }
}