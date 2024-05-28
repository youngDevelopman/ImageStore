using ImageStore.Application.Configuration;
using ImageStore.Application.Exceptions;
using ImageStore.Application.Utils;
using ImageStore.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ImageStore.Application.Users.Commands.GenerateAccessToken
{
    public class GenerateAccessTokenCommandHandler : IRequestHandler<GenerateAccessTokenCommand, GenerateAccessTokenResultDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtConfiguration _jwtConfiguration;

        private const string TOKEN_TYPE = "Bearer";

        public GenerateAccessTokenCommandHandler(IUserRepository userRepository, IOptions<JwtConfiguration> jwtConfiguration)
        {
            _userRepository = userRepository;
            _jwtConfiguration = jwtConfiguration.Value;
        }

        public async Task<GenerateAccessTokenResultDto> Handle(GenerateAccessTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if(user == null)
            {
                throw new UserNotFoundException($"User with email {request.Email} does not exist");
            }

            bool isPasswordValid = PasswordHasher.ValidatePassword(request.Password, user.PasswordSalt, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new InvalidPasswordException($"Password is not valid for the user with email {request.Email}");
            }

            var claimsToAdd = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken
                (
                    issuer: _jwtConfiguration.Issuer,
                    audience: _jwtConfiguration.Issuer,
                    claims: claimsToAdd,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials
                );
            

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return new GenerateAccessTokenResultDto(TOKEN_TYPE, token);
        }
    }
}
