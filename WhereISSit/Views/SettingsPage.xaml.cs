using Microsoft.Maui.Controls;
using WhereISSit.Services;

namespace WhereISSit.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            // Carrega o tema salvo ao abrir a página
            string savedTheme = ThemeService.GetSavedTheme();

            if (savedTheme == "Dark")
            {
                DarkRadio.IsChecked = true;
            }
            else
            {
                LightRadio.IsChecked = true;
            }
        }

        private void OnThemeRadioCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
            {
                return; // evita dupla chamada
            }

            RadioButton changed = (RadioButton)sender;

            if (changed == LightRadio)
            {
                ThemeService.ApplyLight();
            }
            else if (changed == DarkRadio)
            {
                ThemeService.ApplyDark();
            }
        }
    }
}