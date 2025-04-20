using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Warehouse name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Location { get; set; }

        // Навігаційна властивість для зв'язку з InventoryItem
        public List<InventoryItem>? InventoryItems { get; set; }
    }
}
