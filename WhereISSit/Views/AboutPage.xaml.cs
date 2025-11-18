using Microsoft.Maui.Controls;
using System;

namespace WhereISSit.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void OnNasaPageClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.nasa.gov/mission/international-space-station/");
        }

        private async void OnN2yoPageClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.n2yo.com/");
        }

        private async void OnRepoClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://github.com/castilhotafe/iss-tracker");
        }

        private async void OnTafeClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.northmetrotafe.wa.edu.au/");
        }
    }
}