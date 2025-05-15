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
        // Set UserId before validation
        item.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (ModelState.IsValid)
        {
            item.IsCompleted = false;
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // If invalid, reload the view with errors
        var items = await _context.TodoItems
            .Where(t => t.UserId == item.UserId)
            .ToListAsync();
        return View("Index", items);
    }

    // Update existing to-do
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(TodoItem item)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingItem = await _context.TodoItems.FindAsync(item.Id);

        if (existingItem == null || existingItem.UserId != userId)
            return NotFound();

        existingItem.Title = item.Title;
        existingItem.Description = item.Description;
        existingItem.Deadline = item.Deadline;
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
