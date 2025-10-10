using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace WhereISSit
{
    public class IssPass
    {
        public long startUTC { get; set; }
        public int duration { get; set; }
        public double maxEl { get; set; }
        public string? maxAzCompass { get; set; }

        // Novo: propriedades já formatadas para exibição
        public string NextPassTime { get; set; } = "";
        public string DurationText { get; set; } = "";
        public string MaxElevationText { get; set; } = "";

        // Método principal para buscar e formatar dados
        public static async Task<List<IssPass>> GetNextSightingsAsync(double latitude, double longitude)
        {
            string apiKey = "XHX4GS-NRH8Y3-PG2C33-5KZV";
            string url = $"https://api.n2yo.com/rest/v1/satellite/visualpasses/25544/{latitude}/{longitude}/0/2/300/&apiKey={apiKey}";

            using HttpClient client = new();

            try
            {
                string jsonResponse = await client.GetStringAsync(url);

                using var document = JsonDocument.Parse(jsonResponse);
                var root = document.RootElement;

                if (root.TryGetProperty("passes", out JsonElement passesElement))
                {
                    var passes = JsonSerializer.Deserialize<List<IssPass>>(passesElement.GetRawText());

                    // Loop simples para formatar os textos
                    foreach (var pass in passes ?? [])
                    {
                        try
                        {
                            // Converte Unix → hora local
                            DateTime startLocal = DateTimeOffset
                                .FromUnixTimeSeconds(pass.startUTC)
                                .ToLocalTime()
                                .DateTime;

                            pass.NextPassTime = $"Next visible pass: {startLocal:hh:mm tt}";

                            // Duração em minutos
                            int durationMinutes = pass.duration / 60;
                            pass.DurationText = $"Duration: {durationMinutes} min";

                            // Elevação e direção
                            string direction = pass.maxAzCompass ?? "?";
                            pass.MaxElevationText = $"Max Elevation: {pass.maxEl:F0}° {direction}";
                        }
                        catch
                        {
                            pass.NextPassTime = "Error formatting pass data";
                            pass.DurationText = "";
                            pass.MaxElevationText = "";
                        }
                    }

                    return passes ?? new List<IssPass>();
                }

                return new List<IssPass>();
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load ISS data: {ex.Message}", "OK");
                }
                return [];
            }
        }
    }
}