using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;

        public BTProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
                var project = await _context.Projects
                                            .Include(p => p.Company)
                                            .Include(p => p.ProjectPriorty)
                                            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
                return project!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public Task ArchiveProjectAsync(Project project)
        {
            throw new NotImplementedException();
        }


        public Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> IsAssignedProjectManager(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserOnProject(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<int> LookupProjectPriorityId(string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RestoreProjectsAsync(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
