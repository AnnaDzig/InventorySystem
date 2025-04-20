using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
  public class PasswordResetToken
  {
    [Key]
    public int Id { get; set; }


    [Required, StringLength(100)]
    public string Email { get; set; }

    [Required, StringLength(200)]
    public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }
  }
}

