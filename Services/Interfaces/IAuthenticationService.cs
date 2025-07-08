using System.Threading.Tasks;

namespace QuitSmartApp.Services.Interfaces
{
    /// <summary>
    /// Authentication service interface for login/logout operations
    /// </summary>
    public interface IAuthenticationService
    {
        Task<bool> LoginUserAsync(string username, string password);
        Task<bool> LoginAdminAsync(string username, string password);
        Task LogoutAsync();
        Task<bool> RegisterUserAsync(string username, string email, string password, string fullName, DateTime? dateOfBirth = null, string? gender = null);
        bool IsUserLoggedIn { get; }
        bool IsAdminLoggedIn { get; }
        Guid? CurrentUserId { get; }
        Guid? CurrentAdminId { get; }
        string? CurrentUsername { get; }
    }
}
