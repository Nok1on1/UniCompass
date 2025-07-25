using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniCompass.Models
{
    [Table("university_ratings")]
    public class University_Ratings : BaseModel
    {
        [PrimaryKey("rating_id")]
        [Column("rating_id")]
        public int RatingId { get; set; }

        [Reference(typeof(Universities))]
        [Column("university_id")]
        public int UniversityId { get; set; }

        [Reference(typeof(Users))]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("rating_value")]
        public decimal RatingValue { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("rated_at")]
        public DateTime? RatedAt { get; set; }

        [Column("likes")]
        public int? Likes { get; set; }
    }
}
