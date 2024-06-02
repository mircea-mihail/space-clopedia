using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Models;
using System;
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
        private readonly IWebHostEnvironment _environment;
        public ArticolController(SpaceClopediaContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            Articole = _context.Articol.OrderBy(articol => articol.Titlu).ToList();
            var lenArticole = Articole.Count;
            int index = 0;
            while(index < lenArticole - 1)
            {
                while (index < lenArticole - 1 && Articole[index].Titlu == Articole[index + 1].Titlu)
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
        public async Task<IActionResult> GetImage(int id)
        {
            var article = await _context.Articol.FindAsync(id);
            if (article == null || article.Image == null)
            {
                return NotFound();
            }

            return File(article.Image, "image/jpeg"); // Adjust the MIME type as needed
        }

        [HttpGet]
        public IActionResult Articol(int articolId)
        {

            List<SelectListItem> accessLevel = new List<SelectListItem>();
            accessLevel.Add(new SelectListItem { Text = "Protected" });
            accessLevel.Add(new SelectListItem { Text = "Public" });
            ViewBag.AccessLevel = accessLevel;

            ArticolCurent = _context.Articol.Where(articol => articol.Id == articolId).OrderBy(articol => articol.NumarVersiune).Include(articol => articol.Domeniu).LastOrDefault();
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
            List<SelectListItem> accessLevel = new List<SelectListItem>();
            accessLevel.Add(new SelectListItem { Text = "Protected" });
            accessLevel.Add(new SelectListItem { Text = "Public" });

            ViewBag.Domenii = domenii;
            ViewBag.AccessLevel = accessLevel;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdaugaArticol(ArticolViewModel articol)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            //foreach (var error in errors)
            //{
            //    Debug.WriteLine(error.ErrorMessage);
            //}

            ArticolModel articolNou = new ArticolModel();

            articolNou.DomeniuId = articol.DomeniuId;
            articolNou.Titlu = articol.Titlu;
            articolNou.Continut = articol.Continut;
            articolNou.Autor = articol.Autor;
            articolNou.DataCreare = DateTime.Now;
            articolNou.DataModificare = DateTime.Now;
            articolNou.NumarVersiune = 0;
            articolNou.AutorModificare = articolNou.Autor;
            articolNou.AccessLevel = articol.AccessLevel;

            if (!ModelState.IsValid)
            {
                List<SelectListItem> domenii = _context.Domeniu.Select(domeniu => new SelectListItem { Text = domeniu.NumeDomeniu, Value = domeniu.Id.ToString() }).ToList();
                List<SelectListItem> accessLevel = new List<SelectListItem>();
                accessLevel.Add(new SelectListItem { Text = "Protected" });
                accessLevel.Add(new SelectListItem { Text = "Public" });

                ViewBag.Domenii = domenii;
                ViewBag.AccessLevel = accessLevel;

                return View(articolNou);
            }

            if (articol.ImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await articol.ImageFile.CopyToAsync(memoryStream);
                    articolNou.Image = memoryStream.ToArray();
                }
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
            List<SelectListItem> accessLevel = new List<SelectListItem>();
            accessLevel.Add(new SelectListItem { Text = "Protected" });
            accessLevel.Add(new SelectListItem { Text = "Public" });

            ViewBag.Domenii = domenii;
            ViewBag.AccessLevel = accessLevel;

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
            articol.AutorModificare = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Debug.WriteLine(error.ErrorMessage);
                }

                List<SelectListItem> domenii = _context.Domeniu.Select(domeniu => new SelectListItem { Text = domeniu.NumeDomeniu, Value = domeniu.Id.ToString() }).ToList();
                List<SelectListItem> accessLevel = new List<SelectListItem>();
                accessLevel.Add(new SelectListItem { Text = "Protected" });
                accessLevel.Add(new SelectListItem { Text = "Public" });

                ViewBag.Domenii = domenii;
                ViewBag.AccessLevel = accessLevel;

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
