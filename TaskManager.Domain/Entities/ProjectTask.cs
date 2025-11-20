using TaskManager.Domain.Enums;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Domain.Entities
{
    public class ProjectTask
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; private set; }
        public string Detail { get; private set; }
        public DateTime? DueDate { get; private set; }
        public TaskStatus Status { get; private set; } = TaskStatus.Todo;

        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; }

        private ProjectTask() { }

        public ProjectTask(string title, string? detail, DateTime? dueDate, Guid projectId)
        {
            Title = title;
            Detail = detail;
            DueDate = dueDate;
            ProjectId = projectId;
        }

        public void Update(string title, string? detail, DateTime? dueDate)
        {
            Title = title;
            Detail = detail;
            DueDate = dueDate;
        }
        public void ChangeStatus(TaskStatus status)
        {
            Status = status;
        }
    }
}
