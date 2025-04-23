using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
  public class InventoryItem
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Item name is required.")]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Warehouse ID is required.")]
    [ForeignKey("Warehouse")]
    public int WarehouseId { get; set; }


    public Warehouse? Warehouse { get; set; }

    public bool IsActive { get; set; } = true;
  }
}

