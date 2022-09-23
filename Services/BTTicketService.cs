using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
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
                                        .ThenInclude(c => c.User)
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


        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BTUser? btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<Ticket>? tickets = new();
            try
            {
                if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Admin)))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(p => p.Tickets!).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Developer)))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(p => p.Tickets!)
                                                    .Where(t => t.DeveloperUserId == userId || t.SubmitterUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Submitter)))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(t => t.Tickets!).Where(t => t.SubmitterUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.ProjectManager)))
                {
                    List<Ticket>? projectTickets = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(t => t.Tickets!).ToList();
                    List<Ticket>? submittedTickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(p => p.Tickets!).Where(t => t.SubmitterUserId == userId).ToList();
                    tickets = projectTickets.Concat(submittedTickets).ToList();
                }
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }





    }


}