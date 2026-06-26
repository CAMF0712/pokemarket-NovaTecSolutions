using PokeGrading.Models;

namespace PokeGrading.Repositories
{
    public class UserRegistrationRecord
    {
        public required Guid UserId { get; init; }

        public required int RoleId { get; init; }

        public required string Email { get; init; }

        public required string Alias { get; init; }

        public required string PasswordHash { get; init; }

        public required string Country { get; init; }

        public required string PreferredLanguage { get; init; }

        public required string Status { get; init; }

        public required bool Active { get; init; }

        public required DateTime CreatedAt { get; init; }
    }

    public interface IUserRepository
    {
        bool UserExists(Guid userId);

        bool EmailExists(string email);

        User? GetByEmail(string email);

        void UpdateLastLogin(Guid userId);

        void CreateUser(UserRegistrationRecord record);
    }
}
