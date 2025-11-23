using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhereISSit.Views
{
    public partial class StalkingPage : ContentPage
    {
        private double lastLatitude;
        private double lastLongitude;

        public StalkingPage()
        {
            InitializeComponent();
            ConfigureTapGesture();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await HandleLocationFlowAsync();  //Load next sightings
            _ = StartAutoUpdateLoopAsync();   // initiate loop for positions
        }

        
        //tap config
        private void ConfigureTapGesture()
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += OnLocationLabelTapped;
            LocationStatusLabel.GestureRecognizers.Add(tap);
        }

        //Tap initiates location flow
        private async void OnLocationLabelTapped(object? sender, TappedEventArgs e)
        {
            await HandleLocationFlowAsync(forceRequest: true);
        }

        //main method location flow
        private async Task HandleLocationFlowAsync(bool forceRequest = false)
        {
            try
            {
                await EnsureLocationPermissionAsync(forceRequest);//#1 permission

                var location = await GetUserLocationAsync();//#2 location
                if (location == null)
                {
                    UpdateLocationStatus("Location unavailable");//#3 update label
                    return;
                }

                lastLatitude = location.Latitude;
                lastLongitude = location.Longitude;

                await ShowUserCityAsync(location);//#4 
                await UpdateIssInformationAsync(location);
            }
            catch
            {
                UpdateLocationStatus("Error retrieving location");
            }
        }

        //#1
        private async Task EnsureLocationPermissionAsync(bool forceRequest)
        {
            bool firstLaunch = Preferences.Get("IsFirstLaunch", true);

            if (!firstLaunch && !forceRequest)
                return;

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            Preferences.Set("IsFirstLaunch", false);
        }

        //#2 locatrion
        private async Task<Location?> GetUserLocationAsync()
        {
            var request = new GeolocationRequest(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(10)
            );

            return await Geolocation.GetLocationAsync(request);
        }

        //#3 update labels
        private void UpdateLocationStatus(string text)
        {
            if (LocationStatusLabel != null)
            {
                LocationStatusLabel.Text = text;
            }
        }

        //#4 City label
        private async Task ShowUserCityAsync(Location location)
        {
            var places = await Geocoding.GetPlacemarksAsync(location);
            var place = places.FirstOrDefault();

            UpdateLocationStatus($"{place?.Locality}, {place?.AdminArea}");//#3
        }



        // ---------------------------------------------------------------------------------------------



        //call service request
        private async Task UpdateIssInformationAsync(Location location)
        {
            await LoadNextPassAsync(location.Latitude, location.Longitude); // #1
            await LoadCurrentPositionAsync(location.Latitude, location.Longitude); // #2
        }

        //next sightings card populate
        private async Task LoadNextPassAsync(double latitude, double longitude) // #1
        {
            var service = new Services.IssService();
            var passes = await service.GetNextSightingsAsync(latitude, longitude);

            if (passes == null || passes.Count == 0)
            {
                NextPassLabel.Text = "No passes available in the next 48hrs.";
                DurationLabel.Text = "";
                ElevationLabel.Text = "";
                return;
            }

            var pass = passes[0];

            NextPassLabel.Text = pass.NextPassTime;
            DurationLabel.Text = pass.DurationText;
            ElevationLabel.Text = pass.MaxElevationText;
        }

        //populate extra infos
        private async Task LoadCurrentPositionAsync(double latitude, double longitude) // #2
        {
            var service = new Services.IssService();
            var positions = await service.GetCurrentPositionAsync(latitude, longitude);// passes the parameters lat/lon to create request

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

        //update poositions whitin API limit (less than 1000 requests per hour)
        private async Task StartAutoUpdateLoopAsync()
        {
            for (int i = 0; i < 900; i++)
            {
                if (lastLatitude != 0 && lastLongitude != 0)
                    await LoadCurrentPositionAsync(lastLatitude, lastLongitude); //#2 

                await Task.Delay(4000);
            }
        }
    }
}