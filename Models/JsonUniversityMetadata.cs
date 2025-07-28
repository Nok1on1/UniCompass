using System;
using System.Text.Json.Serialization;

namespace UniCompass.Models;

public class JsonUniversityMetadata
{
    [JsonPropertyName("total_universities")]
    public int TotalUniversities { get; set; }

    [JsonPropertyName("processed_at")]
    public DateTime ProcessedAt { get; set; }
}
