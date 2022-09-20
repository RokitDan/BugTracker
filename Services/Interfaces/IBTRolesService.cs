using BugTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Services.Interfaces
{
    public interface IBTRolesService
    {
        public Task<bool> AddUserToRoleAsync(BTUser user, string roleName);
        public Task<List<IdentityRole>> GetRolesAsync();
        public Task<IEnumerable<string>> GetUserRolesAsync(BTUser user);
        public Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId);
        public Task<bool> IsUserInRoleAsync(BTUser member, string roleName);
        public Task<bool> RemoveUserFromRole(BTUser user, string roleName);
        public Task<bool> RemoveUserFromRoles(BTUser user, IEnumerable<string> roleNames);


    }
}
