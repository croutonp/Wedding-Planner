#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;
public class Login
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string LoginEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string LoginPassword { get; set; }
}