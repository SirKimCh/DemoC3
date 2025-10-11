using Microsoft.AspNetCore.Identity;

namespace BanhMyIT.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int? DomainUserId { get; set; }
    }
}
