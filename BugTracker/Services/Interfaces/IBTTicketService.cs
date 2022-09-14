using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        public Task AddNewTicketAsync(Ticket ticket);

        public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);
        public Task AddTicketcommentasync(TicketComment ticketComment);
        public Task ArchiveTicketAsync(Ticket ticket);
        public Task AssignTicketAsync(int ticketId, string userId);
        public Task<List<Ticket>> GetAllTicketsByCompanyIdAsync(int companyId);
        public Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId);
        public Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId, int companyId);
        public Task<Ticket> GetTicketByIdAsync(int ticketId);
        public Task RestoreTicketAsync(Ticket ticket);
        //public Task<bool> AssignDeveloperAsync(string userId, int ticketId);
        
        public Task UpdateTicketAsync(Ticket ticket);

    }
}
