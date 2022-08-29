﻿using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public string? PropertyName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        [Required]
        public string UserId { get; set; }

        //foreign keys
        public int? TicketId { get; set; }


        //nav properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }






    }
}