using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        //primary key
        public int Id { get; set; } //hidden in view

        //foreign key
        public int CompanyId { get; set; } //hidden in view

        //public BTUser? ProjectManager { get; set; }

        [Required]
        [DisplayName("Project Name")]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; } //hidden in view

        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
        public bool? HasPM { get; set; }

        //Propertires for storing image
        public byte[]? ImageData { get; set; }   //hidden in view
        public string? ImageType { get; set; }   //hidden in view

        //Property for passing file information from the form(html) to the post.
        //Also not saved in teh database via [NotMapped] attribute
        [DataType(DataType.Upload)]
        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        public bool Archived { get; set; } //hidden in view

        public int ProjectPriorityId { get; set; }

        // nav properties
        public virtual Company? Company { get; set; }
        public virtual ProjectPriority? ProjectPriority { get; set; }
        public virtual ICollection<BTUser>? Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}
