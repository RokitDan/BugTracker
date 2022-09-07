using Microsoft.AspNetCore.Mvc.Rendering;


namespace BugTracker.Models.ViewModels
{
    public class ProjectMembersViewModel
    {
        public Project? Project { get; set; }
        public SelectList? Members { get; set; }
    }
}
