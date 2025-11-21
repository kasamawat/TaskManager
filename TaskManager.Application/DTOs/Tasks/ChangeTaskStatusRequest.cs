using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.DTOs.Tasks
{
    public class ChangeTaskStatusRequest
    {
        public TaskStatus Status { get; set; }
    }
}
