using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BugTracker.Extensions
{
    public class BTUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<BTUser, IdentityRole>
    {
        public BTUserClaimsPrincipalFactory(UserManager<BTUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BTUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            //A claim is metadata and provides more information about our existing data. Kind of like a cookie in a browser. Stored in assembly per user. Wiped away when user is logged out
            //claims need to be stored as strings since strings are the easiest type of data to store and manipulate
            identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));

            return identity;
        }
    }
}