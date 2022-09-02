using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTRolesService
    {
        public Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId);
    }
}
