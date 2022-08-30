
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BugTracker.Models;
public class BTUser : IdentityUser
{
    [Required]
    [DisplayName("First Name")]
    public string? FirstName { get; set; }
    [Required]
    [DisplayName("Last Name")]
    public string? LastName { get; set; }
    [NotMapped]
    [DisplayName("Full Name")]
    public string? FullName { get; set; }

    [DataType(DataType.Upload)]
    [NotMapped]
    public IFormFile? ImageFromFile { get; set; }
    public string? ImageFileName { get; set; }
    public string? ImageFileTyle { get; set; }

    //foreign key
    public int CompanyId { get; set; }

    //nav properties
    public virtual Company? Company { get; set; }
    public virtual ICollection<Project>? Projects { get; set; }


}
