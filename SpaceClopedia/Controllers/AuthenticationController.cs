using Microsoft.AspNetCore.Mvc;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Models;

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
    }
}
