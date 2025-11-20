using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Tasks
{
    public class ProjectTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string? Detail { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; }
    }
}
