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
        //get list of bowling games
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
                    return game;
                }).ToList();
                return gameList;
            }
        }
        //create a bowling game
        [HttpPost]
        public dynamic CreateBowlingGame(string name)
        {
            if(name == null || name == "")
            {
                //send back message for user to fill in name
                return new { code = 0, message = "Please enter your name" };
            }
            using(BowlingGameContext context = new())
            {
                //create game of bowling
                Game game = new Game
                {
                    playerName = name
                };
                context.Games.Add(game);
                context.SaveChanges();
                //return success message to user with created game id
                return new { code = 1, gameID = game.GameID };
            }
           
        }
        [HttpPost]
        public dynamic getBowlingGame(int gameID)
        {
            int totalScore = 0;
            int bowlingPins = 10;
            using (BowlingGameContext context = new())
            {
                dynamic bowlingGame = new ExpandoObject();
                //get game information
                bowlingGame = context.Games.Where(g => g.GameID == gameID).Include(g => g.Frames).ThenInclude(g => g.scores).ToList().Select(x =>
                {
                    dynamic game = new ExpandoObject();
                    //bowling pin number for allowable pins
                    
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
                        //return empty string if frame hasn't been scored
                        frame.frameScore = f.isScored ? f.frameScore.ToString() : "";
                        totalScore += frameScore;
                        if(!f.isFinished)
                        {
                            bowlingPins = 10 - (frameScore % 10);
                        }
                        //check if frame is finished
                        frame.isFinished = f.isFinished;
                       
                        return frame;

                    }).ToList();
                    game.bowlingPins = bowlingPins;
                    return game;
                }).First();
                return bowlingGame;
            }
        }

        //add score to game
        [HttpPost]
        public void addScore(int score, int gameID)
        {
            using(BowlingGameContext context = new())
            {
                var game = context.Games.Where(g => g.GameID == gameID).Include(g => g.Frames).FirstOrDefault();
                //check if game exists and not finished
                if(game != null && !game.isFinished)
                {
                    var frames = game.Frames.OrderByDescending(f => f.frameNumber).ToList();
                    //Add frame if game has no frames or last frame has not been finished
                    if((frames.Count() == 0) || frames.First().isFinished)
                    {
                        addFrameToGame(gameID, score);
                    }
                    //Add score to unfinished frame
                    else
                    {

                        int frameID = frames.Select(f => f.FrameID).FirstOrDefault();
                        int frameNumber = frames.Select(f => f.frameNumber).First();
                        addScoreToFrame(frameID, frameNumber, score);
                    }
                }
                //calculate scores for scorecard
                calculateGameScores(gameID);
            }
        }
        public void addFrameToGame(int gameID, int score)
        {
            using (BowlingGameContext context = new())
            {
                int frameNumber = context.Frames.Where(f => f.GameID == gameID).Count() + 1;
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
        public void addScoreToFrame(int frameID, int frameNumber, int score)
        {
            using(BowlingGameContext context = new())
            {
                //get score and position from current frame
                var scores = context.Scores.Where(f => f.FrameID == frameID).ToList();
                int scorePosition = scores.Count() + 1;
                var frame = context.Frames.Where(f => f.FrameID == frameID).First();
                Score newScore = new Score
                {
                    FrameID = frameID,
                    scorePosition = scorePosition,
                    scoreNumber = score,
                    //mark as strike if score is 10
                    isStrike = score == 10,
                    //mark as spare if score + previous score total's 10
                    isSpare = scores.Where(s => !s.isStrike).Select(s => s.scoreNumber).Sum() + score == 10
                    
                    
                };

                context.Scores.Add(newScore);
                context.SaveChanges();
                //mark game and frames as finished if scores are complete in frame and last frame is filled with scores
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
        //calculate scores to game for scorecard
        public void calculateGameScores(int gameID)
        {
            using (BowlingGameContext context = new())
            {
                //variable to check for current score for scoreboard
                int runningScore = 0;
                //get game with frames and scores
                var game = context.Games.Where(g => g.GameID == gameID).Include(g => g.Frames.Where(f => f.isFinished)).ThenInclude(f => f.scores).FirstOrDefault();
                //get frames in order of frame number
                var frames = game.Frames.OrderBy(f => f.frameNumber).ToList();

                for (int frameIndx = 0; frameIndx < frames.Count; frameIndx++)
                {
                    //create variable to hold scores for calculating strike and spare frames
                    List<Score> futureScores = new List<Score>();

                    var currentFrame = game.Frames[frameIndx];
                    //get current frame score
                    var frameScore = game.Frames[frameIndx].scores.Select(s => s.scoreNumber).Sum();
                    //add up all scores from future frames for sprike and spare calculations
                    var futureFrames = frames.Where(f => f.frameNumber > currentFrame.frameNumber).OrderBy(f => f.frameNumber).ToList();
                    //add scores from game to a list
                    foreach (var frame in futureFrames)
                    {
                        futureScores.AddRange(frame.scores.OrderBy(s => s.scorePosition));
                    }
                    //check if frame is not 10 for strike and spare calculations
                    if (currentFrame.frameNumber != 10)
                    {
                        //check if score for frame is less than 10 and calculate by sum
                        if (frameScore < 10)
                        {
                            runningScore += frameScore;
                            currentFrame.frameScore = runningScore;
                            currentFrame.isScored = true;
                        }
                        else if (frameScore == 10)
                        {
                            //strike calculation
                            if (currentFrame.scores.Count == 1 && futureScores.Count >= 2)
                            {
                                //get first two scores after current frame for calculation
                                runningScore += 10 + futureScores.Take(2).Select(s => s.scoreNumber).Sum();
                                currentFrame.frameScore = runningScore;
                                currentFrame.isScored = true;
                            }
                            //spare calculation
                            else if (currentFrame.scores.Count == 2 && futureScores.Count >= 1)
                            {
                                //get first score after current frame for calculation
                                runningScore += 10 + futureScores.Take(1).Select(s => s.scoreNumber).Sum();

                                currentFrame.frameScore = runningScore;
                                currentFrame.isScored = true;
                            }
                        }
                    }
                    //calculate for tenth frame with adding all parts up
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
    }
}