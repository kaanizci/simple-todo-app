using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            var userId = HttpContext.User?.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("❌ Kullanıcı kimliği alınamadı.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized("Kullanıcı kimliği alınamadı.");

                return await _context.TodoItems
                    .Where(t => t.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Hata: " + ex.Message);
                return StatusCode(500, "Sunucuda bir hata oluştu.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var todoItem = await _context.TodoItems.FindAsync(id);
                if (todoItem == null || todoItem.UserId != userId) return NotFound();

                return todoItem;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Hata: " + ex.Message);
                return StatusCode(500, "Sunucuda bir hata oluştu.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem([FromBody] TodoItem todoItem)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    Console.WriteLine("❌ Kullanıcı kimliği alınamadı.");
                    return Unauthorized("Kullanıcı kimliği alınamadı.");
                }

                Console.WriteLine("✅ Kullanıcı UID: " + userId);

                todoItem.UserId = userId;
                _context.TodoItems.Add(todoItem);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Hata: " + ex.Message);
                return StatusCode(500, "Sunucuda bir hata oluştu.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, TodoItem updatedTodo)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var existingTodo = await _context.TodoItems.FindAsync(id);
                if (existingTodo == null || existingTodo.UserId != userId) return NotFound();

                existingTodo.Title = updatedTodo.Title;
                existingTodo.IsCompleted = updatedTodo.IsCompleted;
                existingTodo.DueDate = updatedTodo.DueDate;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Hata: " + ex.Message);
                return StatusCode(500, "Sunucuda bir hata oluştu.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var todoItem = await _context.TodoItems.FindAsync(id);
                if (todoItem == null || todoItem.UserId != userId) return NotFound();

                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Hata: " + ex.Message);
                return StatusCode(500, "Sunucuda bir hata oluştu.");
            }
        }
    }
}