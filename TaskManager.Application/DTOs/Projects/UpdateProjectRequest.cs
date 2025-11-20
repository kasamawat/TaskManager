using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Projects
{
    public class UpdateProjectRequest
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
