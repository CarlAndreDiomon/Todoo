using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Todoo.Data;
using Todoo.Models;

[Authorize]
public class TodoController : Controller
{
    private readonly ApplicationDbContext _context;

    public TodoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Display the list
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var items = await _context.TodoItems
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return View(items);
    }

    // Add a new to-do
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(TodoItem item)
    {
        if (ModelState.IsValid)
        {
            item.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            item.IsCompleted = false; // Default to not completed
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // Update existing to-do
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(TodoItem item)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingItem = await _context.TodoItems.FindAsync(item.Id);

        if (existingItem == null || existingItem.UserId != userId)
            return NotFound();

        existingItem.Title = item.Title;
        existingItem.Description = item.Description;
        existingItem.IsCompleted = item.IsCompleted;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Delete a to-do
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var item = await _context.TodoItems.FindAsync(id);

        if (item == null || item.UserId != userId)
            return NotFound();

        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
