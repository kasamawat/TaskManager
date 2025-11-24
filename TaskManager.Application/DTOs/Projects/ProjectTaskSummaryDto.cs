using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Projects
{
    public class ProjectTaskSummaryDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        public int TotalTasks { get; set; }
        public int TodoCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }
        public int OverdueCount { get; set; }
    }
}
