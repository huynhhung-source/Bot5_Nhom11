using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;
using System.Security.Claims;

namespace doanweb.Services
{
    public interface IOAuthService
    {
        Task<User> GetOrCreateUserAsync(ClaimsPrincipal principal, string provider);
    }

    public class OAuthService : IOAuthService
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<OAuthService> _logger;

        public OAuthService(GymDbContext dbContext, ILogger<OAuthService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<User> GetOrCreateUserAsync(ClaimsPrincipal principal, string provider)
        {
            // Extract claims from the principal
            var email = principal.FindFirst(ClaimTypes.Email)?.Value 
                     ?? principal.FindFirst("urn:facebook:email")?.Value
                     ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            var name = principal.FindFirst(ClaimTypes.Name)?.Value
                    ?? principal.FindFirst("urn:facebook:name")?.Value
                    ?? principal.FindFirst(ClaimTypes.GivenName)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException($"Unable to extract email from {provider} claims");
            }

            // Check if user already exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                _logger.LogInformation($"User found: {email}");
                return existingUser;
            }

            // Create new user
            var newUser = new User
            {
                FullName = name ?? "User",
                Email = email,
                PhoneNumber = "",
                Address = "",
                Gender = "Other",
                CreatedDate = DateTime.Now,
                Status = "Active",
                PasswordHash = "" // OAuth users don't need password
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"New user created via {provider}: {email}");

            return newUser;
        }
    }
}
