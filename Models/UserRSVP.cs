#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Components.Forms;

namespace WeddingPlanner.Models;

public class UserRSVP
{
    [Key]
    public int UserRSVPId { get; set; }
     
    public int UserId { get; set; }
    public int WeddingId { get; set; }
       
    public User? Guest { get; set; }
    public Wedding? WeddingGuest { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
