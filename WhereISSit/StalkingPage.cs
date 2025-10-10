using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhereISSit
{
    public partial class StalkingPage : ContentPage
    {
        public StalkingPage()
        {
            InitializeComponent();
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await CheckAndRequestLocationAsync(forceRequest: true);
            LocationStatusLabel.GestureRecognizers.Add(tapGesture);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckAndRequestLocationAsync();
        }

        // --- LOCALIZAÇÃO ---
        private async Task CheckAndRequestLocationAsync(bool forceRequest = false)
        {
            try
            {
                bool isFirstLaunch = Preferences.Get("IsFirstLaunch", true);
                if (isFirstLaunch || forceRequest)
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    Preferences.Set("IsFirstLaunch", false);
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                // Registra o evento do Expander fora do if
                NextSightingsExpander.ExpandedChanged += async (s, e) =>
                {
                    if (e.IsExpanded && location != null)
                        await LoadNextSightings(location.Latitude, location.Longitude);
                };

                if (location != null)
                {
                    // Tenta obter o nome da cidade e estado via Reverse Geocoding
                    var placemarks = await Geocoding.GetPlacemarksAsync(location);
                    var placemark = placemarks?.FirstOrDefault();

                    if (placemark != null)
                    {
                        // Exibe nome do local (ex: "San Jose, California")
                        UpdateLocationLabel($"{placemark.Locality}, {placemark.AdminArea}");
                    }
                    else
                    {
                        // Caso falhe, exibe coordenadas
                        UpdateLocationLabel($"Lat: {location.Latitude:F2}, Lon: {location.Longitude:F2}");
                    }
                }
                else
                {
                    UpdateLocationLabel("Location unavailable");
                }
            }
            catch
            {
                UpdateLocationLabel("Error retrieving location");
            }
        }

        private void UpdateLocationLabel(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (LocationStatusLabel != null)
                    LocationStatusLabel.Text = message;
            });
        }

        // --- POPULAR NEXT SIGHTINGS ---
        private async Task LoadNextSightings(double latitude, double longitude)
        {
            var passes = await IssPass.GetNextSightingsAsync(latitude, longitude);

            if (passes != null && passes.Count > 0)
            {
                var firstPass = passes[0];
                NextPassLabel.Text = firstPass.NextPassTime;
                DurationLabel.Text = firstPass.DurationText;
                ElevationLabel.Text = firstPass.MaxElevationText;
            }
            else
            {   
                NextPassLabel.Text = "No passes available in the next 48hrs at your location.";
                DurationLabel.Text = "";
                ElevationLabel.Text = "";
            }
        }
    }
}