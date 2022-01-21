using BowlingGame.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;

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
            using(BowlingGameContext context = new())
            {
                context.Database.EnsureCreated();
            }
            
            return View();
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
                return new { code = 1, gameID = game.GameID };
            }
           
        }
        [HttpPost]
        public dynamic getBowlingGame(int gameID)
        {
            int totalScore = 0;
            using(BowlingGameContext context = new())
            {
                dynamic bowlingGame = new ExpandoObject();
                bowlingGame = context.Games.Where(g => g.GameID == gameID).Include(g => g.Frames).ThenInclude(g => g.scores).ToList().Select(x =>
                {
                    dynamic game = new ExpandoObject();
                    int bowlingPins = 10;
                    game.name = x.playerName;
                    game.gameID = x.GameID;
                    game.isFinished = x.isFinished;
                    game.Frames = x.Frames.ToList().Select(f =>
                    {
                        int frameScore = 0;
                        dynamic frame = new ExpandoObject();
                        frame.frameID = f.FrameID;
                        frame.scores = f.scores.Select(s =>
                        {
                            dynamic score = new ExpandoObject();
                            score.position = s.scorePosition;
                            score.scoreID = s.scoreID;
                            score.scoreNumber = s.scoreNumber;
                            score.isSpare = s.isSpare;
                            score.isStrike = s.isStrike;
                            frameScore += score.scoreNumber;

                            return score;
                        }).ToList();
                        frame.frameScore = f.isScored ? f.frameScore.ToString() : "";
                        if(!f.isFinished)
                        {
                            bowlingPins = 10 - (frameScore % 10);
                        }
                        frame.isFinished = f.isFinished;
                        
                        return frame;

                    }).ToList();
                    game.bowlingPins = bowlingPins;
                    return game;
                }).First();
                return bowlingGame;
            }
        }
        [HttpPost]
        public void addScore(int score, int gameID)
        {
            using(BowlingGameContext context = new())
            {
                var game = context.Games.Where(g => g.GameID == gameID).Include(g => g.Frames).FirstOrDefault();
                if(game != null && !game.isFinished)
                {
                    var frames = game.Frames.OrderByDescending(f => f.frameNumber).ToList();
                    if((frames.Count() == 0) || frames.First().isFinished)
                    {
                        addFrameToGame(gameID, score);
                    }
                    else
                    {
                        int frameID = frames.Select(f => f.FrameID).FirstOrDefault();
                        int frameNumber = frames.Select(f => f.frameNumber).First();
                        addScoreToFrame(frameID, frameNumber, score);
                    }
                }
                calculateGameScores(gameID);
            }
        }
        public void addScoreToFrame(int frameID, int frameNumber, int score)
        {
            using(BowlingGameContext context = new())
            {
                var scores = context.Scores.Where(f => f.FrameID == frameID).ToList();
                int scorePosition = scores.Count() + 1;
                var frame = context.Frames.Where(f => f.FrameID == frameID).First();
                Score newScore = new Score
                {
                    FrameID = frameID,
                    scorePosition = scorePosition,
                    scoreNumber = score,
                    isStrike = score == 10,
                    isSpare = scores.Where(s => !s.isStrike).Select(s => s.scoreNumber).Sum() + score == 10
                    
                    
                };
                context.Scores.Add(newScore);
                context.SaveChanges();
                var rem = ((scores.Select(s => s.scoreNumber).Sum() + score) % 10);
                if (((score == 10 || scorePosition == 2) && frameNumber != 10) || (frameNumber == 10 && scorePosition == 2 && (scores.Select(s => s.scoreNumber).Sum() + score) % 10 != 0) || (frameNumber == 10 && scorePosition == 3))
                {
                    frame.isFinished = true;
                    if(frameNumber == 10)
                    {
                        var game = context.Games.Where(g => g.GameID == frame.GameID).First();
                        game.isFinished = true;
                    }
                    context.SaveChanges();
                }
            }
        }
        public void calculateGameScores(int gameID)
        {
            using (BowlingGameContext context = new())
            {
                int runningScore = 0;
                var game = context.Games.Where(g => g.GameID == gameID).Include(g => g.Frames.Where(f => f.isFinished)).ThenInclude(f => f.scores).FirstOrDefault();
                var frames = game.Frames.OrderBy(f => f.frameNumber).ToList();

                for (int frameIndx = 0; frameIndx < frames.Count; frameIndx++)
                {
                    int framescoreIndex = frameIndx;
                    List<Score> futureScores = new List<Score>();
                    var currentFrame = game.Frames[frameIndx];
                    var frameScore = game.Frames[frameIndx].scores.Select(s => s.scoreNumber).Sum();
                    var futureFrames = frames.Where(f => f.frameNumber > currentFrame.frameNumber).OrderBy(f => f.frameNumber).ToList();
                    foreach(var frame in futureFrames)
                    {
                        futureScores.AddRange(frame.scores.OrderBy(s => s.scorePosition));
                    }
                    if(currentFrame.frameNumber != 10)
                    {
                        if (frameScore < 10)
                        {
                            runningScore += frameScore;
                            currentFrame.frameScore = runningScore;
                            currentFrame.isScored = true;
                        }
                        else if (frameScore == 10)
                        {
                            if (currentFrame.scores.Count == 1 && futureScores.Count >= 2)
                            {
                                runningScore += 10 + futureScores.Take(2).Select(s => s.scoreNumber).Sum();
                                currentFrame.frameScore = runningScore;
                                currentFrame.isScored = true;
                            }
                            else if (currentFrame.scores.Count == 2 && futureScores.Count >= 1)
                            {
                                runningScore += 10 + futureScores.Take(1).Select(s => s.scoreNumber).Sum();
                                currentFrame.frameScore = runningScore;
                                currentFrame.isScored = true;
                            }


                        }
                    }
                    else 
                    {

                        runningScore += currentFrame.scores.Select(s => s.scoreNumber).Sum();
                        currentFrame.frameScore = runningScore;
                        currentFrame.isScored = true;
                        game.isFinished = true;
                    }
                    



                }
                context.SaveChanges();
            }
        }
        public void addFrameToGame(int gameID, int score)
        {
            using(BowlingGameContext context = new())
            {
                int frameNumber = context.Frames.Where(f => f.GameID == gameID).Count() +1;
                Frame newFrame = new Frame
                {
                    GameID = gameID,
                    frameNumber = frameNumber,

                };
                context.Frames.Add(newFrame);
                context.SaveChanges();
                addScoreToFrame(newFrame.FrameID, frameNumber, score);
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