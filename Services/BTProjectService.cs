using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                                              .Include(p => p.Tickets)
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
                .ThenInclude(t => t.TicketPriority)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.DeveloperUser)
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
                        ticket.ArchivedByProject = false;
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

        public async Task<bool> AddProjectManagerAsync(string userId, Project project)
        {
            try
            {
                BTUser? currentPM = await GetProjectManagerAsync(project.Id)!;
                BTUser? selectedPM = await _context.Users.FindAsync(userId);

                //Remove Current PM
                if (currentPM != null)
                {
                    await RemoveProjectManagerAsync(project.Id);
                }

                //Add new PM
                try
                {
                    await AddUserToProjectAsync(selectedPM!, project);

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
            Project project = await GetProjectByIdAsync(projectId);
            return await AddUserToProjectAsync(user, project);
        }

        public async Task<bool> AddUserToProjectAsync(BTUser user, Project project)
        {
            try
            {
                //Check if BTUser in on project
                if (!project.Members.Any(x => x.Id == user.Id))
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

        public async Task<List<BTUser>> GetDevsAsync(int companyId)
        {
            try
            {
                List<BTUser> devs = new();
                List<BTUser> companyMembers = _context.Users.Where(u => u.CompanyId == companyId).ToList();

                foreach (var user in companyMembers)
                {
                    if (await _rolesService.IsUserInRoleAsync(user, "Developer"))
                    {
                        devs.Add(user);
                    }
                }
                return devs;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetProjectMembersbyRoleAsync(int projectId, string roleName)
        {
            try
            {
                Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                List<BTUser> members = new();

                return await GetProjectMembersbyRoleAsync(project.Members, roleName);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetProjectMembersbyRoleAsync(ICollection<BTUser> memberList, string roleName)
        {
            List<BTUser> userList = new();

            foreach (BTUser member in memberList)
            {
                if (await _rolesService.IsUserInRoleAsync(member, roleName))
                {
                    userList.Add(member);
                }
            }

            return userList;
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project>? projects = (await _context.Users
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(p => p.Company)
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(p => p.Members)
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(p => p.Tickets)
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(t => t.Tickets)
                                                                .ThenInclude(t => t.DeveloperUser)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)
                                                                 .ThenInclude(t => t.SubmitterUser)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)
                                                                 .ThenInclude(t => t.TicketPriority)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)
                                                                 .ThenInclude(t => t.TicketStatus)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)
                                                                 .ThenInclude(t => t.TicketType)
                                                         .FirstOrDefaultAsync(u => u.Id == userId))?.Projects!.ToList();
                return projects!;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priority)
        {
            try
            {
                List<Project> projects = await _context.Projects.Include(p => p.Company)
                                                    .Include(p => p.ProjectPriority)
                                                    .Where(p => p.CompanyId == companyId)
                                                    .Where(p => p.ProjectPriority!.Name == priority)
                                                    .ToListAsync();
                return projects!;
            }
            catch
            {
                throw;
            }
        }
    }
}