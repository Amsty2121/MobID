namespace MobID.MainGateway.Models.Entities
{
    public class RefreshToken : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }


        public User User { get; set; }
    }
}
