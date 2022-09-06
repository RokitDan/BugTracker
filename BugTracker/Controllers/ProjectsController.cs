using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BugTracker.Models.Enums;

namespace BugTracker.Controllers
{

    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BTUser> _userManager; //UserManage is designed to accept Identity type parameters or its children
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _rolesService;

        public ProjectsController(ApplicationDbContext context, IImageService imageService, UserManager<BTUser> userManager, IBTProjectService projectService, IBTRolesService rolesService)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _projectService = projectService;
            _rolesService = rolesService;
        }

        // GET: Current Projects
        public async Task<IActionResult> Index()
        {
            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            var currentProjects = await _projectService.GetCurrentProjectsByCompanyIdAsync(companyId);

            return View(currentProjects);

        }

        //GET: All Projects

        public async Task<IActionResult> AllProjects(int companyId)
        {
            companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            List<Project> allProjects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            return View(allProjects);

        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id!.Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            //TODO: Abstract the use of _context

            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name");

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,EndDate,ImageFile,ProjectPriorityId")] Project project)
        {
            if (ModelState.IsValid)
            {

                //TODO: Make the companyId retrieval more efficient because the program is currently hitting the database everytime the program needs companyId. It is not efficient this way

                project.CompanyId = (await _userManager.GetUserAsync(User)).CompanyId;

                //Dates
                project.CreatedDate = DataUtility.GetPostGresDate(DateTime.Now);
                project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                if (project.ImageFile != null)
                {
                    project.ImageData = await _imageService.ConvertFileToByteArrayAsync(project.ImageFile);
                    project.ImageType = project.ImageFile.ContentType;
                }

                await _projectService.AddProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);




            return View(project);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Description,CreatedDate,StartDate,EndDate,ImageData,ImageType,Archived,ImageFile,ProjectPriorityId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.CreatedDate = DataUtility.GetPostGresDate(project.CreatedDate);
                    project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                    project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                    if (project.ImageFile != null)
                    {
                        project.ImageData = await _imageService.ConvertFileToByteArrayAsync(project.ImageFile);
                        project.ImageType = project.ImageFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(project);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }



            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id!.Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }

            await _projectService.ArchiveProjectAsync(id);

            return RedirectToAction(nameof(ArchivedProjects));

        }



        // GET: ArchivedProjects
        public async Task<IActionResult> ArchivedProjects()
        {

            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            List<Project> archivedProjects = await _projectService.GetArchivedProjectsByCompanyIdAsync(companyId);

            return View(archivedProjects);

        }


        //GET: Project to Restore and then confirm
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id!.Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);

        }

        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            {
                if (_context.Projects == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
                }

                await _projectService.RestoreProjectAsync(id);

                return RedirectToAction(nameof(Index));

            }
        }

        //GET: Assign Project Manager
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignProjectManagerAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AssignPMViewModel model = new();

            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            model.Project = await _projectService.GetProjectByIdAsync(id.Value);

            //Get current PM (if exists)
            string? currentPMID = (await _projectService.GetProjectManagerAsync(model.Project.Id))?.Id;

            //Service call to RoleService
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName", currentPMID);



            return View(model);
        }

        //POST: Assign Project Manager

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignProjectManager(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                //Add PM to Project and TODO: enhance this process
                await _projectService.AddProjectManagerAsync(model.PMID, model.Project!.Id);

                return RedirectToAction(nameof(Index));
            }



            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            model.Project = await _projectService.GetProjectByIdAsync(model.Project!.Id);

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName");

            string? currentPMID = (await _projectService.GetProjectManagerAsync(model.Project.Id))?.Id;


            return RedirectToAction(nameof(AssignProjectManager), new { id = model.Project!.Id });
        }


        //GET: Unassigned Projects
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            List<Project> unassignedProjects = await _projectService.GetUnassignedProjectsAsync(companyId);
            return View(unassignedProjects);
        }





        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
