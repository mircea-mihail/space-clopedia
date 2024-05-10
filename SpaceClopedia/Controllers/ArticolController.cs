﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Models;
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
            Articole = _context.Articol.ToList();
            if(Articole == null)
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
            if(!ModelState.IsValid)
            {
                List<SelectListItem> domenii = _context.Domeniu.Select(domeniu => new SelectListItem { Text = domeniu.NumeDomeniu, Value = domeniu.Id.ToString() }).ToList();

                ViewBag.Domenii = domenii;

                return View(articolNou);
            }

            //procesare
            //POST-- > functie de procesare a continutului --> face split dupa continutul text si titlul de imagine
            //    --> se adauga implicit data creare si modif, versionare 0, autor modificare = autor

            articolNou.Domeniu = _context.Domeniu.Where(domeniu => domeniu.Id == articolNou.Domeniu.Id).FirstOrDefault();
            _context.Add(articolNou);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}