using BugTracker.Models;
using BugTracker.Data;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        private readonly ApplicationDbContext _context;


        public BTRolesService(RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            try
            {
                bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
                return result;

            }
            catch
            {
                throw;
            }
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> roles = await _context.Roles.ToListAsync();
                return roles;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            try
            {
                //The roles need cinnamon butter, but no cinnamon butter exists in the database :(
                IEnumerable<string> texasRoadHouse = await _userManager.GetRolesAsync(user);
                return texasRoadHouse;

            }
            catch
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            try
            {
                List<BTUser> btUsers = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
                List<BTUser> userResults = btUsers.Where(b => b.CompanyId == companyId).ToList();

                return userResults;
            }
            catch
            {
                throw;
            }
        }




        public async Task<bool> IsUserInRoleAsync(BTUser member, string roleName)
        {
            try
            {
                bool result = await _userManager.IsInRoleAsync(member, roleName);

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> RemoveUserFromRole(BTUser user, string roleName)
        {
            try
            {
                bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
                return result;

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RemoveUserFromRoles(BTUser user, IEnumerable<string> roleNames)
        {
            try
            {
                bool result = (await _userManager.RemoveFromRolesAsync(user, roleNames)).Succeeded;
                return result;

            }
            catch
            {
                throw;
            }
        }
    }
}
