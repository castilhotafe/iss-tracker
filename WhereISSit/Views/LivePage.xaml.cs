using Microsoft.Maui.Controls;

namespace WhereISSit.Views
{
    public partial class LivePage : ContentPage
    {
        public LivePage()
        {
            InitializeComponent();

            // Very simple WebView loading a normal webpage
            string webpageUrl = "https://www.sen.com/live";

            LiveVideoWebView.Source = new UrlWebViewSource
            {
                Url = webpageUrl
            };
        }
    }
}