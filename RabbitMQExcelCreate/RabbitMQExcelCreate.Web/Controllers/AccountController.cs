using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace RabbitMQExcelCreate.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _singInManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> singInManager)
        {
            _userManager = userManager;
            _singInManager = singInManager;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var hasUser = await _userManager.FindByEmailAsync(Email);
            if (hasUser==null)
            {
                return View();
            }
            var signInResult=await _singInManager.PasswordSignInAsync(hasUser, Password,true,false);
            if (!signInResult.Succeeded)
            {
                return View();
            }
            return RedirectToAction(nameof(HomeController.Index),"Home");
        }
    }
}
