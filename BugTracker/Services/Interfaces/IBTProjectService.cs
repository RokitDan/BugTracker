using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTProjectService
    {
        //public Task GetCompanyIdAsync(BTUser user);
        public Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId);
        public Task<List<Project>> GetCurrentProjectsByCompanyIdAsync(int companyId);
        public Task<List<Project>> GetArchivedProjectsByCompanyIdAsync(int companyId);
        public Task AddProjectAsync(Project project);
        public Task<Project> GetProjectByIdAsync(int? ProjectId);
        public Task UpdateProjectAsync(Project project);
        public Task ArchiveProjectAsync(int projectId);
        public Task RestoreProjectAsync(int projectId);

    }
}
