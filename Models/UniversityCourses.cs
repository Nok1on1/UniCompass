using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniCompass.Models;

[Table("university_courses")]
public class UniversityCourses : BaseModel
{
    [PrimaryKey("course_id")]
    [Column("course_id")]
    public string? CourseId { get; set; }

    [Reference(typeof(Universities))]
    [Column("university_id")]
    public string? UniversityId { get; set; }

    [Column("degree_id")]
    public int DegreeId { get; set; }

    [Column("course_name")]
    public string CourseName { get; set; } = string.Empty;

    [Column("course_description")]
    public string? CourseDescription { get; set; }

    [Column("price")]
    public int price { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
}
