using ERP_Models.Common.Services;
using ERP_Models.DTOs.ERP_Organization;
using ERP_Models.Entities.Common.Responses;
using ERP_Models.Entities.Data.ERP_Organization;
using ERP_OrganizationService.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP_Organization.Services.UserService
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly OrganizationDbContext _context;

        public UserService(OrganizationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApiResponse<User>> RegisterAsync(RegisterUserDto dto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.Trim().ToLower() && !u.IsDeleted);
            if (exists)
                return ApiResponse<User>.Fail("Email already exists");

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId ?? 3,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ApiResponse<User>.Ok(user, "User registered successfully");
        }

        public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            return ApiResponse<List<User>>.Ok(users);
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            return user == null
                ? ApiResponse<User>.Fail("User not found")
                : ApiResponse<User>.Ok(user);
        }

        public async Task<ApiResponse<User>> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return ApiResponse<User>.Fail("User not found");

            user.FullName = dto.FullName.Trim();
            user.PhoneNumber = dto.PhoneNumber;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ApiResponse<User>.Ok(user, "User updated");
        }

        public async Task<ApiResponse<bool>> SoftDeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return ApiResponse<bool>.Fail("User not found");

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "User deleted");
        }

        // AUTH METHODS — USED BY AUTHSERVICE
        public async Task<ApiResponse<User>> GetUserWithHashByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted && u.IsActive);

            return user == null
                ? ApiResponse<User>.Fail("User not found or inactive")
                : ApiResponse<User>.Ok(user);
        }

        public async Task<ApiResponse<bool>> SaveRefreshTokenAsync(int userId, string token, string jwtId, DateTime expiry)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                Token = token,
                JwtId = jwtId,
                ExpiryDate = expiry
            });

            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }

        public async Task<ApiResponse<bool>> ValidateAndUseRefreshTokenAsync(string token)
        {
            var rt = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);

            if (rt == null) return ApiResponse<bool>.Fail("Invalid or expired token");

            rt.IsUsed = true;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
    }
}