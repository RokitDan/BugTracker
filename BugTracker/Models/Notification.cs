using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Notification
    {
        [Required]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? NotificationTypeId { get; set; }
        public bool? HasBeenViewed { get; set; }

        //foreign keys
        public int TicketId { get; set; }
        public int ProjectedId { get; set; }
        [Required]
        public string? SenderId  { get; set; }
        [Required]
        public string? RecipientId { get; set; }

        //nav properties
        public virtual NotificationType? NotificationType { get; set; }
        public virtual Ticket? Ticket { get; set; }
        public virtual Project? Project { get; set; }
        public virtual BTUser? Sender { get; set; }
        public virtual Recipient? Recipient { get; set; }









    }
}
