using KahootTeamRealTimeAdmin.Models;
using Microsoft.AspNetCore.Mvc;

namespace KahootTeamRealTimeAdmin.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        //Login 
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserNameOrEmail == "admin" && model.Password == "password")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }
    }
}

