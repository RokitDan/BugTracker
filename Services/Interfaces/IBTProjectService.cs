using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTProjectService
    {
        public Task AddProjectAsync(Project project);

        public Task<bool> AddProjectManagerAsync(string userId, Project project);

        public Task ArchiveProjectAsync(int projectId);

        Task<bool> AddUserToProjectAsync(BTUser user, int projectId);

        public Task<bool> AddUserToProjectAsync(BTUser user, Project project);

        public Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId);

        public Task<List<Project>> GetArchivedProjectsByCompanyIdAsync(int companyId);

        public Task<List<Project>> GetCurrentProjectsByCompanyIdAsync(int companyId);

        public Task<List<BTUser>> GetDevsAndSubsAsync(int id, int companyId);

        public Task<List<BTUser>> GetDevsAsync(int companyId);

        public Task<Project> GetProjectByIdAsync(int projectId);
        public Task<List<Project>> GetUserProjectsAsync(string userId);

        public Task<BTUser>? GetProjectManagerAsync(int projectId);

        public Task<List<Project>> GetUnassignedProjectsAsync(int companyId);

        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        public Task RemoveProjectManagerAsync(int projectId);

        public Task RestoreProjectAsync(int projectId);

        public Task UpdateProjectAsync(Project project);

        public Task<bool> UserRemovedFromProjectAsync(BTUser user, int projectId);

        public Task<bool> RemoveUsersButNotPMAsync(int projectId);

        public Task<List<BTUser>> GetProjectMembersbyRoleAsync(int projectId, string roleName);

        Task<List<BTUser>> GetProjectMembersbyRoleAsync(ICollection<BTUser> memberList, string roleName);
        public Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priority);
    }
}