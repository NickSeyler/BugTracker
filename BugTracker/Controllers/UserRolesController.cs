using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Controllers
{
    [Authorize(Roles="Admin")]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService,
                                   IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();
            int companyId = User.Identity!.GetCompanyId();

            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;

                IEnumerable<string> select = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", select);

                model.Add(viewModel);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = User.Identity!.GetCompanyId();

            BTUser? btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser?.Id);
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser!);
            string userRole = member.SelectedRoles?.FirstOrDefault()!;

            if (!string.IsNullOrEmpty(userRole))
            {
                if(await _rolesService.RemoveUserFromRolesAsync(btUser!, roles))
                {
                    await _rolesService.AddUserToRoleAsync(btUser!, userRole);
                }
            }


            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
