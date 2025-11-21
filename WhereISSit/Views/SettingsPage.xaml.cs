using Microsoft.Maui.Controls;
using WhereISSit.Services;

namespace WhereISSit.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            // ========== Carregar TEMA ==========
            string savedTheme = ThemeService.GetSavedTheme();
            if (savedTheme == "Dark")
                DarkRadio.IsChecked = true;
            else
                LightRadio.IsChecked = true;

            // ========== Carregar FORMATO DE HORAS ==========
            string format = TimeFormat.Get();
            if (format == "12h")
                TimeFormat12.IsChecked = true;
            else
                TimeFormat24.IsChecked = true;
        }

        // ========== THEME ==========
        private void OnThemeRadioCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
                return;

            if (sender == LightRadio)
                ThemeService.ApplyLight();
            else if (sender == DarkRadio)
                ThemeService.ApplyDark();
        }

        // ========== TIME FORMAT ==========
        private void OnTimeFormatCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
                return;

            if (sender == TimeFormat24)
                TimeFormat.Set("24h");
            else if (sender == TimeFormat12)
                TimeFormat.Set("12h");
        }
    }
}