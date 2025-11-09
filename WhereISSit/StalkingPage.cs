using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace WhereISSit
{
    // this page shows user location and ISS data
    public partial class StalkingPage : ContentPage
    {
        // store last known latitude and longitude
        private double lastLatitude;
        private double lastLongitude;

        // constructor runs once when page is created
        public StalkingPage()
        {
            InitializeComponent();
        
            // create tap gesture for manual refresh
            // when the label is tapped, it will call OnLocationStatusLabelTapped
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnLocationStatusLabelTapped;
            LocationStatusLabel.GestureRecognizers.Add(tapGesture);
        }

        // this runs automatically when page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // check location when page loads
            await CheckAndRequestLocationAsync();

            // start background update loop for ISS info section
            _ = StartInfoSectionLoop();
        }

        // this runs when you leave the page
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // TODO: stop background updates if needed later
        }

        // ==================================================
        // LOCATION
        // ==================================================
        private async Task CheckAndRequestLocationAsync(bool forceRequest = false)
        {
            try
            {
                // check if this is the first launch or forced refresh
                bool firstLaunch = Preferences.Get("IsFirstLaunch", true);

                if (firstLaunch || forceRequest)
                {
                    // check location permission
                    var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                    // if not granted, ask for permission
                    if (permissionStatus != PermissionStatus.Granted)
                    {
                        permissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    }

                    // mark as already launched
                    Preferences.Set("IsFirstLaunch", false);
                }

                // create location request (medium accuracy, 10s timeout)
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                // get user's current location
                var userLocation = await Geolocation.GetLocationAsync(request);

                if (userLocation != null)
                {
                    // save coordinates
                    lastLatitude = userLocation.Latitude;
                    lastLongitude = userLocation.Longitude;

                    // get city and region name from coordinates
                    var places = await Geocoding.GetPlacemarksAsync(userLocation);
                    var place = places.FirstOrDefault();

                    if (place != null)
                    {
                        // show city and region in label
                        UpdateLocationLabel($"{place.Locality}, {place.AdminArea}");
                    }
                    else
                    {
                        // show coordinates if no city found
                        UpdateLocationLabel($"Lat: {userLocation.Latitude:F2}, Lon: {userLocation.Longitude:F2}");
                    }

                    // load ISS next passes and position once
                    await LoadNextSightings(userLocation.Latitude, userLocation.Longitude);
                    await LoadCurrentPosition(userLocation.Latitude, userLocation.Longitude);
                }
                else
                {
                    // if GPS fails
                    UpdateLocationLabel("Location unavailable");
                }
            }
            catch
            {
                // if something goes wrong
                UpdateLocationLabel("Error retrieving location");
            }
        }

        // this runs when user taps the location label
        private async void OnLocationStatusLabelTapped(object? sender, TappedEventArgs e)
        {
            // calls the same location check but forces refresh
            await CheckAndRequestLocationAsync(true);
        }

        // ==================================================
        // UPDATE LOCATION LABEL
        // ==================================================
        // this safely updates UI from background threads
        // parameter "message" is the text to show on the label
        // message comes from CheckAndRequestLocationAsync()
        private void UpdateLocationLabel(string message)
        {
            // run UI code on main thread to avoid app crash
            MainThread.BeginInvokeOnMainThread(delegate
            {
                if (LocationStatusLabel != null)
                {
                    // this updates the text visible to user
                    LocationStatusLabel.Text = message;
                }
            });
        }

        // ==================================================
        // NEXT SIGHTINGS (NEXT ISS PASS)
        // ==================================================
        private async Task LoadNextSightings(double latitude, double longitude)
        {
            // create service that calls API
            var service = new Services.IssService();

            // get next ISS passes based on location
            var issPasses = await service.GetNextSightingsAsync(latitude, longitude);

            if (issPasses != null && issPasses.Count > 0)
            {
                // take the first upcoming pass
                var nextPass = issPasses[0];

                // show next pass time
                if (!string.IsNullOrEmpty(nextPass.NextPassTime))
                {
                    NextPassLabel.Text = nextPass.NextPassTime;
                }
                else
                {
                    NextPassLabel.Text = "Next pass time unavailable";
                }

                // show duration and max elevation
                DurationLabel.Text = nextPass.DurationText;
                ElevationLabel.Text = !string.IsNullOrEmpty(nextPass.MaxElevationText)
                    ? nextPass.MaxElevationText
                    : $"Max Elevation: {nextPass.MaxEl}°";
            }
            else
            {
                // if no passes found
                NextPassLabel.Text = "No passes available in the next 48hrs.";
                DurationLabel.Text = "";
                ElevationLabel.Text = "";
            }
        }

        // ==================================================
        // INFO SECTION (CURRENT POSITION OF ISS)
        // ==================================================
        private async Task LoadCurrentPosition(double latitude, double longitude)
        {
            // call API to get current ISS position
            var service = new Services.IssService();
            var issPositions = await service.GetCurrentPositionAsync(latitude, longitude);

            if (issPositions != null && issPositions.Count > 0)
            {
                // take first position result
                var currentPosition = issPositions[0];

                // show formatted coordinates and altitude
                LatitudeLabel.Text = currentPosition.Latitude.ToString("F2");
                LongitudeLabel.Text = currentPosition.Longitude.ToString("F2");
                AltitudeLabel.Text = currentPosition.AltitudeText;

                // placeholders (not yet implemented)
                AzimuthLabel.Text = currentPosition.AzimuthText;
                ElevationInfoLabel.Text = currentPosition.ElevationText;
            }
            else
            {
                // show default values if API fails
                LatitudeLabel.Text = "--";
                LongitudeLabel.Text = "--";
                AltitudeLabel.Text = "--";
                ElevationInfoLabel.Text = "--";
                AzimuthLabel.Text = "--";
            }
        }

        // ==================================================
        // LOOP TO UPDATE INFO SECTION AUTOMATICALLY
        // ==================================================
        private async Task StartInfoSectionLoop()
        {
            // simple loop that updates ISS info automatically
            // positions endpoint allows 1000 requests per hour (~1 call every 4s)

            for (int i = 0; i < 900; i++) // about 1 hour worth of calls
            {
                // only update if we already have valid coordinates
                if (lastLatitude != 0 && lastLongitude != 0)
                {
                    await LoadCurrentPosition(lastLatitude, lastLongitude);
                }

                // wait 4 seconds before next update
                await Task.Delay(4000);
            }
        }
    }
}