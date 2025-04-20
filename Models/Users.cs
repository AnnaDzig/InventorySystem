using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
  public class User
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [StringLength(100)]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
    [StringLength(50)]
    public string? Role { get; set; }
  }
}
