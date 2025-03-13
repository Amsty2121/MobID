using MobID.MainGateway.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req.Validators
{
    public class ValidRolesAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var roles = value as ICollection<string>;
            if (roles == null)
            {
                return false;
            }

            return roles.All(role => EntityConstants.Roles.Contains(role));
        }
    }
}
