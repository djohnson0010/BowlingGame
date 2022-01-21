namespace BowlingGame.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


        public class BowlingGameContext : DbContext
        {
            public DbSet<Game> Games { get; set; }
            public DbSet<Frame> Frames { get; set; }
            public DbSet<Score> Scores { get; set; }

            public string DbPath { get; }

            public BowlingGameContext()
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                DbPath = System.IO.Path.Join(path, "BowlingGame.db");
            }
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseSqlite($"Data Source={DbPath}");
        }

        public class Game
        {
            public int GameID { get; set; }
            public string playerName { get; set; }
            public int? score { get; set; }
            public bool isFinished { get; set; }

            public List<Frame> Frames { get; } = new();
        }

        public class Frame
        {
            public int FrameID { get; set; }
            public int frameNumber { get; set; }
            public bool isFinished { get; set; }
            public bool isScored { get; set; }
            public int frameScore { get; set; }
        public List<Score> scores { get; } = new();
            public int GameID { get; set; }
            public Game Game { get; set; }
        }
        public class Score
        {
            public int scoreID { get; set; }
            public int scoreNumber { get; set; }
            public bool isSpare { get; set; }
            public bool isStrike { get; set; }
            public int scorePosition { get; set; }
            public int FrameID { get; set; }
            public Frame Frame { get; set; }
        }

}
