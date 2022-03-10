﻿#nullable disable
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
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models.ViewModels;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _ticketService;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;

        public TicketsController(ApplicationDbContext context,
                                 UserManager<BTUser> userManager,
                                 IBTTicketService ticketService,
                                 IBTProjectService projectService,
                                 IBTCompanyInfoService companyInfoService,
                                 IBTRolesService rolesService,
                                 IBTLookupService lookupService)
        {
            _context = context;
            _userManager = userManager;
            _ticketService = ticketService;
            _projectService = projectService;
            _companyInfoService = companyInfoService;
            _rolesService = rolesService;
            _lookupService = lookupService;
        }

        // GET: Tickets/MyTickets
        public async Task<IActionResult> MyTickets()
        {
            string userId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId();
            List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(userId, companyId);

            return View(tickets);
        }

        // GET: Tickets/AllTickets
        public async Task<IActionResult> AllTickets()
        {
            List<Ticket> tickets = new();
            int companyId = User.Identity.GetCompanyId();

            if (User.IsInRole(nameof(BTRole.Admin)) || User.IsInRole(nameof(BTRole.ProjectManager)))
            {
                tickets = await _companyInfoService.GetAllTicketsAsync(companyId);
            }
            else
            {
                tickets = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Where(t => !t.Archived).ToList();
            }

            return View(tickets);
        }

        // GET: Tickets/ArchivedTickets
        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);

            return View(tickets);
        }

        // GET: Tickets/UnassignedTickets
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity.GetCompanyId();
            string btUserId = _userManager.GetUserId(User);

            List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);

            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                return View(tickets);
            }
            else
            {
                List<Ticket> pmTickets = new();

                foreach (Ticket ticket in tickets)
                {
                    if (await _projectService.IsAssignedProjectManagerAsync(btUserId, ticket.ProjectId))
                    {
                        pmTickets.Add(ticket);
                    }
                }

                return View(pmTickets);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignDeveloper(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }


            AssignDeveloperViewModel model = new();

            model.Ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);
            model.DeveloperList = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(BTRole.Developer)), "Id", "FullName");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if (!string.IsNullOrEmpty(model.DeveloperID))
            {
                await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperID);
                return RedirectToAction(nameof(AllTickets));
            }
            return RedirectToAction(nameof(AssignDeveloper), new { ticketId = model.Ticket!.Id });
        }



        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId();
            AddTicketWithProjectViewModel model = new();

            model.OwnerUserID = _userManager.GetUserId(User);
            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                model.ProjectList = new SelectList(await _companyInfoService.GetAllProjectsAsync(companyId), "Id", "Name");
            }
            else
            {
                model.ProjectList = new SelectList(await _projectService.GetUserProjectsAsync(model.OwnerUserID), "Id", "Name");
            }
            model.PriorityList = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            model.StatusList = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name");
            model.TypeList = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddTicketWithProjectViewModel model)
        {
            int companyId = User.Identity.GetCompanyId();;

            if (ModelState.IsValid)
            {
                try
                {
                    model.Ticket.CreatedDate = DateTime.UtcNow;

                    if (!string.IsNullOrEmpty(model.ProjectID.ToString()))
                    {
                        model.Ticket.ProjectId = model.ProjectID.Value;
                    }    

                    await _ticketService.AddNewTicketAsync(model.Ticket);

                    return RedirectToAction(nameof(AllTickets));
                }
                catch (Exception)
                {
                    throw;
                }
            }


            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                model.ProjectList = new SelectList(await _companyInfoService.GetAllProjectsAsync(companyId), "Id", "Name");
            }
            else
            {
                model.ProjectList = new SelectList(await _projectService.GetUserProjectsAsync(model.OwnerUserID), "Id", "Name");
            }
            model.PriorityList = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            model.StatusList = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name");
            model.TypeList = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            return View(model);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();
            AddTicketWithProjectViewModel model = new();

            model.Ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (model.Ticket == null)
            {
                return NotFound();
            }


            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                model.ProjectList = new SelectList(await _companyInfoService.GetAllProjectsAsync(companyId), "Id", "Name");
            }
            else
            {
                model.ProjectList = new SelectList(await _projectService.GetUserProjectsAsync(model.OwnerUserID), "Id", "Name");
            }
            model.PriorityList = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            model.StatusList = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name");
            model.TypeList = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddTicketWithProjectViewModel model)
        {
            if (model != null)
            {
                try
                {

                    if (!string.IsNullOrEmpty(model.ProjectID.ToString()))
                    {
                        model.Ticket.ProjectId = model.ProjectID.Value;
                    }

                    model.Ticket.CreatedDate = DateTime.SpecifyKind(model.Ticket.CreatedDate.DateTime, DateTimeKind.Utc);
                    model.Ticket.UpdatedDate = DateTime.UtcNow;

                    await _ticketService.UpdateTicketAsync(model.Ticket);

                    return RedirectToAction(nameof(AllTickets));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(model.Ticket!.Id))
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

            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                model.ProjectList = new SelectList(await _companyInfoService.GetAllProjectsAsync(companyId), "Id", "Name");
            }
            else
            {
                model.ProjectList = new SelectList(await _projectService.GetUserProjectsAsync(model.OwnerUserID), "Id", "Name");
            }
            model.PriorityList = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            model.StatusList = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name");
            model.TypeList = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            return View(model);
        }


        // GET: Tickets/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Projects/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            await _ticketService.ArchiveTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }

        // GET: Projects/Restore/5
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Projects/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            await _ticketService.RestoreTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    }
}
