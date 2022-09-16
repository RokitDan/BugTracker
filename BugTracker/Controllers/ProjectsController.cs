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
using BugTracker.Extensions;

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
            //int companyId = User.Identity!.GetCompanyId();

            int companyId = User.Identity!.GetCompanyId();

            var currentProjects = await _projectService.GetCurrentProjectsByCompanyIdAsync(companyId);

            return View(currentProjects);

        }

        //GET: All Projects

        public async Task<IActionResult> AllProjects(int companyId)
        {
            companyId = User.Identity!.GetCompanyId();

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
        public async Task<IActionResult> Create()
        {
            CreateEditProjectViewModel viewModel = new()
            {
                Project = new Project(),
                ProjectManagers = new SelectList(await _rolesService.GetUsersInRoleAsync("ProjectManager", User.Identity!.GetCompanyId()), "Id", "FullName"),
                ProjectPriorityId = new SelectList(_context.ProjectPriorities, "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditProjectViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                viewModel.Project!.CompanyId = User.Identity!.GetCompanyId();

                //Dates
                viewModel.Project!.CreatedDate = DataUtility.GetPostGresDate(DateTime.Now);
                viewModel.Project!.StartDate = DataUtility.GetPostGresDate(viewModel.Project!.StartDate);
                viewModel.Project!.EndDate = DataUtility.GetPostGresDate(viewModel.Project!.EndDate);

                if (viewModel.Project!.ImageFile != null)
                {
                    viewModel.Project!.ImageData = await _imageService.ConvertFileToByteArrayAsync(viewModel.Project!.ImageFile);
                    viewModel.Project!.ImageType = viewModel.Project!.ImageFile.ContentType;
                }

                if (!string.IsNullOrEmpty(viewModel.PMID))
                {
                    await _projectService.AddProjectManagerAsync(viewModel.PMID!, viewModel.Project.Id);
                };

                if (User.IsInRole(nameof(BTRoles.DemoUser)))
                {
                    await _projectService.AddProjectAsync(viewModel.Project!);
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", viewModel.Project!.ProjectPriorityId);

            return View(viewModel);
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
            project!.CreatedDate = DataUtility.GetPostGresDate(project.CreatedDate);
            project!.StartDate = DataUtility.GetPostGresDate(project.StartDate);
            project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

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
                    //project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                    //project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

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
        //TODO: maybe only authorize PM and admin to see archived projects in view. ask antonio
        public async Task<IActionResult> ArchivedProjects()
        {

            int companyId = User.Identity!.GetCompanyId();

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

            int companyId = User.Identity!.GetCompanyId();

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



            int companyId = User.Identity!.GetCompanyId();

            model.Project = await _projectService.GetProjectByIdAsync(model.Project!.Id);

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName");

            string? currentPMID = (await _projectService.GetProjectManagerAsync(model.Project.Id))?.Id;


            return RedirectToAction(nameof(AssignProjectManager), new { id = model.Project!.Id });
        }


        //GET: Unassigned Projects
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity!.GetCompanyId();

            List<Project> unassignedProjects = await _projectService.GetUnassignedProjectsAsync(companyId);
            return View(unassignedProjects);
        }


        //GET: Add Team Members Page
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AddTeamMembers(int id, int companyId)
        {
            ProjectMembersViewModel viewModel = new();
            companyId = User.Identity!.GetCompanyId();
            viewModel.Project = await _projectService.GetProjectByIdAsync(id);

            if (id <= 0)
            {
                return NotFound();
            }

            List<string> currentMembers = viewModel.Project.Members!.Select(m => m.Id).ToList();
            List<BTUser> devsAndSubs = await _projectService.GetDevsAndSubsAsync(id, companyId);
            viewModel.CompanyMembers = new MultiSelectList(devsAndSubs, "Id", "FullName", currentMembers);

            return View(viewModel);
        }

        //POST: Save Team Members to Project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamMembers(ProjectMembersViewModel viewModel)
        {
            int companyId = User.Identity!.GetCompanyId();
            viewModel.Project = await _projectService.GetProjectByIdAsync(viewModel.Project!.Id);

            await _projectService.RemoveUsersButNotPMAsync(viewModel.Project!.Id);

            foreach (string userId in viewModel.SelectedMembers)
            {
                BTUser? employee = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                await _projectService.AddUserToProjectAsync(employee!, viewModel.Project!.Id)!;
            }

            return RedirectToAction(nameof(Index));

        }






        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
