using PokeGrading.Models;
using PokeGrading.Utilities;

namespace PokeGrading.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseService _database;

        public UserRepository(DatabaseService database)
        {
            _database = database;
        }

        public bool UserExists(Guid userId)
        {
            var user = _database.QuerySingleOrDefault<Guid?>(
                @"
                SELECT user_id
                FROM USERS
                WHERE user_id = @user_id
                ",
                new()
                {
                    { "user_id", userId }
                });

            return user != null;
        }

        public bool EmailExists(string email)
        {
            var user = _database.QuerySingleOrDefault<Guid?>(
                @"
                SELECT user_id
                FROM USERS
                WHERE LOWER(email) = LOWER(@email)
                ",
                new()
                {
                    { "email", email }
                });

            return user != null;
        }

        public User? GetByEmail(string email)
        {
            return _database.QuerySingleOrDefault<User>(
                @"
                SELECT
                    user_id AS UserId,
                    role_id AS RoleId,
                    email AS Email,
                    alias AS Alias,
                    password_hash AS PasswordHash,
                    country AS Country,
                    preferred_language AS PreferredLanguage,
                    status AS Status,
                    active AS Active,
                    created_at AS CreatedAt,
                    last_login AS LastLogin
                FROM USERS
                WHERE email = @Email
                ",
                new()
                {
                    { "Email", email }
                });
        }

        public void UpdateLastLogin(Guid userId)
        {
            _database.ExecuteNonQuery(
                @"
                UPDATE USERS
                SET last_login = GETUTCDATE()
                WHERE user_id = @UserId
                ",
                new()
                {
                    { "UserId", userId }
                });
        }

        public void CreateUser(UserRegistrationRecord record)
        {
            _database.ExecuteNonQuery(
                @"
                INSERT INTO USERS
                (
                    user_id,
                    role_id,
                    email,
                    alias,
                    password_hash,
                    country,
                    preferred_language,
                    status,
                    active,
                    created_at
                )
                VALUES
                (
                    @user_id,
                    @role_id,
                    @email,
                    @alias,
                    @password_hash,
                    @country,
                    @preferred_language,
                    @status,
                    @active,
                    @created_at
                )
                ",
                new()
                {
                    { "user_id", record.UserId },
                    { "role_id", record.RoleId },
                    { "email", record.Email },
                    { "alias", record.Alias },
                    { "password_hash", record.PasswordHash },
                    { "country", record.Country },
                    { "preferred_language", record.PreferredLanguage },
                    { "status", record.Status },
                    { "active", record.Active },
                    { "created_at", record.CreatedAt }
                });
        }
    }
}
