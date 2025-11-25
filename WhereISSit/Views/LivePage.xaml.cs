using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;

namespace WhereISSit.Views
{
    public partial class LivePage : ContentPage
    {
        public LivePage()
        {
            InitializeComponent();

            CheckConnection();

            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        private void CheckConnection()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                NoConnectionLabel.IsVisible = false;
            }
            else
            {
                NoConnectionLabel.IsVisible = true;
            }
        }

        private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            CheckConnection();
        }
    }
}