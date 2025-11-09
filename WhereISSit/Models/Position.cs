using System;
using System.Text.Json.Serialization;

namespace WhereISSit.Models
{
    public class Position
    {
        [JsonPropertyName("satlatitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("satlongitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("sataltitude")]
        public double AltitudeKm { get; set; }
        public string AltitudeText => $"{AltitudeKm:F1} km";

        [JsonPropertyName("azimuth")]
        public double Azimuth { get; set; }
        public string AzimuthText => $"{Azimuth:F1}°";

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }
        public string ElevationText => $"{Elevation:F1}°";

        public Position() { }
    }
}