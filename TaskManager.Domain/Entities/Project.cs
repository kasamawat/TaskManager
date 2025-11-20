using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public Guid OwnerId { get; private set; }
        public User Owner { get; private set; }

        public ICollection<ProjectTask> Tasks { get; private set; } = [];

        private Project() { }

        public Project(string name, string? description, Guid ownerId)
        {
            Name = name;
            Description = description;
            OwnerId = ownerId;
        }
        public void Rename(string name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}
