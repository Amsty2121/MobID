namespace MobID.MainGateway.Models.Dtos.Rsp
{
    public class UsersPagedRsp
    {
        public List<UserDto> Users { get; }
        public int TotalCount { get; }
        public int PageSize { get; }
        public int PageNumber { get; }
        public bool HasMore => (PageNumber * PageSize) < TotalCount;

        public UsersPagedRsp(List<UserDto> users, int totalCount, int pageSize, int pageNumber)
        {
            Users = users;
            TotalCount = totalCount;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }
}
