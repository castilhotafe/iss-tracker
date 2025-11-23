using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WhereISSit.Models;

namespace WhereISSit.Services
{
    public class IssService
    {
        
        public async Task<List<IssPass>> GetNextSightingsAsync(double latitude, double longitude)
        {
            
            const string apiKey = "XHX4GS-NRH8Y3-PG2C33-5KZV";
            string baseUrl = "https://api.n2yo.com/rest/v1/satellite";

            
            string requestUrl = $"{baseUrl}/visualpasses/25544/{latitude}/{longitude}/0/10/120/&apiKey={apiKey}";
            //lat/long comming as arguments from the code behing using system Geolocation

            
            using HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new(HttpMethod.Get, requestUrl);

            
            HttpResponseMessage httpResponse = await httpClient.SendAsync(request);

            
            if (!httpResponse.IsSuccessStatusCode)
            {
                
                throw new HttpRequestException($"HTTP Error: {httpResponse.StatusCode}");
            }

            // Read the response body content as a JSON string
            string jsonResponse = await httpResponse.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize<PassResponse>(jsonResponse);
            
            return response?.Passes ?? new List<IssPass>();
        }




        public async Task<List<Position>> GetCurrentPositionAsync(double latitude, double longitude)
        {
            // api key constant
            const string apiKey = "XHX4GS-NRH8Y3-PG2C33-5KZV";
            // base url for the api
            string baseUrl = "https://api.n2yo.com/rest/v1/satellite";

            // build request url for positions endpoint
            string requestUrl = $"{baseUrl}/positions/25544/{latitude}/{longitude}/300/60/&apiKey={apiKey}";

            // create http client and request message
            using HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new(HttpMethod.Get, requestUrl);

            // send request to the api
            HttpResponseMessage httpResponse = await httpClient.SendAsync(request);

            // check if response is successful
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP Error: {httpResponse.StatusCode}");
            }

            // read response content as json string
            string jsonResponse = await httpResponse.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize<PositionResponse>(jsonResponse);
            return response?.Positions ?? new List<Position>();
        }
    }
}