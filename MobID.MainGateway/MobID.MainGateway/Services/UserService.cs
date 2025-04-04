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

            var simpleRoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = simpleRoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRoleRepository.Add(userRole, ct);

            user.UserRoles = new List<UserRole> { userRole };

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

        public async Task<bool> AssignRoleToUser(Guid userId, Guid roleId, CancellationToken ct = default)
        {
            if (!await _userRepository.IsIdPresent(userId))
            {
                return false;
            }
            
            var existing = await _userRoleRepository.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
            if (existing != null)
                return false;

            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRoleRepository.Add(userRole, ct);
            return true;
        }

        public async Task<bool> RemoveRoleFromUser(Guid userId, Guid roleId, CancellationToken ct = default)
        {
            if (!await _userRepository.IsIdPresent(userId))
            {
                return false;
            }

            var userRole = await _userRoleRepository.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
            if (userRole == null)
                return false;

            await _userRoleRepository.Remove(userRole, ct);
            return true;
        }

        public async Task<List<string>> GetUserRoles(Guid userId, CancellationToken ct = default)
        {
            if (!await _userRepository.IsIdPresent(userId))
            {
                return Enumerable.Empty<string>().ToList();
            }

            var roles = await _userRoleRepository.GetWhereWithInclude(ur => ur.UserId == userId, ct, ur => ur.Role);
            return roles.Select(r => r.Role.Name).ToList();
        }

    }
}
