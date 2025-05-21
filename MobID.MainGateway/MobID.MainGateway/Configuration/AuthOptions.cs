using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MobID.MainGateway.Configuration 
{
    public class AuthOptions
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenLifetimeDays { get; set; }
        public int RefreshTokenLifetimeDays { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            if (SecretKey == null)
            {
                string errorMessage = "Login failed - invalid SecretKey";

                throw new ArgumentNullException(errorMessage);
            }

            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}
