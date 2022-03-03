using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTCompanyInfoService : IBTCompanyInfoService
    {
        private readonly ApplicationDbContext _context;

        public BTCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {

            Company? company = new();

            try
            {
                if (companyId != null)
                {
                    company = await _context.Companies
                                            .Include(c => c.Members)
                                            .Include(c => c.Projects)
                                            .Include(c => c.Invites)
                                            .FirstOrDefaultAsync(c => c.Id == companyId);
                }
                return company!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            List<BTUser> members = new();
            try
            {
                members = await _context.Users
                                        .Where(u => u.CompanyId == companyId)
                                        .ToListAsync();
                return members;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> projects = new();
            try
            {
                projects = await _context.Projects
                                         .Where(p => p.CompanyId == companyId)
                                         .Include(p => p.Members)!
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.Comments)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.Attachments)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.History)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.Notifications)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.DeveloperUser)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.OwnerUser)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.TicketStatus)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.TicketPriority)
                                         .Include(p => p.Tickets)!
                                             .ThenInclude(t => t.TicketType)
                                         .Include(p => p.ProjectPriorty)
                                         .ToListAsync();
                return projects;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
        {
            List<Project> projects = new();
            List<Ticket> tickets = new();
            try
            {
                projects = await GetAllProjectsAsync(companyId);
                tickets = projects.SelectMany(p => p.Tickets).ToList();
                                        

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Invite>> GetAllInvitesAsync(int companyId)
        {
            List<Invite>? invites = new();
            try
            {
                invites = (await _context.Companies
                                         .Include(c => c.Invites)!
                                            .ThenInclude(i => i.Company)
                                         .Include(c => c.Invites)!
                                            .ThenInclude(i => i.Project)
                                         .Include(c => c.Invites)!
                                            .ThenInclude(i => i.Invitee)
                                         .Include(c => c.Invites)!
                                            .ThenInclude(i => i.Invitor)
                                         .FirstOrDefaultAsync(c => c.Id == companyId))?.Invites!.ToList();
                return invites!;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
