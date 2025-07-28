using System;
using System.Text.Json.Serialization;

namespace UniCompass.Models;

public class JsonUniversity
{
    [JsonPropertyName("uni_id")]
    public string UniId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("subjects")]
    public List<JsonSubject> Subjects { get; set; }
}
