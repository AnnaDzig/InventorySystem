using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Inventory Item ID is required.")]
        [ForeignKey("InventoryItem")]
        public int InventoryItemId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Take Date is required.")]
        public DateTime TakeDate { get; set; }

        public DateTime? ReturnDate { get; set; } // nullable бо можуть не повернути одразу

        [StringLength(255)]
        public string? Condition { get; set; } // стан при поверненні

        public string? Message { get; set; } // повідомлення / коментар користувача

        // Навігаційні властивості
        public InventoryItem? InventoryItem { get; set; }
        public User? User { get; set; }
    }
}
