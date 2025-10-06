using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

namespace WhereISSit
{
    public partial class StalkingPage : ContentPage
    {
        public StalkingPage()
        {
            InitializeComponent();

            // Tap on label to retry location permission or update
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await CheckAndRequestLocationAsync(forceRequest: true);
            LocationStatusLabel.GestureRecognizers.Add(tapGesture);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Run the location check when the page appears
            await CheckAndRequestLocationAsync();
        }

        private async Task CheckAndRequestLocationAsync(bool forceRequest = false)
        {
            try
            {
                bool isFirstLaunch = Preferences.Get("IsFirstLaunch", true);

                // Only check or request permission on first launch or when forced by user tap
                if (isFirstLaunch || forceRequest)
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    }

                    Preferences.Set("IsFirstLaunch", false);
                }

                // Get real-time location (not cached)
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location);
                    var placemark = placemarks?.FirstOrDefault();

                    if (placemark != null && !string.IsNullOrWhiteSpace(placemark.Locality))
                    {
                        string city = placemark.Locality;
                        string country = placemark.CountryName ?? "";
                        UpdateLocationLabel($"{city}, {country}");
                    }
                    else
                    {
                        UpdateLocationLabel($"Lat: {location.Latitude:F4}, Lon: {location.Longitude:F4}");
                    }
                }
                else
                {
                    UpdateLocationLabel("Location unavailable");
                }
            }
            catch (FeatureNotSupportedException)
            {
                UpdateLocationLabel("Location not supported on this device");
            }
            catch (PermissionException)
            {
                UpdateLocationLabel("Update location");
            }
            catch (Exception)
            {
                UpdateLocationLabel("Error retrieving location");
            }
        }

        private void UpdateLocationLabel(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (LocationStatusLabel != null)
                {
                    LocationStatusLabel.Text = message;
                }
            });
        }
    }
}