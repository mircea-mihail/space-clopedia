using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Logic;
using SpaceClopedia.Models;
using System.Diagnostics;

namespace SpaceClopedia.Controllers
{
    public class UtilizatorController : Controller
    {
        private readonly SpaceClopediaContext _context;
        public List<UtilizatorModel>? Utilizatori { get; set; }

        public UtilizatorController(SpaceClopediaContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var utilizatori = _context.Utilizator.OrderBy(u => u.NumeUtilizator).ToList();

            if (utilizatori == null)
            {
                return View("Error", "Home");
            }

            var rolValues = Enum.GetValues(typeof(Rol)).Cast<Rol>();
            var roluri = rolValues.Select(r => new SelectListItem
            {
                Value = ((int)r).ToString(),
                Text = r.ToString()
            }).ToList();

            ViewBag.Roluri = roluri;

            var viewModel = utilizatori.Select(u => new UtilizatorViewModel
            {
                UserId = u.Id,
                UserName = u.NumeUtilizator,
                SelectedRole = (int)u.Rol
            }).ToList();
            var utilizatorCurent = utilizatori.Where(user => user.NumeUtilizator == User.Identity.Name).FirstOrDefault();
            if ((int)utilizatorCurent.Rol != 0)
            {
                return View("Error", "Home");
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateRoles(List<UtilizatorViewModel> model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Debug.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                Debug.WriteLine(model.Count);
                foreach (UtilizatorViewModel item in model)
                { 
                    UtilizatorModel? user = _context.Utilizator.Where(utilizator => utilizator.Id == item.UserId).FirstOrDefault();
                    //Debug.WriteLine(user.NumeUtilizator);
                    //Debug.WriteLine((int)item.SelectedRole);
                    if (user != null)
                    {
                        user.Rol = (Rol)item.SelectedRole;
                        _context.Update(user);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View("Index", model);
        }
    }


}
