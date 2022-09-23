using BugTracker.Data;
using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<BTUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBTTicketService _ticketService;
        private readonly IImageService _imageService;
        private readonly IBTTicketHistoryService _ticketHistoryService;
        private readonly IBTProjectService _projectService;
        private readonly IBTNotificationService _notificationService;

        public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTNotificationService notificationService, IBTTicketService ticketService, IBTProjectService projectService, IImageService imageService, IBTTicketHistoryService ticketHistoryService)
        {
            _context = context;
            _userManager = userManager;
            _ticketService = ticketService;
            _imageService = imageService;
            _ticketHistoryService = ticketHistoryService;
            _projectService = projectService;
            _notificationService = notificationService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity!.GetCompanyId();
            //  int projectId = (await _context.Projects.Id)
            var tickets = await _ticketService.GetCurrentTicketsByCompanyIdAsync(companyId);
            //.Where(t => t.Project.Id == projectId);

            return View(tickets.OrderByDescending(x => x.TicketPriority?.Id).ToList());
        }

        // GET: Tickets
        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity!.GetCompanyId();
            //  int projectId = (await _context.Projects.Id)
            var tickets = await _ticketService.GetAllTicketsByCompanyIdAsync(companyId);
            //.Where(t => t.Project.Id == projectId);

            return View(tickets.OrderByDescending(x => x.TicketPriority?.Id).ToList());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0 || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,DeveloperUserId")] Ticket ticket)
        {
            ModelState.Remove("SubmitterUserId");

            if (ModelState.IsValid)
            {
                ticket.SubmitterUserId = _userManager.GetUserId(User);
                int statusId = (await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == nameof(BTTicketStatuses.New)))!.Id;

                ticket.TicketStatusId = statusId;

                ticket.CreatedDate = DataUtility.GetPostGresDate(DateTime.Now);

                // all tickets need this if they are going to be in the database
                await _ticketService.AddNewTicketAsync(ticket);

                int companyId = User.Identity!.GetCompanyId();
                string userId = _userManager.GetUserId(User);
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id, companyId);
                await _ticketHistoryService.AddHistoryAsync(null!, newTicket, userId);
                BTUser user = await _userManager.GetUserAsync(User);
                BTUser? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId)!;

                Notification notification = new()
                {
                    NotificationTypeId = (await _context.NotificationTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationType.Ticket))).Id,
                    TicketId = ticket.Id,
                    Title = "New Ticket Added",
                    Message = $"New Ticket: {ticket.Title} was created by {user.FullName}",
                    CreatedDate = DataUtility.GetPostGresDate(DateTime.UtcNow),
                    SenderId = userId,
                    RecipientId = projectManager?.Id,
                };

                await _notificationService.AddNotificationAsync(notification);
                if (projectManager != null)
                {
                    await _notificationService.SendEmailNotificationAsync(notification, $"New Ticket Added for Project: {ticket.Project!.Name}");
                }
                else
                {
                    notification.RecipientId = userId;
                    await _notificationService.SendEmailNotificationAsync(notification, $"New Ticket Added for Project: {ticket.Project!.Name}");
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketStatusId);
            ViewData["DeveloperUserId"] = new SelectList(await _projectService.GetDevsAsync(companyId), "Id", "FullName");

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ProjectId,Archived,TicketTypeId,TicketPriorityId,TicketStatusId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int companyId = User.Identity!.GetCompanyId();
                string userId = _userManager.GetUserId(User);

                Ticket? oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id, companyId);
                try
                {
                    ticket.CreatedDate = DataUtility.GetPostGresDate(ticket.CreatedDate);
                    ticket.UpdatedDate = DataUtility.GetPostGresDate(DateTime.Now);
                    await _ticketService.UpdateTicketAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id, companyId);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, userId);

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketStatusId);
            return View(ticket);
        }

        // GET: Tickets/Archive/5
        public async Task<IActionResult> Archive(int id) //TODO: change to Archive. This will be just like the IsDeleted in the BlogApp
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.Archived = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity!.GetCompanyId();
            var tickets = await _ticketService.GetArchivedTicketsByCompanyAsync(companyId);

            return View(tickets);
        }

        //GET: Project to Restore and then confirm
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0 || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket != null)
            {
                ticket!.Archived = false;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ArchivedTickets));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                ticketAttachment.FileData = await _imageService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
                ticketAttachment.FileType = ticketAttachment.FormFile.ContentType;

                ticketAttachment.CreatedDate = DateTime.Now;
                ticketAttachment.UserId = _userManager.GetUserId(User);

                await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";
            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }

        //GET: Unassigned Tickets
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity!.GetCompanyId();
            //  int projectId = (await _context.Projects.Id)
            var tickets = (await _ticketService.GetAllTicketsByCompanyIdAsync(companyId)).Where(t => string.IsNullOrEmpty(t.DeveloperUserId));

            return View(tickets);
        }

        //GET: assign devs to ticket
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignDeveloper(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }

            AssignDeveloperViewModel viewModel = new();

            viewModel.Ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);
            int companyId = User.Identity!.GetCompanyId();

            viewModel.Developers = new SelectList(await _projectService.GetDevsAsync(companyId), "Id", "FullName");

            return View(viewModel);
        }

        //Post: assign dev to ticket
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel viewModel)
        {
            int companyId = User.Identity!.GetCompanyId();
            BTUser btUser = await _userManager.GetUserAsync(User);
            if (viewModel.DeveloperId != null)
            {
                Ticket? oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(viewModel.Ticket!.Id, companyId);
                try
                {
                    await _ticketService.AssignTicketAsync(viewModel.Ticket!.Id, viewModel.DeveloperId!);
                }
                catch
                {
                    throw;
                }

                string userId = _userManager.GetUserId(User);
                Ticket? newTicket = await _ticketService.GetTicketAsNoTrackingAsync(viewModel.Ticket!.Id, companyId);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, userId);

                Notification notification = new()
                {
                    NotificationTypeId = (await _context.NotificationTypes.FirstOrDefaultAsync(n => n.Name == nameof(BTNotificationType.Ticket))).Id,
                    TicketId = viewModel.Ticket.Id,
                    Title = "New Ticket Added",
                    Message = $"New Ticket: {viewModel.Ticket.Title} was created by {btUser.FullName}",
                    CreatedDate = DataUtility.GetPostGresDate(DateTime.UtcNow),
                    SenderId = userId,
                    RecipientId = viewModel.DeveloperId,
                };
                await _notificationService.AddNotificationAsync(notification);
                await _notificationService.SendEmailNotificationAsync(notification, "Ticket Assigned");

                return RedirectToAction(nameof(Details), new { id = viewModel.Ticket.Id });
            }
            return RedirectToAction(nameof(AssignDeveloper), new { ticketId = viewModel.Ticket!.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketcomment([Bind("Id,TicketId,Comment")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = _userManager.GetUserId(User);
                    ticketComment.CreatedDate = DateTime.UtcNow;

                    await _ticketService.AddTicketCommentAsync(ticketComment);

                    await _ticketHistoryService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);
                }
                catch
                {
                    throw;
                }
            }

            return RedirectToAction("Details", new { id = ticketComment.TicketId });
        }

        private bool TicketExists(int id)
        {
            return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> MyTickets()
        {
            int companyId = User.Identity!.GetCompanyId();

            BTUser user = await _userManager.GetUserAsync(User);
            string userId = user.Id;
            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId, companyId);

            return View(tickets.OrderByDescending(x => x.TicketPriority?.Id).ToList());
        }
    }
}