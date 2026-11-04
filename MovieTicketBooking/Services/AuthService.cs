
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        // Customer Registration
        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request)
        {
            // Normalize email
            string email = request.Email.Trim().ToLowerInvariant();

            // Check whether email already exists
            var existingUser =
                await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "Email is already registered."
                );
            }

            // Create new user
            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = email,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow
            };

            // Hash password before saving
            user.PasswordHash =
                _passwordService.HashPassword(user, request.Password);

            // Save user in database
            await _userRepository.AddAsync(user);

            // Generate JWT token
            string token = _jwtService.GenerateToken(user);

            // Calculate token expiration
            var jwtSettings = _configuration.GetSection("Jwt");

            double duration = Convert.ToDouble(
                jwtSettings["DurationInMinutes"]
            );

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(duration),
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        // Customer Login
        public async Task<AuthResponseDto?> LoginAsync(
            LoginRequestDto request)
        {
            // Normalize email
            string email = request.Email.Trim().ToLowerInvariant();

            // Find user by email
            var user =
                await _userRepository.GetByEmailAsync(email);

            // Check whether user exists
            if (user == null)
            {
                return null;
            }

            // Verify password
            bool isPasswordValid =
                _passwordService.VerifyPassword(user, request.Password);

            if (!isPasswordValid)
            {
                return null;
            }

            // Generate JWT token
            string token = _jwtService.GenerateToken(user);

            // Calculate token expiration
            var jwtSettings = _configuration.GetSection("Jwt");

            double duration = Convert.ToDouble(
                jwtSettings["DurationInMinutes"]
            );

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(duration),
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
