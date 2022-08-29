using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Company
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [NotMapped]
        public IFormFile? ImageFromFile { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageFileTyle { get; set; }

        public ICollection<Project> Projects = new HashSet<Project>();
        public virtual ICollection<BTUser>? Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();

    }
}
