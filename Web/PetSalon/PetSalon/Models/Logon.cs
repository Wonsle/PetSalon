using System.Security;

namespace PetSalon.Models
{
    public class Logon
    {
        public string UserName { get; set; }
        public SecureString Password { get; set; }
    }
}
