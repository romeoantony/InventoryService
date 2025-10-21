
using InventoryService.Data;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers;

public class ShopsController : Controller
{
    private readonly InventoryDbContext _context;

    public ShopsController(InventoryDbContext context)
    {
        _context = context;
    }

    // GET: Shops
    public async Task<IActionResult> Index(string searchString)
    {
        ViewData["CurrentFilter"] = searchString;

        var shopsQuery = from s in _context.Shops
                         select s;

        if (!string.IsNullOrEmpty(searchString))
        {
            shopsQuery = shopsQuery.Where(s => s.Name.Contains(searchString) 
                                            || s.Address.Contains(searchString));
        }

        return View(await shopsQuery.AsNoTracking().ToListAsync());
    }

    // GET: Shops/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Shops/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Address")] Shop shop)
    {
        if (ModelState.IsValid)
        {
            _context.Add(shop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(shop);
    }

    // GET: Shops/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var shop = await _context.Shops.FindAsync(id);
        if (shop == null)
        {
            return NotFound();
        }
        return View(shop);
    }

    // POST: Shops/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Shop shop)
    {
        if (id != shop.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(shop);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopExists(shop.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(shop);
    }

    // GET: Shops/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var shop = await _context.Shops
            .FirstOrDefaultAsync(m => m.Id == id);
        if (shop == null)
        {
            return NotFound();
        }

        return View(shop);
    }

    // POST: Shops/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var shop = await _context.Shops.FindAsync(id);
        if (shop != null)
        {
            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ShopExists(int id)
    {
        return _context.Shops.Any(e => e.Id == id);
    }
}