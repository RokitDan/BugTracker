using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Company
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }

      
        //Propertires for storing image
        public byte[]? ImageData { get; set; } //= Array.Empty<byte>();
        public string? ImageType { get; set; } //= "";

        //Property for passing file information from the form(html) to the post.
        //Also not saved in teh database via [NotMapped] attribute
        [DataType(DataType.Upload)]
        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        public virtual ICollection<Project>? Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<BTUser>? Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();

    }
}
