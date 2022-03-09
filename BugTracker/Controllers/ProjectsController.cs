#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using BugTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models.ViewModels;
using BugTracker.Models.Enums;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTFileService _fileService;

        public ProjectsController(ApplicationDbContext context,
                                  UserManager<BTUser> userManager,
                                  IBTProjectService projectService,
                                  IBTRolesService rolesService,
                                  IBTLookupService lookupService,
                                  IBTCompanyInfoService companyInfoService,
                                  IBTFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
            _rolesService = rolesService;
            _lookupService = lookupService;
            _companyInfoService = companyInfoService;
            _fileService = fileService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity.GetCompanyId();
            var applicationDbContext = await _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriorty).Where(p => p.CompanyId == companyId).ToListAsync();
            return View(applicationDbContext);
        }

        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);
            var projects = await _projectService.GetUserProjectsAsync(userId);

            return View(projects);
        }

        public async Task<IActionResult> AllProjects()
        {

            int companyId = User.Identity.GetCompanyId();
            var projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            
            return View(projects);
        }

        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId();
            var projects = await _projectService.GetArchivedProjectsByCompanyAsync(companyId);

            return View(projects);
        }

        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity.GetCompanyId();
            var projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPM(int? projectId)
        {
            if (projectId == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();

            AssignPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(projectId.Value, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                return RedirectToAction(nameof(AllProjects));
            }
            return RedirectToAction(nameof(AssignPM), new { projectId = model.Project!.Id });
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int companyId = User.Identity.GetCompanyId();
            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId();
            AddProjectWithPMViewModel model = new();

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            int companyId = User.Identity.GetCompanyId();

            if (ModelState.IsValid)
            {
                try
                {
                    if(model.Project?.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;
                    model.Project.CreatedDate = DateTime.UtcNow;

                    model.Project.StartDate = DateTime.SpecifyKind(model.Project.StartDate.DateTime, DateTimeKind.Utc);
                    model.Project.EndDate = DateTime.SpecifyKind(model.Project.EndDate.DateTime, DateTimeKind.Utc);

                    await _projectService.AddNewProjectAsync(model.Project);

                    if(!string.IsNullOrEmpty(model.PMID))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                    }

                    return RedirectToAction(nameof(Index));

                }
                catch(Exception)
                {
                    throw;
                }
            }

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // GET: Projects/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (model.Project == null)
            {
                return NotFound();
            }

            BTUser projectManager = await _projectService.GetProjectManagerAsync(companyId);
            if(projectManager != null)
            {
                model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName", projectManager!.Id);
            }
            else
            {
                model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            }
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                try
                {
                    if (model.Project?.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }
                    
                    model.Project.CreatedDate = DateTime.SpecifyKind(model.Project.CreatedDate.DateTime, DateTimeKind.Utc);
                    model.Project.StartDate = DateTime.SpecifyKind(model.Project.StartDate.DateTime, DateTimeKind.Utc);
                    model.Project.EndDate = DateTime.SpecifyKind(model.Project.EndDate.DateTime, DateTimeKind.Utc);

                    await _projectService.UpdateProjectAsync(model.Project);

                    if (!string.IsNullOrEmpty(model.PMID))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                    }

                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(model.Project!.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            int companyId = User.Identity.GetCompanyId();

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // GET: Projects/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            
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
            int companyId = User.Identity.GetCompanyId();
            var project = await _projectService.GetProjectByIdAsync(id, companyId);
            
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(p => p.Id == id);
        }
    }
}
