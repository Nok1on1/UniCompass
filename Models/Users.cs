using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniCompass.Models
{
    [Table("users")]
    public class Users : BaseModel
    {
        [PrimaryKey("user_id")]
        public Guid UserId { get; set; }

        [Column("username")]
        public string? Username { get; set; }

        [Column("user_type")]
        public string? UserType { get; set; }

        [Column("first_name")]
        public string? FirstName { get; set; }

        [Column("last_name")]
        public string? LastName { get; set; }

        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        [Column("photo_public_id")]
        public string? PhotoPublicId { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
