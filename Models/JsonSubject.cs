using System;
using System.Text.Json.Serialization;

namespace UniCompass.Models;

public class JsonSubject
{
    [JsonPropertyName("subject_id")]
    public string SubjectId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; }
}
