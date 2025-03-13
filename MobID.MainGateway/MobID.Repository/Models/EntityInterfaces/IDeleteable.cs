namespace SmartPass.Repository.Models.EntityInterfaces
{
    public interface IDeleteable
    {
        bool IsDeleted { get; }
        DateTime? DeletedUtcDate { get; }
    }
}
