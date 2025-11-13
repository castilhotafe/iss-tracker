using System;
using System.Text.Json.Serialization;

namespace WhereISSit.Models
{
    public class PassResponse
        {
            [JsonPropertyName("passes")]
            public List<IssPass> Passes { get; set; } =new();
        }
}