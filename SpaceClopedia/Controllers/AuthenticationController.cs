using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Models;
using System.Security.Claims;
using System.Diagnostics;

namespace SpaceClopedia.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly SpaceClopediaContext context;
        public AuthenticationController(SpaceClopediaContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UtilizatorModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    if(string.IsNullOrWhiteSpace(model.NumeUtilizator))
                    {
                        ModelState.AddModelError(string.Empty, "Numele de utilizator este invalid");
                    }
                    else if(string.IsNullOrWhiteSpace(model.Parola))
                    {
                        ModelState.AddModelError(string.Empty, "Parola este invalida");
                    }
                    else if(string.IsNullOrWhiteSpace(model.ParolaConfirm) || model.Parola != model.ParolaConfirm)
                    {
                        ModelState.AddModelError(string.Empty, "Confirmarea parolei este invalida");
                    }
                    else if(context.Utilizator.Where(utilizator => utilizator.NumeUtilizator.ToLower() == model.NumeUtilizator.ToLower()).Count() > 0) 
                    {
                        ModelState.AddModelError(string.Empty, "Numele de utilizator este deja folosit");
                    }
                    else
                    {
                        try
                        {
                            context.Utilizator.Add(model);
                            context.SaveChanges();
                            return RedirectToAction("Index", "Home");
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, "Error creating account: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(UtilizatorModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.NumeUtilizator))
                        ModelState.AddModelError(string.Empty, "Numele de utilizator este gol!");
                    else if (string.IsNullOrWhiteSpace(model.Parola))
                        ModelState.AddModelError(string.Empty, "Parola este goala!");

                    UtilizatorModel? user = context.Utilizator.Where(user => user.NumeUtilizator!.ToLower() == model.NumeUtilizator!.ToLower() && user.Parola == model.Parola).FirstOrDefault();

                    if (user != null)
                    {
                        List<Claim> claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.NumeUtilizator!),
                            new Claim("Rol", user.Rol.ToString())
                        };
                        var claimIdentity = new ClaimsIdentity(claims, "AuthenticationCookie");

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError(string.Empty, "Invalid username or password!");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error logging in: " + ex.Message);
                }

            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Debug.WriteLine(error.ErrorMessage);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToAction("Index", "Home");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
