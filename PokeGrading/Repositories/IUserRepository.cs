// Contrato de repositorio: define operaciones de persistencia para IUserRepository.
using PokeGrading.Models;

namespace PokeGrading.Repositories
{
    /// <summary>
    /// Modelo interno con la informacion necesaria para registrar un usuario.
    /// </summary>
    public class UserRegistrationRecord
    {
        /// <summary>
        /// Identificador unico del usuario.
        /// </summary>
        public required Guid UserId { get; init; }

        /// <summary>
        /// Rol asignado al usuario en el sistema.
        /// </summary>
        public required int RoleId { get; init; }

        /// <summary>
        /// Correo utilizado para autenticacion.
        /// </summary>
        public required string Email { get; init; }

        /// <summary>
        /// Alias visible del usuario.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Hash de la contrasena almacenado de forma segura.
        /// </summary>
        public required string PasswordHash { get; init; }

        /// <summary>
        /// Pais de residencia del usuario.
        /// </summary>
        public required string Country { get; init; }

        /// <summary>
        /// Idioma preferido de uso dentro de la plataforma.
        /// </summary>
        public required string PreferredLanguage { get; init; }

        /// <summary>
        /// Estado funcional del usuario (por ejemplo, ACTIVE).
        /// </summary>
        public required string Status { get; init; }

        /// <summary>
        /// Indica si el usuario esta habilitado para operar.
        /// </summary>
        public required bool Active { get; init; }

        /// <summary>
        /// Fecha de creacion del registro.
        /// </summary>
        public required DateTime CreatedAt { get; init; }
    }

    /// <summary>
    /// Contrato de operaciones de persistencia para usuarios.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Verifica la existencia de un usuario por su id.
        /// </summary>
        bool UserExists(Guid userId);

        /// <summary>
        /// Verifica si un correo ya se encuentra registrado.
        /// </summary>
        bool EmailExists(string email);

        /// <summary>
        /// Recupera un usuario por correo.
        /// </summary>
        User? GetByEmail(string email);

        /// <summary>
        /// Actualiza la fecha de ultimo inicio de sesion.
        /// </summary>
        void UpdateLastLogin(Guid userId);

        /// <summary>
        /// Persiste un nuevo usuario.
        /// </summary>
        void CreateUser(UserRegistrationRecord record);
    }
}

