using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Member Comment")]
        [StringLength(2000)]
        public string? Comment { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        //foreign keys
        public int TicketId { get; set; }
        [Required]
        public string? UserId { get; set; }

        //nav properties
        [DisplayName("Ticket")]
        public virtual Ticket? Ticket { get; set; }

        [DisplayName("Team Member")]
        public virtual BTUser? User { get; set; }


    }
}
