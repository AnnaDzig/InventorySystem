using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.ViewModels
{
    public class InventoryFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public int? WarehouseId { get; set; }
        public string? SortOrder { get; set; }

        public List<InventoryItem>? Items { get; set; }
        public SelectList? Warehouses { get; set; }
    }
}

