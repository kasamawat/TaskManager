using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Tasks
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; } = "";
        public string? Detail { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
