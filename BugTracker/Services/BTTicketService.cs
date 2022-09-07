using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        public Task<bool> AssignDeveloperAsync(string userId, int ticketId)
        {
            throw new NotImplementedException();
        }

        //public async Task<bool> AssignDeveloperAsync(string userId, int ticketId)
        //{
        //    try
        //    {

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
    }
}
