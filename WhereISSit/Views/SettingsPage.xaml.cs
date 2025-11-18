using System;
using Microsoft.Maui.Controls;
using WhereISSit.Services;

namespace WhereISSit.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            ThemePicker.Items.Add("Light");
            ThemePicker.Items.Add("Dark");

            string savedThemeChoice = ThemeService.GetSavedTheme();

            if (savedThemeChoice == "Dark")
            {
                ThemePicker.SelectedIndex = 1;
            }
            else
            {
                ThemePicker.SelectedIndex = 0;
            }
        }

        private void OnThemePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = ThemePicker.SelectedIndex;

            if (selectedIndex == 0)
            {
                ThemeService.ApplyLight();
            }
            else if (selectedIndex == 1)
            {
                ThemeService.ApplyDark();
            }
        }
    }
}