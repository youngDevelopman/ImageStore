using ImageStore.Application.Exceptions;
using ImageStore.Application.Utils;

namespace ImageStore.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResultDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterUserResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user != null)
            {
                throw new UserAlreadyExistsException($"User with email {user.Email} already exists");
            }

            (string salt, string hash) = PasswordHasher.HashPassword(request.Password);
            var newUser = new User()
            {
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = hash,
                PasswordSalt = salt,
            };

            await _userRepository.AddUserAdync(newUser, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            return new RegisterUserResultDto(newUser.Id, newUser.Email, newUser.UserName);
        }
    }
}
