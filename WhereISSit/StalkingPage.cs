using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhereISSit
{
    public partial class StalkingPage : ContentPage
    {
        private double lastLatitude;
        private double lastLongitude;

        public StalkingPage()
        {
            InitializeComponent();

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnLocationStatusLabelTapped;
            LocationStatusLabel.GestureRecognizers.Add(tapGesture);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await CheckAndRequestLocationAsync();

            _ = StartInfoSectionLoop(); // no "_ =" e sem await
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ==================================================
        // MAIN METHOD (clean)
        // ==================================================
        private async Task CheckAndRequestLocationAsync(bool forceRequest = false)
        {
            try
            {
                await CheckPermissionsAsync(forceRequest);

                var userLocation = await GetUserLocationAsync();

                if (userLocation == null)
                {
                    UpdateLocationLabel("Location unavailable");
                    return;
                }

                lastLatitude = userLocation.Latitude;
                lastLongitude = userLocation.Longitude;

                await ShowUserCityAsync(userLocation);

                await UpdateIssDataAsync(userLocation);
            }
            catch
            {
                UpdateLocationLabel("Error retrieving location");
            }
        }

        // ==================================================
        // PERMISSIONS
        // ==================================================
        private async Task CheckPermissionsAsync(bool forceRequest)
        {
            bool firstLaunch = Preferences.Get("IsFirstLaunch", true);

            if (!firstLaunch && !forceRequest)
                return;

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            Preferences.Set("IsFirstLaunch", false);
        }

        // ==================================================
        // LOCATION
        // ==================================================
        private async Task<Location?> GetUserLocationAsync()
        {
            var request = new GeolocationRequest(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(10)
            );

            return await Geolocation.GetLocationAsync(request);
        }

        // ==================================================
        // CITY / LABEL UPDATE
        // ==================================================
        private async Task ShowUserCityAsync(Location userLocation)
        {
            var places = await Geocoding.GetPlacemarksAsync(userLocation);
            var place = places.FirstOrDefault();

            if (place != null)
                UpdateLocationLabel($"{place.Locality}, {place.AdminArea}");
            else
                UpdateLocationLabel($"Lat: {userLocation.Latitude:F2}, Lon: {userLocation.Longitude:F2}");
        }

        // ==================================================
        // LOAD ISS (passes + position)
        // ==================================================
        private async Task UpdateIssDataAsync(Location userLocation)
        {
            await LoadNextSightings(userLocation.Latitude, userLocation.Longitude);
            await LoadCurrentPosition(userLocation.Latitude, userLocation.Longitude);
        }

        // ==================================================
        // TAP REFRESH
        // ==================================================
        private async void OnLocationStatusLabelTapped(object? sender, TappedEventArgs e)
        {
            await CheckAndRequestLocationAsync(true);
        }

        // ==================================================
        // SIMPLE LABEL UPDATE (iniciante)
        // ==================================================
        private void UpdateLocationLabel(string message)
        {
            if (LocationStatusLabel != null)
            {
                LocationStatusLabel.Text = message;
            }
        }

        // ==================================================
        // NEXT SIGHTINGS
        // ==================================================
        private async Task LoadNextSightings(double latitude, double longitude)
        {
            var service = new Services.IssService();
            var issPasses = await service.GetNextSightingsAsync(latitude, longitude);

            if (issPasses == null || issPasses.Count == 0)
            {
                NextPassLabel.Text = "No passes available in the next 48hrs.";
                DurationLabel.Text = "";
                ElevationLabel.Text = "";
                return;
            }

            var firstPass = issPasses[0];

            NextPassLabel.Text = firstPass.NextPassTime;
            DurationLabel.Text = firstPass.DurationText;
            ElevationLabel.Text = firstPass.MaxElevationText;
        }

        // ==================================================
        // CURRENT POSITION
        // ==================================================
        private async Task LoadCurrentPosition(double latitude, double longitude)
        {
            var service = new Services.IssService();
            var positions = await service.GetCurrentPositionAsync(latitude, longitude);

            if (positions == null || positions.Count == 0)
            {
                LatitudeLabel.Text = "--";
                LongitudeLabel.Text = "--";
                AltitudeLabel.Text = "--";
                ElevationInfoLabel.Text = "--";
                AzimuthLabel.Text = "--";
                return;
            }

            var pos = positions[0];

            LatitudeLabel.Text = pos.Latitude.ToString("F2");
            LongitudeLabel.Text = pos.Longitude.ToString("F2");
            AltitudeLabel.Text = pos.AltitudeText;
            AzimuthLabel.Text = pos.AzimuthText;
            ElevationInfoLabel.Text = pos.ElevationText;
        }

        // ==================================================
        // UPDATE LOOP (very simple)
        // ==================================================
        private async Task StartInfoSectionLoop()
        {
            for (int counter = 0; counter < 900; counter++)
            {
                if (lastLatitude != 0 && lastLongitude != 0)
                {
                    await LoadCurrentPosition(lastLatitude, lastLongitude);
                }

                await Task.Delay(4000);
            }
        }
    }
}