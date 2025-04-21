// ✅ TodoItem.cs (Güncel ve hatasız versiyon)
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        public string? UserId { get; set; } // Firebase UID frontend'den gelmiyor, backend ekleyecek
    }
}
