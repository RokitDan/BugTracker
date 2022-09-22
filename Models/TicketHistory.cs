using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public string? PropertyName { get; set; }
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Created")]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Previous Value")]
        public string? OldValue { get; set; }

        [DisplayName("Current Value")]
        public string? NewValue { get; set; }

        //foreign keys
        public int? TicketId { get; set; }

        [Required]
        public string? UserId { get; set; }

        //nav properties
        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? User { get; set; }
    }
}