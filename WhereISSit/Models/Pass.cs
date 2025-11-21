using System;
using System.Text.Json.Serialization;
using WhereISSit.Services; // para usar TimeFormat.Get()

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

        // ============================================================
        // FORMATTED START TIME (UTC → Local + formato 12h/24h)
        // ============================================================
        public string NextPassTime
        {
            get
            {
                // 1. converter UNIX → UTC
                DateTime utcTime = DateTimeOffset.FromUnixTimeSeconds(StartUTC).UtcDateTime;

                // 2. converter UTC → horário local
                DateTime localTime = utcTime.ToLocalTime();

                // 3. pegar formato salvo ("12h" ou "24h")
                string userFormat = TimeFormat.Get();

                if (userFormat == "12h")
                {
                    // Formato 12h
                    return localTime.ToString("h:mm tt, dd MMM yyyy");
                }

                // Formato 24h (padrão)
                return localTime.ToString("HH:mm, dd MMM yyyy");
            }
        }

        // ============================================================
        // Duration text
        // ============================================================
        public string DurationText
        {
            get
            {
                int minutes = Duration / 60;
                int seconds = Duration % 60;
                return $"Duration: {minutes} min {seconds} sec";
            }
        }

        // ============================================================
        // Elevation text
        // ============================================================
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