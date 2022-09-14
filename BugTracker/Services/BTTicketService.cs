using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        //public async Task<List<BTUser>> GetDevs (int companyId)
        //{
        //    List<BTUser> devs = new();
        //    List<BTUser> companyMembers = _context.Users.Where(u => u.CompanyId == companyId).ToList();

        //    foreach (var user in companyMembers)
        //    {
        //        if (await _rolesService.IsUserInRoleAsync(user, "Developer")))
        //        {
        //            devs.Add(user);
        //        }

        //    }
        //    return devs;
        //}

        //public async Task<bool> AssignDeveloperAsync(string userId, int ticketId)
        //{
        //    await GetDevs(companyId)// get user with role of developer
        //    // get the user's id
        //    // assign the user's id as the ticket's DeveloperUserId property
        //}



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

                return ticket;
            }
            catch
            {
                throw;
            }
        }

        public Task AddNewTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task AddTicketcommentasync(TicketComment ticketComment)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
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

        public async Task<List<Ticket>> GetAllTicketsByCompanyIdAsync(int companyId)
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



        public Task RestoreTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
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
    }
}

