using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Linq;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Valid = false;
            return View(new TheModel());
        }

        [HttpPost]
        public IActionResult Index(TheModel model)
        {
            ViewBag.Valid = ModelState.IsValid;
            
            //CHATGPT
            //https://stackoverflow.com/questions/449513/removing-characters-from-strings-with-linq

            if (ViewBag.Valid)
            {
                var filtered = model.Phrase.Where(c => c != ' ');

                model.Counts = filtered
                    .GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count());
                model.Lower = new string(filtered.Select(char.ToLower).ToArray());
                model.Upper = new string(filtered.Select(char.ToUpper).ToArray());
            }

            return View(model);
        }
    }
}
