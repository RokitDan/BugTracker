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


        public async Task AddProjectAsync(Project project)
        {
            try
            {
                await _context.AddAsync(project);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task ArchiveProjectAsync(int projectId)
        {
            try
            {
                Project project = await GetProjectByIdAsync(projectId);

                if (project != null)
                {
                    project!.Archived = true;

                    foreach (Ticket ticket in project.Tickets)
                    {
                        ticket.ArchivedByProject = true;
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Project> allProjects = await _context.Projects.Include(p => p.Company)
                                              .Include(p => p.ProjectPriority)
                                              .Where(p => p.CompanyId == companyId)
                                              .ToListAsync();

                return allProjects;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                return await _context.Projects.Where(p => p.Archived == true && p.CompanyId == companyId)
                                              .Include(p => p.Company)
                                              .Include(p => p.ProjectPriority)
                                              .ToListAsync();

            }
            catch
            {
                throw new NotImplementedException();
            }
        }



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
                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(int projectId)
        {

            try
            {
                Project? project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.Tickets)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == projectId);

                return project!;
            }
            catch
            {
                throw;

            }
        }

        public async Task RestoreProjectAsync(int projectId)
        {
            try
            {
                Project project = await GetProjectByIdAsync(projectId);

                if (project != null)
                {
                    project!.Archived = false;

                    foreach (Ticket ticket in project.Tickets)
                    {
                        ticket.ArchivedByProject = true;
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();

            }
            catch
            {
                throw;
            }
        }
    }
}
