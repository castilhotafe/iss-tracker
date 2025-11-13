using System;
using System.Text.Json.Serialization;

namespace WhereISSit.Models
{
    public class PositionResponse
        {
            [JsonPropertyName("positions")]
            public List<Position> Positions { get; set; } =new();
        }
}