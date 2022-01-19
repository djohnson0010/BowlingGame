namespace BowlingGame.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


        public class BowlingGameContext : DbContext
        {
            public DbSet<Game> Games { get; set; }
            public DbSet<Frame> Frames { get; set; }

            public string DbPath { get; }

            public BowlingGameContext()
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                DbPath = System.IO.Path.Join(path, "BowlingGame.db");
            }

            // The following configures EF to create a Sqlite database file in the
            // special "local" folder for your platform.
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseSqlite($"Data Source={DbPath}");
        }

        public class Game
        {
            public int GameID { get; set; }
            public string playerName { get; set; }

            public List<Frame> Frames { get; } = new();
        }

        public class Frame
        {
            public int FrameID { get; set; }
            public int score { get; set; }

            public int GameID { get; set; }
            public Game Game { get; set; }
        }

}
