using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets
                                        .Include(t => t.DeveloperUser)
                                        .Include(t => t.Project)
                                        .Include(t => t.Comments)
                                        .Include(t => t.SubmitterUser)
                                        .Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.Attachments)
                                        .Include(t => t.History)
                                        .Include(t => t.TicketType)
                                        .FirstOrDefaultAsync(m => m.Id == ticketId);

                return ticket!;
            }
            catch
            {
                throw;
            }
        }

        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                await _context.AddAsync(ticket);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task AddTicketCommentAsync(TicketComment ticketComment)
        {
            try
            {
                _context.Add(ticketComment);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public ICollection<TicketComment> GetCommentsByTicketId(int ticketId)
        {
            ICollection<TicketComment> comments = _context.TicketComments.Where(c => c.TicketId == ticketId).ToList();
            return comments;
        }

        public async Task ArchiveTicketAsync(int ticketId)
        {
            try
            {
                Ticket ticket = await GetTicketByIdAsync(ticketId);
                ticket.Archived = false;
            }
            catch
            {
                throw;
            }
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            try
            {
                Ticket ticket = await GetTicketByIdAsync(ticketId);
                ticket.DeveloperUserId = userId;

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetCurrentTicketsByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects
                                        .Where(p => p.CompanyId == companyId && !p.Archived)
                                        .SelectMany(p => p.Tickets)
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.DeveloperUser)
                                            .Include(t => t.History)
                                            .Include(t => t.Project)
                                            .Include(t => t.SubmitterUser)
                                            .Include(t => t.TicketPriority)
                                            .Include(t => t.TicketStatus)
                                            .Include(t => t.TicketType)
                                            .Where(t => !t.Archived)
                                            .ToListAsync();

                return tickets;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects
                                        .Where(p => p.CompanyId == companyId)
                                        .SelectMany(p => p.Tickets)
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.DeveloperUser)
                                            .Include(t => t.History)
                                            .Include(t => t.Project)
                                            .Include(t => t.SubmitterUser)
                                            .Include(t => t.TicketPriority)
                                            .Include(t => t.TicketStatus)
                                            .Include(t => t.TicketType)
                                            .ToListAsync();

                return tickets;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects
                                        .Where(p => p.CompanyId == companyId)
                                        .SelectMany(p => p.Tickets)
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.DeveloperUser)
                                            .Include(t => t.History)
                                            .Include(t => t.Project)
                                            .Include(t => t.SubmitterUser)
                                            .Include(t => t.TicketPriority)
                                            .Include(t => t.TicketStatus)
                                            .Include(t => t.TicketType)
                                            .Where(t => t.Archived || t.ArchivedByProject)
                                            .ToListAsync();

                return tickets;
            }
            catch
            {
                throw;
            }
        }

        public async Task RestoreTicketAsync(int ticketId)
        {
            try
            {
                Ticket ticket = await GetTicketByIdAsync(ticketId);
                ticket.Archived = false;
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId, int companyId)
        {
            try
            {
                Ticket? ticket = await _context.Projects
                                        .Where(p => p.CompanyId == companyId)
                                        .SelectMany(p => p.Tickets)
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.DeveloperUser)
                                            .Include(t => t.SubmitterUser)
                                            .Include(t => t.TicketPriority)
                                            .Include(t => t.TicketStatus)
                                            .Include(t => t.History)
                                            .Include(t => t.TicketType)
                                            .Include(t => t.Project)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(t => t.Id == ticketId);

                return ticket!;
            }
            catch
            {
                throw;
            }
        }

        public Task ArchiveTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task RestoreTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }
    }
}