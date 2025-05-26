using MobID.MainGateway.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req.Validators
{
    public class ValidAccessLevelsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var level = value as string;
            if (level == null)
            {
                return false;
            }

            return EntityConstants.AccessLevels.Contains(level);
        }
    }
}
