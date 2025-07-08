using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Helpers;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Services
{
    // Authentication service implementation for login/logout operations
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IAdminLogRepository _adminLogRepository;

        // Current session state
        private User? _currentUser;
        private Admin? _currentAdmin;

        public AuthenticationService(
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            IAdminLogRepository adminLogRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _adminLogRepository = adminLogRepository;
        }

        public bool IsUserLoggedIn => _currentUser != null;
        public bool IsAdminLoggedIn => _currentAdmin != null;
        public Guid? CurrentUserId => _currentUser?.UserId;
        public Guid? CurrentAdminId => _currentAdmin?.AdminId;
        public string? CurrentUsername => _currentUser?.Username ?? _currentAdmin?.Username;

        public async Task<bool> LoginUserAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.ValidateUserAsync(username, password);
                if (user != null)
                {
                    _currentUser = user;
                    _currentAdmin = null;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoginAdminAsync(string username, string password)
        {
            try
            {
                var admin = await _adminRepository.ValidateAdminAsync(username, password);
                if (admin != null)
                {
                    _currentAdmin = admin;
                    _currentUser = null; // Clear user session

                    // Update last login time
                    await _adminRepository.UpdateLastLoginAsync(admin.AdminId);

                    // Log admin login
                    await _adminLogRepository.LogActionAsync(admin.AdminId, "Login", null, "Admin logged in");

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _currentAdmin = null;
            return Task.CompletedTask;
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password, string fullName, DateTime? dateOfBirth = null, string? gender = null)
        {
            try
            {
                // Check if username or email already exists
                if (await _userRepository.IsUsernameExistsAsync(username))
                    return false;

                if (await _userRepository.IsEmailExistsAsync(email))
                    return false;

                // Create new user
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = username,
                    Email = email,
                    PasswordHash = PasswordHelper.HashPassword(password),
                    FullName = fullName,
                    DateOfBirth = dateOfBirth?.Date == null ? null : DateOnly.FromDateTime(dateOfBirth.Value),
                    Gender = gender,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
