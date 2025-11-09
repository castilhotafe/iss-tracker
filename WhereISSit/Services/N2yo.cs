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
            // lat/long virao como argumentos da localizacao do usuario

            // using statement is a context manager that disposes the httpclient to free memory
            using HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new(HttpMethod.Get, requestUrl);

            // Send the request to the API
            HttpResponseMessage httpResponse = await httpClient.SendAsync(request);

            
            if (!httpResponse.IsSuccessStatusCode)
            {
                
                throw new HttpRequestException($"HTTP Error: {httpResponse.StatusCode}");
            }

            // Read the response body content as a JSON string
            string jsonResponse = await httpResponse.Content.ReadAsStringAsync();

            // Parse the JSON response to access its properties
            using JsonDocument document = JsonDocument.Parse(jsonResponse);
            // root accesses the main json object from the api response
            JsonElement root = document.RootElement;

            
            if (!root.TryGetProperty("passes", out JsonElement passesElement))
            {
                //empty list
                return new List<IssPass>();
            }

            // Deserialize the "passes" JSON array into a list of IssPass objects
            List<IssPass>? passList = JsonSerializer.Deserialize<List<IssPass>>(passesElement.GetRawText());

            
            return passList ?? new List<IssPass>();
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

            // parse json response
            using JsonDocument document = JsonDocument.Parse(jsonResponse);
            JsonElement root = document.RootElement;

            // check if "positions" property exists
            if (!root.TryGetProperty("positions", out JsonElement positionsElement))
            {
                // return empty list if no positions data
                return new List<Position>();
            }

            // deserialize positions json array into list of Position objects
            List<Position>? positionList = JsonSerializer.Deserialize<List<Position>>(positionsElement.GetRawText());

            // return list or empty list if null
            return positionList ?? new List<Position>();
        }
    }
}