using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugTracker.Extensions;



namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        //private readonly IImageService _imageService;
        //private readonly UserManager<BTUser> _userManager;
        //private readonly IBTTicketService _ticketService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
            //_imageService = imageService;
            //_userManager = userManager;
            //_ticketService = ticketService;
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

        public async Task<List<Project>> GetArchivedProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                return await _context.Projects.Where(p => p.Archived == true && p.CompanyId == companyId)
                                              .Include(p => p.Company)
                                              .Include(p => p.ProjectPriority)
                                              .Include(p => p.Members)
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
                                              .Include(p => p.Members)
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
                .Include(p => p.Members)
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

        public async Task<BTUser>? GetProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                foreach (BTUser member in project.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        return member;
                    }
                }

                return null!;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                BTUser? currentPM = await GetProjectManagerAsync(projectId)!;
                BTUser? selectedPM = await _context.Users.FindAsync(userId);

                //Remove Current PM
                if (currentPM != null)
                {
                    await RemoveProjectManagerAsync(projectId);
                }

                //Add new PM
                try
                {
                    Project? project = await GetProjectByIdAsync(projectId);
                    await AddUserToProjectAsync(selectedPM!, projectId);

                    if (await _rolesService.IsUserInRoleAsync(selectedPM, nameof(BTRoles.ProjectManager)))
                    {
                        //project.ProjectManager = selectedPM;
                        selectedPM.IsProjectManager = true;
                        project.HasPM = true;
                        await _context.SaveChangesAsync();

                    }

                    return true;

                }
                catch
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                foreach (BTUser member in project.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        // Remove BTUser from Project
                        await UserRemovedFromProjectAsync(member, projectId);
                        project.HasPM = false;
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UserRemovedFromProjectAsync(BTUser user, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                bool onProject = project.Members.Any(m => m.Id == user.Id);

                //Check if BTUser in on project
                if (onProject)
                {
                    project.Members.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddUserToProjectAsync(BTUser user, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                bool onProject = project.Members.Any(m => m.Id == user.Id);

                //Check if BTUser in on project
                if (!onProject)
                {
                    project.Members.Add(user);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                if (project != null)
                {
                    //Cehck to see if the user is in a Project Member
                    return project.Members.Any(m => m.Id == userId);
                }

                return false;

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

        public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        {
            try
            {
                // get all projects
                // find out if any members are Project Managers
                // if no members are project managers, add that project to a list
                List<Project> allProjects = await GetCurrentProjectsByCompanyIdAsync(companyId);

                List<Project> unassignedProjects = new();

                foreach (Project project in allProjects)
                {
                    if ((await GetProjectManagerAsync(project.Id)) == null)
                    {
                        unassignedProjects.Add(project);
                    }
                }

                return unassignedProjects;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RemoveUsersButNotPMAsync(int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                foreach (BTUser user in project.Members!)
                {
                    if (!await _rolesService.IsUserInRoleAsync(user, nameof(BTRoles.ProjectManager)))
                    {
                        project.Members.Remove(user);
                    }
                }

                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetDevsAndSubsAsync(int id, int companyId)
        {
            try
            {
                List<BTUser> devsAndSubs = new();
                List<BTUser> companyMembers = _context.Users.Where(u => u.CompanyId == companyId).ToList();

                foreach (var user in companyMembers)
                {
                    if (await _rolesService.IsUserInRoleAsync(user, "Developer") || (await _rolesService.IsUserInRoleAsync(user, "Submitter")))
                    {
                        devsAndSubs.Add(user);
                    }

                }
                return devsAndSubs;
            }
            catch
            {
                throw;
            }
        }
    }
}
