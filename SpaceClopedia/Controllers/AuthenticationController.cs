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
