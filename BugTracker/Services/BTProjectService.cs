using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public BTProjectService(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public Task AddProjectAsync(Project project)
        {
            //try
            //{
            //    //return _context.Add(project);


            //}
            //catch
            //{
                throw new NotImplementedException();
            //}
        }

        public Task ArchiveProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                return await _context.Projects.Include(p => p.Company)
                                              .Include(p => p.ProjectPriority)
                                              .Where(p => p.CompanyId == companyId)
                                              .ToListAsync();
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                return await _context.Projects.Where(p => p.Archived == true)
                                              .Include(p => p.Company)
                                              .Include(p => p.ProjectPriority)
                                              .Where(p => p.CompanyId == companyId)
                                              .ToListAsync();
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        //public async Task GetCompanyIdAsync(BTUser user)
        //{
        //    try
        //    {
        //        return await _userManager.GetUserAsync(User)).CompanyId
        //    }
        //    catch
        //    {
        //    throw new NotImplementedException();
        //    }
        //}

        public async Task<List<Project>> GetCurrentProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                return await _context.Projects.Where(p => p.Archived == false)
                                              .Include(p => p.Company)
                                              .Include(p => p.ProjectPriority)
                                              .Where(p => p.CompanyId == companyId)
                                              .ToListAsync();
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public Task<Project> GetProjectByIdAsync(int? ProjectId)
        {
            try
            {
                return _context.Projects
                 .Include(p => p.Company)
                 .Include(p => p.ProjectPriority)
                 .FirstOrDefaultAsync(m => m.Id == ProjectId);
            }
            catch
            {
                throw new NotImplementedException();

            }
        }

        public Task RestoreProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProjectAsync(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
