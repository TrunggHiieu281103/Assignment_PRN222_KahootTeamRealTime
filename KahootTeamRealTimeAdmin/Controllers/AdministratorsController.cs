using KahootTeamRealTimeAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories.Models;
using Services.Interfaces;

namespace KahootTeamRealTimeAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministratorsController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IRoleService _roleService;

        public AdministratorsController(IAdminService adminService, IRoleService roleService)
        {
            _adminService = adminService;
            _roleService = roleService;
        }

        // GET: Administrators
        public async Task<IActionResult> Index()
        {
            var administrators = await _adminService.GetAllAdministratorsAsync();
            return View(administrators);
        }

        // GET: Administrators/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await _roleService.GetAllRolesAsync(), "Id", "RoleName");
            return View();
        }

        // POST: Administrators/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdministratorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = new Administrator
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    IsActive = model.IsActive,
                    RoleId = model.RoleId
                };

                await _adminService.CreateAdministratorAsync(admin);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new SelectList(await _roleService.GetAllRolesAsync(), "Id", "RoleName");
            return View(model);
        }

        // GET: Administrators/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var administrator = await _adminService.GetAdministratorByIdAsync(id);
            if (administrator == null)
            {
                return NotFound();
            }

            var model = new AdministratorViewModel
            {
                Id = administrator.Id,
                UserName = administrator.UserName,
                IsActive = administrator.IsActive,
                RoleId = administrator.RoleId,
                RoleName = administrator.Role?.RoleName
            };

            ViewBag.Roles = new SelectList(await _roleService.GetAllRolesAsync(), "Id", "RoleName");
            return View(model);
        }

        // POST: Administrators/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdministratorViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var admin = await _adminService.GetAdministratorByIdAsync(id);
                if (admin == null)
                {
                    return NotFound();
                }

                admin.UserName = model.UserName;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    admin.Password = model.Password;
                }
                admin.IsActive = model.IsActive;
                admin.RoleId = model.RoleId;

                await _adminService.UpdateAdministratorAsync(admin);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new SelectList(await _roleService.GetAllRolesAsync(), "Id", "RoleName");
            return View(model);
        }

        // POST: Administrators/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleAdminActiveStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
