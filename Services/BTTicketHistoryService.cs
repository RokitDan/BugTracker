using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        #region Constructor

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion Constructor

        #region Add Ticket History (old Ticket, new Ticket, string userId)

        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            try
            {
                if (oldTicket == null && newTicket != null) // new ticket has been added
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = string.Empty,
                        OldValue = string.Empty,
                        CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                        UserId = userId,
                        Description = "New Ticket has been Added."
                    };

                    try
                    {
                        await _context.AddAsync(ticketHistory);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    if (oldTicket!.Title != newTicket!.Title)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Title",
                            OldValue = oldTicket.Title,
                            NewValue = newTicket.Title,
                            CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket's Title has been Updated."
                        };
                        await _context.AddAsync(ticketHistory);
                    }

                    if (oldTicket.Description != newTicket.Description)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Description",
                            OldValue = oldTicket.Description,
                            NewValue = newTicket.Description,
                            CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket's Description has been Updated."
                        };
                        await _context.AddAsync(ticketHistory);
                    }

                    if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketPriority",
                            OldValue = oldTicket.TicketPriority!.Name,
                            NewValue = newTicket.TicketPriority!.Name,
                            CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket's Priority has been Updated."
                        };
                        await _context.AddAsync(ticketHistory);
                    }

                    if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketStatus",
                            OldValue = oldTicket.TicketStatus!.Name,
                            NewValue = newTicket.TicketStatus!.Name,
                            CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket's Status has been Updated."
                        };
                        await _context.AddAsync(ticketHistory);
                    }

                    if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketType",
                            OldValue = oldTicket.TicketType!.Name,
                            NewValue = newTicket.TicketType!.Name,
                            CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket Type has been Updated."
                        };
                        await _context.AddAsync(ticketHistory);
                    }

                    if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "DeveloperUser",
                            OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                            NewValue = newTicket.DeveloperUser?.FullName ?? "Not Assigned",
                            CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket's Assigned Developer has been Updated."
                        };
                        await _context.AddAsync(ticketHistory);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion Add Ticket History (old Ticket, new Ticket, string userId)

        #region Add Ticket history (int ticketId, string model, string userId)

        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets.FindAsync(ticketId);

                string description = model.ToLower().Replace("ticket", "");
                description = $"{ticket!.Title}: New {description} added. ";

                TicketHistory ticketHistory = new()
                {
                    TicketId = ticket.Id,
                    PropertyName = model,
                    OldValue = string.Empty,
                    NewValue = string.Empty,
                    CreatedDate = DataUtility.GetPostGresDate(DateTime.Now),
                    UserId = userId,
                    Description = description
                };
                try
                {
                    await _context.AddAsync(ticketHistory);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion Add Ticket history (int ticketId, string model, string userId)

        #region Get company Ticket history (int projectId, int companyId)

        public Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        #endregion Get company Ticket history (int projectId, int companyId)

        #region Get Project Ticket History (int projectId, int companyId)

        public Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        #endregion Get Project Ticket History (int projectId, int companyId)
    }
}