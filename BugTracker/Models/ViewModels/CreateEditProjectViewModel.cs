namespace BugTracker.Models.ViewModels
{
    public class CreateEditProjectViewModel
    {
        public Project? Project { get; set; }
        public List<BTUser>? ProjectManagers { get; set; }
    }
}
