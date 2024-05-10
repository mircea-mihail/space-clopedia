using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceClopedia.ContextModels;
using SpaceClopedia.Models;

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
    }
}
