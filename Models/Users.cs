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

        [Column("email")]
        public string? Email { get; set; }

        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("user_type")]
        public string? UserType { get; set; }

        [Column("first_name")]
        public string? FirstName { get; set; }

        [Column("last_name")]
        public string? LastName { get; set; }

        [Column("registration_date")]
        public DateTime? RegistrationDate { get; set; }

        [Column("is_verified")]
        public bool? IsVerified { get; set; }

        [Column("profile_picture_url")]
        public string? ProfilePictureUrl { get; set; }

        [Column("profile_picture_public_id")]
        public string? ProfilePicturePublicId { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
