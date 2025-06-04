using Microsoft.IdentityModel.Tokens;
using Phone_api.Dtos;
using Phone_api.Entities;
using Phone_api.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Phone_api.Services
{
    /// <summary>
    /// Service xử lý việc tạo JWT token.
    /// </summary>
    public class JwtTokenService
    {
        private readonly string _jwtKey;

        /// <summary>
        /// Khởi tạo JwtTokenService với cấu hình.
        /// </summary>
        /// <param name="configuration">Cấu hình chứa khóa JWT.</param>
        public JwtTokenService(IConfiguration configuration)
        {
            _jwtKey = configuration["Jwt:Key"] ?? "";
            if (string.IsNullOrEmpty(_jwtKey)) throw new InvalidOperationException("JWT Key is not configured.");
        }

        /// <summary>
        /// Tạo JWT token cho người dùng.
        /// </summary>
        /// <param name="user">Thông tin người dùng.</param>
        /// <param name="permissions">Danh sách quyền của người dùng (ví dụ: phone.getbyid).</param>
        /// <returns>Thông tin token bao gồm thời hạn hết hạn, token, và thông tin người dùng.</returns>
        public UserLoginResponse GenerateToken(User user, List<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? "None"),
                    new Claim("permissions", JsonSerializer.Serialize(permissions)) // Lưu permissions dưới dạng mảng JSON
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new UserLoginResponse
            {
                Expire = ((DateTimeOffset)tokenDescriptor.Expires.Value).ToUnixTimeSeconds(),
                Token = tokenString,
                User = user.ToDto()
            };
        }
    }
}
