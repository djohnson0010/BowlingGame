using BowlingGame.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Dynamic;

namespace BowlingGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public dynamic getGameList()
        {
            dynamic gameList = new ExpandoObject();
            using (BowlingGameContext context = new())
            {
                gameList.
            }
        }
        [HttpPost]
        public dynamic CreateBowlingGame(string name)
        {
            using(BowlingGameContext context = new())
            {
                Game game = new Game
                {
                    playerName = name
                };
                context.Games.Add(game);
                context.SaveChanges();
                return new { code = 0, message = "test", data = context.Games.ToList() };
            }
           
        }
    }
}