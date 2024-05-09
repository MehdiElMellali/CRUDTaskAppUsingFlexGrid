using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TaskApp.Data
{
    public class TasksContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public TasksContext(DbContextOptions<TasksContext> options) : base(options)
        {
        }
    }
    public class Task
    {
        [Required]
        public int TaskID { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TaskProgress { get; set; }

    }

}