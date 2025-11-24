using Microsoft.Maui.Controls;

namespace WhereISSit.Views
{
    public partial class LivePage : ContentPage
    {
        public LivePage()
        {
            InitializeComponent();

            
            string webpageUrl = "https://www.sen.com/live";

            LiveVideoWebView.Source = new UrlWebViewSource
            {
                Url = webpageUrl
            };
        }
    }
}