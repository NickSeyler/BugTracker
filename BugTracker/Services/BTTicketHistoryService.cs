using BugTracker.Models;
using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        public Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            throw new NotImplementedException();
        }

        public Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }
    }
}
