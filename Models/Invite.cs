﻿using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public DateTime InviteDate { get; set; }
        public DateTime? JoinDate { get; set; }
        public Guid CompanyToken { get; set; }
        public string? Message { get; set; }
        public bool IsValid { get; set; }

        [Required]
        public string? InviteeEmail { get; set; }

        [Required]
        public string? InviteeFirstName { get; set; }

        [Required]
        public string? InviteeLastName { get; set; }

        //foreign keys
        public int CompanyId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public string? InvitorId { get; set; }

        public string? InviteeId { get; set; }

        //nav properties
        public virtual Company? Company { get; set; }

        public virtual Project? Project { get; set; }
        public virtual BTUser? Invitor { get; set; }

        public virtual BTUser? Invitee { get; set; }
    }
}