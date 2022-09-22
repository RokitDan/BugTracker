using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class CreateEditProjectViewModel
    {
        public Project? Project { get; set; }
        public SelectList? ProjectManagers { get; set; }
        public string? PMID { get; set; }
        public SelectList? ProjectPriorityId { get; set; }
    }
}