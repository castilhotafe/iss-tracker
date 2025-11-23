using System;
using System.Text.Json.Serialization;
using WhereISSit.Services; //To use time format service

namespace WhereISSit.Models
{
    public class IssPass
    {
        [JsonPropertyName("startUTC")]
        public long StartUTC { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("maxEl")]
        public double MaxEl { get; set; }

        public string NextPassTime
        {
            get
            {
                // Convert UNIX to UTC
                DateTime utcTime = DateTimeOffset.FromUnixTimeSeconds(StartUTC).UtcDateTime;

                //UTC to local time
                DateTime localTime = utcTime.ToLocalTime();

                //use the preference from the service
                string userFormat = TimeFormat.Get();

                if (userFormat == "12h")
                {
                    
                    return localTime.ToString("h:mm tt, dd MMM yyyy");
                }

                
                return localTime.ToString("HH:mm, dd MMM yyyy");
            }
        }

        
        public string DurationText
        {
            get
            {
                int minutes = Duration / 60;
                int seconds = Duration % 60;
                return $"Duration: {minutes} min {seconds} sec";
            }
        }

        
        public string MaxElevationText
        {
            get
            {
                return $"Max Elevation: {MaxEl}°";
            }
        }

        public IssPass() { }
    }
}