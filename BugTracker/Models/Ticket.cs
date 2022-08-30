﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Xml.Linq;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Ticket Title")]
        public string? Title { get; set; }

        [Required]
        [DisplayName("Ticket Description")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        public bool Archived { get; set; }

        [DisplayName("Archived By Project")]
        public bool ArchivedByProject { get; set; }

        // foreign keys
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketStatusId { get; set; }
        public int TicketPriorityId { get; set; }
        public string? DeveloperUserId { get; set; }
        [Required]
        public string? SubmitterUserId { get; set; }


        //nav properties?
        public virtual Project? Project { get; set; }
        public virtual TicketPriority? TicketPriority { get; set; }
        public virtual TicketStatus? TicketStatus { get; set; }
        public virtual TicketType? TicketType { get; set; }
        public virtual BTUser? DeveloperUser { get; set; }
        public virtual BTUser? SubmitterUser { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<TicketComment>? Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment>? Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory>? History { get; set; } = new HashSet<TicketHistory>();
    }
}
