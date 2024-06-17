using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Discerniy.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IHashService hashService;
        private readonly IDistributedCache cache;
        private HttpContext httpContext { get; set;}
        private readonly JwtOption jwtOption;

        /// <summary>
        /// 0 - clientId
        /// 1 - sessionId
        /// </summary>
        private readonly string cacheKeyFormat = "client:{0}:session:{1}";

        public AuthService(IUserRepository userRepository, IHashService hashService, IDistributedCache cache, IHttpContextAccessor httpContextAccessor, JwtOption jwtOption)
        {
            this.userRepository = userRepository;
            this.hashService = hashService;
            this.cache = cache;
            this.httpContext = httpContextAccessor.HttpContext;
            this.jwtOption = jwtOption;
        }

        public void SetHttpContext(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public async Task<TokenResponse> Login(LoginModelRequest request)
        {
            var client = await userRepository.GetByEmail(request.Email);
            if (client is null)
            {
                throw new UnauthorizedAccessException();
            }

            if (client.NeedPasswordChange)
            {
                throw new BadRequestException("User is not activated. Please check your email.");
            }

            if (!hashService.Verify(request.Password, client.Password))
            {
                throw new UnauthorizedAccessException();
            }


            var session = await userRepository.CreateSession(client.Id);

            string token = CreateSession(client, ref session);

            client.Sessions.Add(session);
            await userRepository.Update(client);

            return new TokenResponse(token, session.ExpiresAt, session.LastUpdated);
        }

        public async Task<DeviceTokenResponse> GenerateDeviceToken(string userId)
        {
            var currentUser = await this.GetUser();

            currentUser.Permissions.Has(p => p.Users.CanCreateDeviceToken);

            var user = await userRepository.Get(userId);
            if (user is null)
            {
                throw new UnauthorizedAccessException();
            }

            if (user.AccessLevel > currentUser.AccessLevel)
            {
                throw new UnauthorizedAccessException();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = GenerateDeviceJwtToken(user);

            return new DeviceTokenResponse(tokenHandler.WriteToken(token), token.ValidTo, token.ValidFrom, user.UpdateLocationSecondsInterval);
        }

        public async Task<DeviceTokenResponse> RefreshDeviceToken()
        {
            var clientType = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? throw new UnauthorizedAccessException();

            if (clientType != "device")
            {
                throw new UnauthorizedAccessException("Only devices can refresh device tokens.");
            }

            var clientId = GetClientId();

            var user = await userRepository.Get(clientId);
            if (user is null)
            {
                throw new UnauthorizedAccessException();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = GenerateDeviceJwtToken(user);

            return new DeviceTokenResponse(tokenHandler.WriteToken(token), token.ValidTo, token.ValidFrom, user.UpdateLocationSecondsInterval);
        }

        public async Task<UserModel> GetUser()
        {
            var clientId = GetClientId();
            var sessionId = GetClientSessionId();

            var client = await userRepository.Get(clientId);
            if (client is null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!client.Sessions.Any(p => p.Id == sessionId))
            {
                throw new UnauthorizedAccessException();
            }

            return client;
        }

        public async Task<UserModel> GetUserByDevice()
        {
            var clientId = GetClientId();

            if(httpContext.User.FindFirst(ClaimTypes.Role)?.Value != "device")
            {
                throw new UnauthorizedAccessException();
            }

            var client = await userRepository.Get(clientId);
            if (client is null)
            {
                throw new UnauthorizedAccessException();
            }

            return client;
        }

        public string GetClientId()
        {
            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
        }

        public string GetClientSessionId()
        {
            return httpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? throw new UnauthorizedAccessException();
        }

        public async Task<string> Refresh()
        {
            var clientId = GetClientId();
            var sessionId = GetClientSessionId();

            var client = await userRepository.Get(clientId);
            if (client is null)
            {
                throw new UnauthorizedAccessException();
            }
            var session = client.Sessions.FirstOrDefault(p => p.Id == sessionId);
            if (session == null)
            {
                throw new UnauthorizedAccessException();
            }

            session.ExpiresAt = DateTime.UtcNow.AddMinutes(jwtOption.UserExpiresInMinutes);

            string token = CreateSession(client, ref session);

            client.Sessions.Add(session);
            await userRepository.Update(client);

            return token;
        }

        private string CreateSession(IClient client, ref ClientSession session)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtOption.Secret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, client.Id),
                new Claim(ClaimTypes.Role, "user"),
                new Claim(ClaimTypes.Sid, session.Id)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtOption.Issuer,
                Audience = jwtOption.Audience,
                Expires = DateTime.UtcNow.AddMinutes(jwtOption.UserExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            session.ExpiresAt = token.ValidTo;
            session.LastUpdated = DateTime.UtcNow;

            return tokenHandler.WriteToken(token);
        }

        private SecurityToken GenerateDeviceJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtOption.Secret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, "device")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtOption.Issuer,
                Audience = jwtOption.Audience,
                Expires = DateTime.UtcNow.AddMinutes(jwtOption.DeviceExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}
