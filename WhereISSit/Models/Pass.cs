using System;
using System.Text.Json.Serialization;

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
        // formatted text version of the start time (UTC converted to local time)
        public string NextPassTime
        {
            get
            {
                // convert unix timestamp (seconds) to readable date
                DateTime passTime = DateTimeOffset.FromUnixTimeSeconds(StartUTC).LocalDateTime;
                return passTime.ToString("HH:mm:ss, dd MMM yyyy");
            }
        }

        // formatted duration text (shows seconds with label)
        public string DurationText
        {
            get
            {
                int minutes = Duration / 60;
                int seconds = Duration % 60;
                return $"Duration: {minutes} min {seconds} sec";
            }
        }

        // formatted elevation text (shows degrees with label)
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