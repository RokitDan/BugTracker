using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public IFormFile? ImageFromFile { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageFileTyle { get; set; }

        //foreign keys
        public int TicketId { get; set; }
        [Required]
        public string UserId { get; set; }

        //navigation properties
        public virtual Ticket? Ticket {get; set; }
        public virtual BTUser? User { get; set; }








    }
}
