using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;

        public UserService(IGenericRepository<User> userRepository, IGenericRepository<UserRole> userRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
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
            if (user == null)
                return false;

            user.DeletedAt = DateTime.UtcNow;
            await _userRepository.Update(user, ct);
            return true;
        }

        public async Task<UserDto?> GetUserById(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetById(userId, ct);
            return user == null ? null : new UserDto(user);
        }

        public async Task<PagedResponse<UserDto>> GetUsersPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            int offset = pagedRequest.PageIndex * pagedRequest.PageSize;
            var userList = (await _userRepository.GetWhere(u => u.DeletedAt == null, ct))?.ToList() ?? new List<User>();
            int total = userList.Count;
            var result = new List<UserDto>();

            var pagedUsers = userList.Skip(offset).Take(pagedRequest.PageSize).ToList();
            foreach (var user in pagedUsers)
            {
                var roles = await _userRoleRepository.GetWhereWithInclude(r => r.UserId == user.Id, ct, r => r.Role);
                result.Add(new UserDto(user, roles.Select(r => r.Role.Name).ToList()));
            }

            return new PagedResponse<UserDto>(pagedRequest.PageIndex, pagedRequest.PageSize, total, result);
        }

    }
}
