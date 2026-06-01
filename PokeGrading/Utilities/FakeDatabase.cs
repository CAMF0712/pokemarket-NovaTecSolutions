using PokeGrading.Models;

namespace PokeGrading.Utilities
{
    public static class FakeDatabase
    {
        public static List<User> Users =
            new List<User>
            {
                new User
                {
                    UserId = Guid.NewGuid(),

                    Email =
                        "admin@pokegrading.com",

                    Alias =
                        "Administrator",

                    PasswordHash =
                        PasswordService.HashPassword(
                            "Admin123"),

                    Country =
                        "CR",

                    PreferredLanguage =
                        "EN",

                    Role =
                        "ADMIN",

                    Active =
                        true,

                    CreatedAt =
                        DateTime.UtcNow,

                    LastLogin =
                        null
                }
            };
    }
}