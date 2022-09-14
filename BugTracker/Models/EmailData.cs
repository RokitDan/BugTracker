using System.ComponentModel.DataAnnotations;
using BugTracker.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class EmailData
    {
        [Required]
        public string? BlogUserId { get; set; } //do they have an account?

        [Required]
        public string? UserEmail { get; set; } //account user's email OR anon email entered by anon user

        public string? AdminEmailAddress { get; set; } //admin email address

        public string? EmailSubject { get; set; } //Subject line

        [Required]
        public string? EmailMessage { get; set; } //Body of the email





    }
}
