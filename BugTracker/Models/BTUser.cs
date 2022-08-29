
using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BugTracker.Models;
public class BTUser : IdentityUser
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    [NotMapped]
    public IFormFile? ImageFromFile { get; set; }
    public string? ImageFileName { get; set; }
    public string? ImageFileTyle { get; set; }

    //foreign key
    public int CompanyId { get; set; }


}
