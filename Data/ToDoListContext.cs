using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ToDoListContext : DbContext
    {
        public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options)
        {
        }
        public DbSet<ToDoTask> ToDoTasks { get; set; }

    }
}