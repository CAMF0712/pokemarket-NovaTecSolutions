// Repositorio: encapsula acceso a datos para UserRepository.
using PokeGrading.Models;
using PokeGrading.Utilities;

namespace PokeGrading.Repositories
{
    /// <summary>
    /// Clase principal que concentra la responsabilidad de UserRepository en esta capa.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseService _database;

        /// <summary>
        /// Inicializa una nueva instancia de UserRepository.
        /// </summary>
        public UserRepository(DatabaseService database)
        {
            _database = database;
        }

        /// <summary>
        /// Verifica la existencia de un usuario por su identificador.
        /// </summary>
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

        /// <summary>
        /// Verifica si el correo ya esta registrado en el sistema.
        /// </summary>
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

        /// <summary>
        /// Recupera un usuario por correo para autenticacion y validaciones.
        /// </summary>
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

        /// <summary>
        /// Actualiza la fecha de ultimo acceso del usuario autenticado.
        /// </summary>
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

        /// <summary>
        /// Crea un nuevo usuario persistiendo su informacion principal.
        /// </summary>
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

