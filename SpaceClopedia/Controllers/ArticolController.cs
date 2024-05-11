using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Models;
using System.Diagnostics;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpaceClopedia.Controllers
{
    public class ArticolController : Controller
    {
        private readonly SpaceClopediaContext _context;
        public List<ArticolModel>? Articole { get; set; }
        public ArticolModel? ArticolCurent { get; set; }
        public ArticolController(SpaceClopediaContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            Articole = _context.Articol.OrderBy(articol => articol.Titlu).ToList();
            var lenArticole = Articole.Count;
            int index = 0;
            while(index < lenArticole - 1)
            {
                while (Articole[index].Titlu == Articole[index + 1].Titlu)
                {
                    Articole.RemoveAt(index);
                    lenArticole--;
                }

                index++;
            }

            if (Articole == null)
            {
                return View("Error", "Home");
            }
            //"index" este redundant, deoarece se poate lua implicit din numele metodei
            //return View("Index", Articole);
            return View(Articole);
        }

        [HttpGet]
        public IActionResult Articol(int articolId)
        {
            ArticolCurent = _context.Articol.Where(articol => articol.Id == articolId).Include(articol => articol.Domeniu).FirstOrDefault();
            if (ArticolCurent == null)
            {
                return View("Error", "Home");
            }

            return View(ArticolCurent);
        }

        [HttpGet]
        public IActionResult AdaugaArticol()
        {
            List<SelectListItem> domenii = _context.Domeniu.Select(domeniu => new SelectListItem { Text = domeniu.NumeDomeniu, Value = domeniu.Id.ToString() }).ToList();

            ViewBag.Domenii = domenii;

            return View();
        }

        [HttpPost]
        public IActionResult AdaugaArticol(ArticolModel articolNou)
        {
            articolNou.DataCreare = DateTime.Now;
            articolNou.DataModificare = DateTime.Now;
            articolNou.NumarVersiune = 0;
            articolNou.AutorModificare = articolNou.Autor;

            if (!ModelState.IsValid)
            {
                List<SelectListItem> domenii = _context.Domeniu.Select(domeniu => new SelectListItem { Text = domeniu.NumeDomeniu, Value = domeniu.Id.ToString() }).ToList();

                ViewBag.Domenii = domenii;

                return View(articolNou);
            }

            //procesare
            //POST-- > functie de procesare a continutului --> face split dupa continutul text si titlul de imagine
            //    --> se adauga implicit data creare si modif, versionare 0, autor modificare = autor

            articolNou.Domeniu = _context.Domeniu.Where(domeniu => domeniu.Id == articolNou.DomeniuId).FirstOrDefault();

            Debug.WriteLine(articolNou.Continut.ToString());

            _context.Add(articolNou);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditeazaArticol(int articolId)
        {
            List<SelectListItem> domenii = _context.Domeniu.Select(domeniu => new SelectListItem { Text = domeniu.NumeDomeniu, Value = domeniu.Id.ToString() }).ToList();

            ViewBag.Domenii = domenii;

            ArticolModel? articol = _context.Articol.Where(articol => articol.Id == articolId).Include(articol => articol.Domeniu).FirstOrDefault();

            if (articol == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(articol);
        }

        [HttpPost]
        public IActionResult EditeazaArticol(ArticolModel articol)
        {

            ArticolModel articolModel = _context.Articol.Where(articol => articol.Titlu == articol.Titlu).OrderBy(articol => articol.DataModificare).LastOrDefault();
            articol.Titlu = articolModel.Titlu;
            articol.DataModificare = DateTime.Now;
            articol.DataCreare = articolModel.DataCreare;
            articol.NumarVersiune = articolModel.NumarVersiune + 1;
            articol.AutorModificare = "Anonymous";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Debug.WriteLine(error.ErrorMessage);
                }

                List<SelectListItem> domenii = _context.Domeniu.Select(domeniu => new SelectListItem { Text = domeniu.NumeDomeniu, Value = domeniu.Id.ToString() }).ToList();

                ViewBag.Domenii = domenii;

                return View(articol);
            }

            articol.Domeniu = _context.Domeniu.Where(domeniu => domeniu.Id == articol.DomeniuId).FirstOrDefault();

            Debug.WriteLine(articol.Continut.ToString());

            _context.Add(articol);
            _context.SaveChanges();

            return View("Articol", articol);
        }
    }
}
