using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class ProjectMembersViewModel
    {
        public Project? Project { get; set; }
        public MultiSelectList? CompanyMembers { get; set; }
        public List<string>? SelectedMembers { get; set; }
    }
}