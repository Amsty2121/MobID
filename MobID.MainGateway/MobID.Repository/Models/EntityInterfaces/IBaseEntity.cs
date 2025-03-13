using System.ComponentModel.DataAnnotations;
namespace SmartPass.Repository.Models.EntityInterfaces
{
    public interface IBaseEntity : IDeleteable, ICreateUpdateable
    {
        Guid Id { get; }
    }
}
