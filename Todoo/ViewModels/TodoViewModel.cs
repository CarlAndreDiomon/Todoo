using System.ComponentModel.DataAnnotations;
using Todoo.Models;

namespace Todo_List_App.ViewModels
{
    public class TodoViewModel
    {
        [Required(ErrorMessage = "Task is required.")]
        public string Task { get; set; }
        public string FullName { get; set; }
        public List<TodoItem> Todos { get; set; } = new List<TodoItem>();
        
    }
}
