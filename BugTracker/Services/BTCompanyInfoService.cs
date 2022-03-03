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
                                        .Where(m => m.CompanyId == companyId)
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
            List<Ticket> tickets = new();
            try
            {
                tickets = await _context.Tickets
                                        
                                        .ToListAsync();
                                        

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Invite>> GetAllInvitesAsync(int companyId)
        {
            List<Invite> invites = new();
            try
            {
                invites = await _context.Invites
                                        .Where(i => i.CompanyId == companyId)
                                        .ToListAsync();
                return invites;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
