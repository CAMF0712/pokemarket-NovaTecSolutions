namespace PokeGrading.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public int RoleId { get; set; }

        public string Email { get; set; }

        public string Alias { get; set; }

        public string PasswordHash { get; set; }

        public string Country { get; set; }

        public string PreferredLanguage { get; set; }

        public string Status { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}