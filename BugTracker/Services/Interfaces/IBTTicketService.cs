using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        public Task<bool> AssignDeveloperAsync(string userId, int ticketId);
        public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);
        public Task<Ticket> GetTicketByIdAsync(int ticketId);
      
    }
}
