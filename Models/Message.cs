using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Transaction ID is required.")]
        [ForeignKey("InventoryTransaction")]
        public int InventoryTransactionId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Message text is required.")]
        [StringLength(500)]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навігаційні властивості
        public InventoryTransaction? InventoryTransaction { get; set; }
        public User? User { get; set; }
    }
}

