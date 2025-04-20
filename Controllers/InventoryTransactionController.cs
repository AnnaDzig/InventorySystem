using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventorySystem.Controllers
{
    [Authorize]
    public class InventoryTransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryTransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /InventoryTransaction/Index
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.InventoryTransactions
                .Include(t => t.InventoryItem)
                .Include(t => t.User)
                .OrderByDescending(t => t.TakeDate)
                .ToListAsync();

            return View(transactions);
        }

        // GET: /InventoryTransaction/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Items"] = new SelectList(await _context.InventoryItems.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: /InventoryTransaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int InventoryItemId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var transaction = new InventoryTransaction
            {
                InventoryItemId = InventoryItemId,
                UserId = userId,
                TakeDate = DateTime.Now
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /InventoryTransaction/Return/5
        public async Task<IActionResult> Return(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var transaction = await _context.InventoryTransactions
                .Include(t => t.InventoryItem)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); 

            if (transaction == null || transaction.ReturnDate != null)
                return NotFound();

            return View(transaction);
        }

        // POST: /InventoryTransaction/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, string Condition, string Message)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var transaction = await _context.InventoryTransactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); 

            if (transaction == null)
                return NotFound();

            transaction.ReturnDate = DateTime.Now;
            transaction.Condition = Condition;
            transaction.Message = Message;

            await _context.SaveChangesAsync();

            return RedirectToAction("MyHistory");
        }


        // GET: /InventoryTransaction/MyActive
        public async Task<IActionResult> MyActive(int? itemId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var query = _context.InventoryTransactions
                .Include(t => t.InventoryItem)
                .Where(t => t.UserId == userId && t.ReturnDate == null);

            if (itemId.HasValue)
            {
                query = query.Where(t => t.InventoryItemId == itemId.Value);
            }

            var transactions = await query.ToListAsync();
            return View(transactions);
        }

        // GET: /InventoryTransaction/MyHistory
        public async Task<IActionResult> MyHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var transactions = await _context.InventoryTransactions
                .Include(t => t.InventoryItem)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TakeDate)
                .ToListAsync();

            return View(transactions);
        }
    }
}

