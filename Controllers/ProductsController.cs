using InventoryService.Data;
using InventoryService.Models;
using InventoryService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers;

public class ProductsController : Controller
{
    private readonly InventoryDbContext _context;

    public ProductsController(InventoryDbContext context)
    {
        _context = context;
    }

    // This action serves the HTML page at the route /Products
    [HttpGet("/Products")]
    public async Task<IActionResult> Index(string searchString)
    {
        // Pass the current search string back to the view to keep it in the search box.
        ViewData["CurrentFilter"] = searchString;

        IQueryable<Product> productsQuery = _context.Products.Include(p => p.Shops);

        if (!String.IsNullOrEmpty(searchString))
        {
            // Filter products where the Name or Sku contains the search string.
            productsQuery = productsQuery.Where(p => p.Name.Contains(searchString) || p.Sku.Contains(searchString));
        }

        return View(await productsQuery.AsNoTracking().ToListAsync());
    }

    // GET: /Products/Create
    // This action serves the page with the form to create a new product.
    [HttpGet("/Products/Create")]
    public async Task<IActionResult> Create()
    {
        var allShops = await _context.Shops.AsNoTracking().ToListAsync();
        var viewModel = new ProductViewModel
        {
            AvailableShops = allShops.Select(s => new ShopAssignmentViewModel
            {
                ShopId = s.Id,
                ShopName = s.Name,
                IsAssigned = false
            }).ToList()
        };
        return View(viewModel);
    }

    // POST: /Products/Create
    [HttpPost("/Products/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel viewModel)
    {
        // We only need to validate the Product part of the ViewModel
        if (TryValidateModel(viewModel.Product))
        {
            var product = viewModel.Product;
            await UpdateProductShops(product, viewModel.AvailableShops);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // If the model state is invalid, we must repopulate the shops list before returning the viewModel.
        var allShops = await _context.Shops.AsNoTracking().ToListAsync();
        foreach (var shopVM in viewModel.AvailableShops)
        {
            shopVM.ShopName = allShops.FirstOrDefault(s => s.Id == shopVM.ShopId)?.Name ?? "[Shop Not Found]";
        }
        
        return View(viewModel);
    }

    // GET: /Products/Edit/{id}
    [HttpGet("/Products/Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .Include(p => p.Shops)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        var allShops = await _context.Shops.AsNoTracking().ToListAsync();
        var viewModel = new ProductViewModel
        {
            Product = product,
            AvailableShops = allShops.Select(s => new ShopAssignmentViewModel
            {
                ShopId = s.Id,
                ShopName = s.Name,
                IsAssigned = product.Shops.Any(ps => ps.Id == s.Id)
            }).ToList()
        };

        return View(viewModel);
    }

    // POST: /Products/Edit/{id}
    [HttpPost("/Products/Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
    {
        if (id != viewModel.Product.Id)
        {
            return NotFound();
        }

        var productToUpdate = await _context.Products
            .Include(p => p.Shops)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (productToUpdate == null) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                if (await TryUpdateModelAsync(productToUpdate, "Product", p => p.Name, p => p.Sku, p => p.Price, p => p.Quantity))
                {
                    await UpdateProductShops(productToUpdate, viewModel.AvailableShops);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(viewModel.Product.Id))
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
        
        // If the model state is invalid, we must repopulate the shops list before returning the viewModel.
        var allShops = await _context.Shops.AsNoTracking().ToListAsync();
        foreach (var shopVM in viewModel.AvailableShops)
        {
            shopVM.ShopName = allShops.FirstOrDefault(s => s.Id == shopVM.ShopId)?.Name ?? "[Shop Not Found]";
        }

        return View(viewModel);
    }

    // GET: /Products/Delete/{id}
    [HttpGet("/Products/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpGet("api/products")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Products.ToListAsync());
    }

    [HttpGet("api/products/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost("api/products")]
    public async Task<IActionResult> CreateApi(Product product)
    {
        // Note: This API endpoint does not handle shop assignments for simplicity.
        // The product.Shops collection would need to be populated with tracked entities if needed.
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("api/products/{id}")]
    public async Task<IActionResult> Update(int id, Product updatedProduct)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        product.Name = updatedProduct.Name;
        product.Sku = updatedProduct.Sku;
        product.Quantity = updatedProduct.Quantity;
        product.Price = updatedProduct.Price;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("api/products/{id}")]
    public async Task<IActionResult> DeleteApi(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: /Products/Delete/{id}
    [HttpPost("/Products/Delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    private async Task UpdateProductShops(Product productToUpdate, List<ShopAssignmentViewModel> assignedShops)
    {
        if (assignedShops == null)
        {
            productToUpdate.Shops.Clear();
            return;
        }

        var selectedShopIds = new HashSet<int>(assignedShops.Where(s => s.IsAssigned).Select(s => s.ShopId));
        
        // Remove shops that are no longer selected
        productToUpdate.Shops
            .Where(s => !selectedShopIds.Contains(s.Id))
            .ToList()
            .ForEach(s => productToUpdate.Shops.Remove(s));

        // Add new shops
        var existingShopIds = new HashSet<int>(productToUpdate.Shops.Select(s => s.Id));
        var newShopIds = selectedShopIds.Where(id => !existingShopIds.Contains(id));
        
        var shopsToAdd = await _context.Shops.Where(s => newShopIds.Contains(s.Id)).ToListAsync();
        
        foreach(var shop in shopsToAdd) productToUpdate.Shops.Add(shop);
    }
}