using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        //primary key
        public int Id { get; set; }

        //foreign key
        public int CompanyId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }

        //Propertires for storing image
        public byte[]? ImageData { get; set; } //= Array.Empty<byte>();
        public string? ImageType { get; set; } //= "";

        //Property for passing file information from the form(html) to the post.
        //Also not saved in teh database via [NotMapped] attribute
        [DataType(DataType.Upload)]
        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        public bool Archived { get; set; }

        public int ProjectPriorityId { get; set; }

        // nav properties
        public virtual Company? Company { get; set; }
        public virtual ProjectPriority? ProjectPriority { get; set; }
        public virtual ICollection<BTUser>? Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}
