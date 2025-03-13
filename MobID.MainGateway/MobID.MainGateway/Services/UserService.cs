using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> AddUser(UserAddReq userDto, CancellationToken ct = default)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = userDto.Email,
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.Add(user, ct);
            return new UserDto(user);
        }

        public async Task<bool> DeleteUser(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetById(userId, ct);
            if (user == null) return false;

            user.DeletedAt = DateTime.UtcNow;
            await _userRepository.Update(user, ct);
            return true;
        }

        public async Task<UserDto?> GetUserById(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetById(userId, ct);
            return user == null ? null : new UserDto(user);
        }

        public async Task<UsersPagedRsp> GetUsersPaged(int pageSize, int pageNumber, CancellationToken ct = default)
        {
            var usersQuery = await _userRepository.GetWhere(u => u.DeletedAt == null, ct);
            var totalCount = usersQuery.Count();

            var users = usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto(u))
                .ToList();

            return new UsersPagedRsp(users, totalCount, pageSize, pageNumber);
        }
    }
}
