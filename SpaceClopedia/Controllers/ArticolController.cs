using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Logic;
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
        public List<ArticolModel>? ArticoleCategorieCurenta { get; set; }
        public ArticolModel? ArticolCurent { get; set; }
        private readonly IWebHostEnvironment _environment;
        public string CuvantCautat = "";
        public List<SelectListItem> SortTypeList { get; set; } = new List<SelectListItem> { new SelectListItem { Text = "cele mai recente" }, new SelectListItem { Text = "alfabetic" }, new SelectListItem { Text = "lungime" } };
        public ArticolController(SpaceClopediaContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            Articole = _context.Articol.OrderBy(articol => articol.Titlu).ThenBy(articol => articol.DataModificare).ToList();
            List<DomeniuModel>? Domenii = _context.Domeniu.OrderBy(domeniu => domeniu.Id).ToList();

            var lenArticole = Articole.Count;
            int index = 0;
            while (index < lenArticole - 1)
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
            Articole = Articole.OrderByDescending(articol => articol.DataCreare).ToList();

            ViewBag.Domenii = Domenii;

            UtilizatorModel? utilizatorCurent = _context.Utilizator.Where(utilizator => utilizator.NumeUtilizator.ToString() == User.Identity.Name).FirstOrDefault();
            if (utilizatorCurent != null)
            {
                Rol rol = new Rol();
                rol = utilizatorCurent.Rol;

                Debug.WriteLine((int)rol);

                ViewBag.Rol = rol;
                Debug.WriteLine("\n\nrol curent: " + (int)rol);
            }

            if (Request.Cookies.TryGetValue("CuvantCautat", out string cuvantCautat))
            {
                CuvantCautat = cuvantCautat;
            }
            if (CuvantCautat == "")
            {
                return View(Articole);
            }

            Articole = Articole.Where(articol => articol.Continut.Contains(CuvantCautat) || articol.Titlu.Contains(CuvantCautat)).ToList();

            return View(Articole);

        }
        [HttpPost]
        public IActionResult SetCuvantCautat(string cuvantCautat)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddSeconds(5) // Set the expiration time to 5 seconds from now
            };

            CuvantCautat = cuvantCautat ?? "";
            Response.Cookies.Append("CuvantCautat", CuvantCautat, cookieOptions);
            return RedirectToAction("Index");
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
            UtilizatorModel? utilizatorCurent = _context.Utilizator.Where(utilizator => utilizator.NumeUtilizator.ToString() == User.Identity.Name).FirstOrDefault();
            if (utilizatorCurent != null)
            {
                Rol rol = new Rol();
                rol = utilizatorCurent.Rol;

                Debug.WriteLine((int)rol);

                ViewBag.Rol = rol;
            }
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
            Debug.WriteLine("\n\nultimul articol: " + articol.Titlu);

            ArticolModel articolModel = _context.Articol.Where(item => item.Titlu == articol.Titlu).OrderBy(articol => articol.DataModificare).LastOrDefault();
            articol.Titlu = articolModel.Titlu;
            articol.DataModificare = DateTime.Now;
            articol.DataCreare = articolModel.DataCreare;
            articol.NumarVersiune = articolModel.NumarVersiune + 1;
            articol.AutorModificare = User.Identity.Name;
            articol.Image = articolModel.Image;
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

        [HttpGet]
        public IActionResult StergeArticol(int articolId)
        {
            ArticolModel? articol = _context.Articol
                .Where(articol => articol.Id == articolId).Include(stire => stire.Domeniu).FirstOrDefault();
            if (articol == null)
            {
                return RedirectToAction("Error", "Home");
            }

            string titluArticol = articol.Titlu;
            List<ArticolModel>? articole = _context.Articol
                .Where(articol => articol.Titlu == titluArticol).Include(stire => stire.Domeniu).ToList();

            //Debug.WriteLine(articole.ToString());
            foreach (ArticolModel item in articole)
            {
                _context.Remove(item);
            }

            _context.SaveChanges();

            //_context.Remove(articol);
            //_context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RevertArticol(int articolId)
        {
            ArticolModel? articol = _context.Articol
                .Where(articol => articol.Id == articolId).Include(stire => stire.Domeniu).FirstOrDefault();
            if (articol == null)
            {
                return RedirectToAction("Error", "Home");
            }

            _context.Remove(articol);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ArticolePeCategorii(int domeniuId)
        {
            List<DomeniuModel>? Domenii = _context.Domeniu.OrderBy(domeniu => domeniu.Id).ToList();

            ArticoleCategorieCurenta = _context.Articol.OrderBy(articol => articol.Titlu).ThenBy(articol => articol.DataModificare).ToList();


            var lenArticole = ArticoleCategorieCurenta.Count;
            int index = 0;
            while (index < lenArticole - 1)
            {
                while (index < lenArticole - 1 && ArticoleCategorieCurenta[index].Titlu == ArticoleCategorieCurenta[index + 1].Titlu)
                {
                    ArticoleCategorieCurenta.RemoveAt(index);
                    lenArticole--;
                }
                index++;
            }

            ArticoleCategorieCurenta = ArticoleCategorieCurenta.Where(articol => articol.DomeniuId == domeniuId).ToList(); 

            if (ArticoleCategorieCurenta == null)
            {
                return View("Error", "Home");
            }

            DomeniuModel? domeniuCurent = Domenii.Where(domeniu => domeniu.Id == domeniuId).FirstOrDefault();
            ViewBag.DomeniuCurent = domeniuCurent;
            ViewBag.SortTypeList = SortTypeList;
            ViewBag.ArticoleCategorieCurenta = ArticoleCategorieCurenta;

            return View();
        }

        [HttpPost]
        public IActionResult ArticolePeCategorii(SortareViewModel sortareAleasa)
        {
            List<DomeniuModel>? Domenii = _context.Domeniu.OrderBy(domeniu => domeniu.Id).ToList();

            ArticoleCategorieCurenta = _context.Articol.OrderBy(articol => articol.Titlu).ThenBy(articol => articol.DataModificare).ToList();
            var lenArticole = ArticoleCategorieCurenta.Count;
            int index = 0;
            while (index < lenArticole - 1)
            {
                while (index < lenArticole - 1 && ArticoleCategorieCurenta[index].Titlu == ArticoleCategorieCurenta[index + 1].Titlu)
                {
                    ArticoleCategorieCurenta.RemoveAt(index);
                    lenArticole--;
                }
                index++;
            }
            ArticoleCategorieCurenta = ArticoleCategorieCurenta.Where(articol => articol.DomeniuId == sortareAleasa.IdDomeniuCurent).ToList();


            if (ArticoleCategorieCurenta == null || Domenii == null)
            {
                return View("Error", "Home");
            }

            if (sortareAleasa.TipSortare == SortTypeList[0].Text)
            {
                ArticoleCategorieCurenta = ArticoleCategorieCurenta.OrderByDescending(articol => articol.DataCreare).ToList();
            }
            if (sortareAleasa.TipSortare == SortTypeList[1].Text)
            {
                ArticoleCategorieCurenta = ArticoleCategorieCurenta.OrderBy(articol => articol.Titlu).ToList();
            }
            if (sortareAleasa.TipSortare == SortTypeList[2].Text)
            {
                ArticoleCategorieCurenta = ArticoleCategorieCurenta.OrderBy(articol => articol.Continut.Length).ToList();
            }

            Debug.WriteLine("\n\n id domeniu primit: " + sortareAleasa.IdDomeniuCurent);
            foreach (DomeniuModel domeniu in Domenii)
            {
                Debug.WriteLine(domeniu.NumeDomeniu + domeniu.Id);
            }

            DomeniuModel domeniuCurent = Domenii.FirstOrDefault(domeniu => domeniu.Id == sortareAleasa.IdDomeniuCurent);


            Debug.WriteLine("\n\nid domeniu curent: " + domeniuCurent.NumeDomeniu);

            ViewBag.DomeniuCurent = domeniuCurent;
            ViewBag.SortTypeList = SortTypeList;
            ViewBag.ArticoleCategorieCurenta = ArticoleCategorieCurenta;

            return View(sortareAleasa);
        }
    }
}
