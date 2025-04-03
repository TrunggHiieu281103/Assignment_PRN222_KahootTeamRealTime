using KahootTeamRealTimeAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.Interfaces;

namespace KahootTeamRealTimeAdmin.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IAdminService _adminService;

        public RegisterController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet] 
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost] 
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var admin = new Administrator
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth),
                    IsActive = true,
                    RoleId = 2
                };

                var registeredAdmin = await _adminService.RegisterAsync(admin);

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}
