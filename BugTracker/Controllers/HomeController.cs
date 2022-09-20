using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BugTracker.Services.Interfaces;
using BugTracker.Data;
using BugTracker.Extensions;
using BugTracker.Models.ViewModels;
using Microsoft.AspNetCore.Identity;


namespace BugTracker.Controllers
{



    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyService _companyService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTProjectService _projectService;
        private readonly ILogger<HomeController> _logger;


        public HomeController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTCompanyService companyService, IBTTicketService ticketService, IBTProjectService projectService, ILogger<HomeController> logger)
        {
            _context = context;
            _companyService = companyService;
            _ticketService = ticketService;
            _projectService = projectService;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Landing()
        {
            return View();
        }



        public async Task<IActionResult> Index()
        {
            return View();
            //try
            //{
            //    int companyId = User.Identity!.GetCompanyId();
            //    Company company = await _companyService.GetCompanyInfoAsync(companyId);
            //    List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);//projects
            //    //List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyIdAsync(companyId);//tickets
            //    List<BTUser> members = await _companyService.GetCompanyMembersAsync(companyId);//members

            //    DashboardViewModel viewModel = new()
            //    {
            //        Company = company,
            //        Projects = projects,
            //        //Tickets = tickets,
            //        Members = members,
            //    };

            //    return View(viewModel);

            //}
            //catch
            //{
            //    throw;
            //}
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //GET; dashboard view
        [HttpGet]
        public async Task<IActionResult> DashboardView()
        {
            try
            {
                int companyId = User.Identity!.GetCompanyId();
                Company company = await _companyService.GetCompanyInfoAsync(companyId);
                List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);//projects
                List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyIdAsync(companyId);//tickets
                List<BTUser> members = await _companyService.GetCompanyMembersAsync(companyId);//members

                DashboardViewModel viewModel = new()
                {
                    Company = company,
                    Projects = projects,
                    Tickets = tickets,
                    Members = members,
                };

                return View(viewModel);

            }
            catch
            {
                throw;
            }
        }
    }
}