using BugTracker.Data;
using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ChartModels;
using BugTracker.Models.Enums;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
                    Projects = projects.OrderBy(x => x.EndDate).ToList(),
                    Tickets = tickets.OrderByDescending(x => x.TicketPriority?.Id).ToList(),
                    Members = members,
                };

                return View(viewModel);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> PlotlyBarChart()
        {
            PlotlyBarData plotlyData = new();
            List<PlotlyBar> barData = new();

            int companyId = User.Identity.GetCompanyId();

            List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            //Bar One
            PlotlyBar barOne = new()
            {
                X = projects.Select(p => p.Name).ToArray(),
                Y = projects.SelectMany(p => p.Tickets).GroupBy(t => t.ProjectId).Select(g => g.Count()).ToArray(),
                Name = "Tickets",
                Type = "bar"
            };

            //Bar Two
            PlotlyBar barTwo = new()
            {
                X = projects.Select(p => p.Name).ToArray(),
                Y = projects.Select(async p => (await _projectService.GetProjectMembersbyRoleAsync(p.Id, nameof(BTRoles.Developer))).Count).Select(c => c.Result).ToArray(),
                Name = "Developers",
                Type = "bar"
            };

            barData.Add(barOne);
            barData.Add(barTwo);

            plotlyData.Data = barData;

            return Json(plotlyData);
        }

        [HttpPost]
        public async Task<JsonResult> GglProjectTickets()
        {
            int companyId = User.Identity.GetCompanyId();

            List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "ProjectName", "TicketCount" });

            foreach (Project prj in projects)
            {
                chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
            }

            return Json(chartData);
        }

        [HttpPost]
        public async Task<JsonResult> GglProjectPriority()
        {
            int companyId = User.Identity.GetCompanyId();

            List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "Priority", "Count" });


            foreach (string priority in Enum.GetNames(typeof(BTProjectPriorities)))
            {
                int priorityCount = (await _projectService.GetAllProjectsByPriorityAsync(companyId, priority)).Count();
                chartData.Add(new object[] { priority, priorityCount });
            }

            return Json(chartData);
        }


        [HttpPost]
        public async Task<JsonResult> AmCharts()
        {

            AmChartData amChartData = new();
            List<AmItem> amItems = new();

            int companyId = User.Identity.GetCompanyId();

            List<Project> projects = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId)).Where(p => p.Archived == false).ToList();

            foreach (Project project in projects)
            {
                AmItem item = new();

                item.Project = project.Name;
                item.Tickets = project.Tickets.Count;
                item.Developers = (await _projectService.GetProjectMembersbyRoleAsync(project.Id, nameof(BTRoles.Developer))).Count();

                amItems.Add(item);
            }

            amChartData.Data = amItems.ToArray();


            return Json(amChartData.Data);
        }
    }

}