using HNSDotNetTraining.Database2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HNSDotNetTraining.MiniPosMvc.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _db;

    public ProductController(AppDbContext db)
    {
        _db = db;
    }

    // GET: Product
    public async Task<IActionResult> Index()
    {
        var products = await _db.TblProducts.AsNoTracking().ToListAsync();
        return View(products);
    }

    // GET: Product/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Product/Create

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblProduct product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        try
        {
            await _db.TblProducts.AddAsync(product);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product created successfully!";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                "",
                ex.InnerException?.Message ?? ex.Message
            );

            return View(product);
        }
    }


    // GET: Product/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblProducts.FindAsync(id);
        if (item == null)
        {
            TempData["ErrorMessage"] = "Product not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(item);
    }

    // POST: Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TblProduct product)
    {
        if (id != product.ProductId)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingItem = await _db.TblProducts.FindAsync(id);
                if (existingItem == null)
                {
                    TempData["ErrorMessage"] = "Product not found!";
                    return RedirectToAction(nameof(Index));
                }

                existingItem.ProductName = product.ProductName;
                existingItem.Price = product.Price;
                existingItem.Quantity = product.Quantity;

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.TblProducts.AnyAsync(e => e.ProductId == id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        return View(product);
    }

    // GET: Product/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.TblProducts.AsNoTracking().FirstOrDefaultAsync(m => m.ProductId == id);
        if (item == null)
        {
            TempData["ErrorMessage"] = "Product not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(item);
    }

    // GET: Product/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.TblProducts.AsNoTracking().FirstOrDefaultAsync(m => m.ProductId == id);
        if (item == null)
        {
            TempData["ErrorMessage"] = "Product not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(item);
    }

    // POST: Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.TblProducts.FindAsync(id);
        if (item != null)
        {
            _db.TblProducts.Remove(item);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Product not found!";
        }

        return RedirectToAction(nameof(Index));
    }
}

