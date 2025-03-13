namespace SmartPass.Repository.Models.EntityInterfaces
{
    public interface ICreateUpdateable
    {
        DateTime CreateUtcDate { get;}
        DateTime? UpdateUtcDate { get;}
    }
}
