using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniCompass.Models
{
    [Table("universities")]
    public class Universities : BaseModel
    {
        [PrimaryKey("university_id")]
        [Column("university_id")]
        public string? UniversityId { get; set; }

        [Column("university_name")]
        public string? UniversityName { get; set; }

        [Column("location")]
        public string? Location { get; set; }

        [Column("established_date")]
        public DateTime? EstablishedDate { get; set; }

        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        [Column("photo_public_id")]
        public string? PhotoPublicId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("bio")]
        public String? Bio { get; set; }
    }
}
