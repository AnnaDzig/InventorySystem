using InventorySystem.Data;
using InventorySystem.Models;
using InventorySystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace InventorySystem.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, int? warehouseId, string? sortOrder)
        {
            var itemsQuery = _context.InventoryItems
                .Include(i => i.Warehouse)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                itemsQuery = itemsQuery.Where(i =>
                    i.Name.Contains(searchTerm) ||
                    (i.Description != null && i.Description.Contains(searchTerm)));
            }

            if (warehouseId.HasValue && warehouseId > 0)
            {
                itemsQuery = itemsQuery.Where(i => i.WarehouseId == warehouseId);
            }

            // Сортування
            ViewBag.NameSort = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.QuantitySort = sortOrder == "qty_asc" ? "qty_desc" : "qty_asc";

            itemsQuery = sortOrder switch
            {
                "name_desc" => itemsQuery.OrderByDescending(i => i.Name),
                "qty_asc" => itemsQuery.OrderBy(i => i.Quantity),
                "qty_desc" => itemsQuery.OrderByDescending(i => i.Quantity),
                _ => itemsQuery.OrderBy(i => i.Name),
            };

            var items = await itemsQuery.ToListAsync();
            var warehouses = await _context.Warehouses.ToListAsync();

            var vm = new InventoryFilterViewModel
            {
                Items = items,
                SearchTerm = searchTerm,
                WarehouseId = warehouseId,
                SortOrder = sortOrder,
                Warehouses = new SelectList(warehouses, "Id", "Name")
            };

            return View(vm);
        }
        // GET: /Inventory/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var warehouses = await _context.Warehouses.ToListAsync();
            ViewData["WarehouseList"] = new SelectList(warehouses, "Id", "Name");
            return View();
        }

        // POST: /Inventory/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                _context.InventoryItems.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var warehouses = await _context.Warehouses.ToListAsync();
            ViewData["WarehouseList"] = new SelectList(warehouses, "Id", "Name");
            return View(item);
        }

        // GET: /Inventory/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound();

            var warehouses = await _context.Warehouses.ToListAsync();
            ViewData["WarehouseList"] = new SelectList(warehouses, "Id", "Name", item.WarehouseId);

            return View(item);
        }

        // POST: /Inventory/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryItem item)
        {
            if (id != item.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.InventoryItems.Update(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.InventoryItems.Any(e => e.Id == item.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            var warehouses = await _context.Warehouses.ToListAsync();
            ViewData["WarehouseList"] = new SelectList(warehouses, "Id", "Name", item.WarehouseId);

            return View(item);
        }

        // GET: /Inventory/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.InventoryItems
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Inventory/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Inventory/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.InventoryItems
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
                return NotFound();

            return View(item);
        }


    }

}
