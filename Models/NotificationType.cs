using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class NotificationType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
