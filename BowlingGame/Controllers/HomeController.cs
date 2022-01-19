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
                gameList = context.Games.ToList().Select(x =>
                {
                    dynamic game = new ExpandoObject();
                    game.id = x.GameID;
                    game.name = x.playerName;
                    game.score = x.score ?? 0;
                    return game;
                }).ToList();
                return gameList;
            }
        }
        [HttpPost]
        public dynamic CreateBowlingGame(string name)
        {
            if(name == null || name == "")
            {
                return new { code = 0, message = "Please enter your name" };
            }
            using(BowlingGameContext context = new())
            {
                Game game = new Game
                {
                    playerName = name
                };
                context.Games.Add(game);
                context.SaveChanges();
                return new { code = 1 };
            }
           
        }
        [HttpPost]
        public dynamic deleteGame(int id)
        {
            using(BowlingGameContext context = new())
            {
                var game = context.Games.Where(g => g.GameID == id).FirstOrDefault();
                if (game != null)
                {
                    context.Games.Remove(game);
                    context.SaveChanges();
                    return new { code = 1 };
                }
                else
                {
                    return new { code = 0, message = "An error occured deleting your game. Please refresh and try again" };


                }

            }
        }
    }
}