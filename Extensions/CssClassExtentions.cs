using BugTracker.Models;
using BugTracker.Models.Enums;

namespace BugTracker.Extensions
{
    public static class CssClassExtentions
    {
        public static string GetSectionPriority(this TicketPriority? priority)
        {
            if (priority is null)
            {
                return "card-info";
            }

            return priority.Name switch
            {
                nameof(BTTicketPriorities.Low) => "card-info",
                nameof(BTTicketPriorities.Medium) => "card-warning",
                nameof(BTTicketPriorities.High) => "card-danger",
                nameof(BTTicketPriorities.Urgent) => "card-urgent",
                _ => string.Empty
            };
        }
    }
}