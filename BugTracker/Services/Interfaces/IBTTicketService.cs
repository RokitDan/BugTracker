namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        public Task<bool> AssignDeveloperAsync(string userId, int ticketId);
    }
}
