using System;

namespace UniCompass.Models;

public class JsonUniversitiesData
{
    public JsonUniversityMetadata MetaData { get; set; }
    public List<JsonUniversity> Universities { get; set; }
}
