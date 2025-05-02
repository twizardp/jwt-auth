using Application.AppConfigs;
using Application.Services.Identity;
using Common.Requests;
using Common.Responses;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AppConfiguration _appConfiguration;

        public TokenService(UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<AppConfiguration> appConfiguration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfiguration = appConfiguration.Value;
        }

        /// <summary>
        /// 获取用户的 JWT 访问令牌和刷新令牌。
        /// </summary>
        /// <param name="tokenRequest">包含用户登录信息的请求对象。</param>
        /// <returns>返回一个包含 TokenResponse 的响应包装器，其中包含生成的令牌信息或错误消息。</returns>
        public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
        {
            var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
            if (user is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("invalid credentials.");
            }

            if (!user.IsActive)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("user not active.");
            }

            if (!user.EmailConfirmed)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("email not confirmed.");
            }

            var isPasswordVaild = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
            if (isPasswordVaild)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("invalid credentials.");
            }
            // 更新用户的刷新令牌和过期时间
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            // 生成 JWT 令牌并返回
            var token = await GenerateTokenAsync(user);
            var response = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryDate.Value
            };

            return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
        }

        /// <summary>
        /// 获取新的访问令牌（通过刷新令牌）。
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("invalid refresh token.");
            }

            var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
            var user = await _userManager.FindByEmailAsync(userPrincipal.FindFirstValue(ClaimTypes.Email));

            if (user is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("user not found.");
            }
            if (user.RefreshToken != refreshTokenRequest.RefreshToken
                || user.RefreshTokenExpiryDate <= DateTime.Now)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("invalid refresh token.");
            }

            var token = CenerateEncryptedToken(await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryDate.Value
            };
            return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
        }

        /// <summary>
        /// 从过期的JWT令牌中提取用户声明信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token.");
            }
            return principal;
        }

        /// <summary>
        /// 生成一个随机的 Base64 编码的刷新令牌。
        /// </summary>
        /// <returns>Base64 编码的字符串作为刷新令牌。</returns>
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        /// <summary>
        /// 为指定用户生成 JWT 访问令牌。
        /// </summary>
        /// <param name="user">需要生成令牌的用户。</param>
        /// <returns>生成的 JWT 访问令牌字符串。</returns>
        private async Task<string> GenerateTokenAsync(User user)
        {
            var token = CenerateEncryptedToken(await GetClaimsAsync(user));
            return token;
        }

        /// <summary>
        /// 使用给定的声明生成并加密 JWT 令牌。
        /// </summary>
        /// <param name="claims">要包含在令牌中的声明集合。</param>
        /// <returns>生成的 JWT 令牌字符串。</returns>
        private string CenerateEncryptedToken(IEnumerable<Claim> claims)
        {
            var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 获取与指定用户关联的所有声明（包括用户声明、角色声明和权限声明）。
        /// </summary>
        /// <param name="user">目标用户。</param>
        /// <returns>包含所有声明的 IEnumerable&lt;Claim&gt; 集合。</returns>
        private async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var currentRole = await _roleManager.FindByNameAsync(role);
                var allPermissionForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
                permissionClaims.AddRange(allPermissionForCurrentRole);
            }

            var claims = new List<Claim>()
            {
                new (ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.MobilePhone,user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }
    }
}
