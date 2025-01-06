using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;

namespace EscapeFromPrison
{
    public class GameViewModel
    {
        public ObservableCollection<GameEntity> Entities { get; set; }

        private int _mapSize;
        private string _difficulty;
        private Random _random;

        public GameViewModel()
        {
            Entities = new ObservableCollection<GameEntity>();
            _random = new Random();
        }

        public void InitializeGame(int mapSize, string difficulty)
        {
            _mapSize = Math.Max(mapSize, 7); // Ensure minimum map size of 7x7
            _difficulty = difficulty;
            SetupEntities();
        }

        private void SetupEntities()
        {
            Entities.Clear();

            Entities.Add(new GameEntity
            {
                Fill = Brushes.Blue,
                X = 0,
                Y = 0,
                EntityType = EntityType.Player
            });

            Entities.Add(new GameEntity
            {
                Fill = Brushes.Green,
                X = _mapSize - 1,
                Y = _mapSize - 1,
                EntityType = EntityType.Exit
            });

            int numGuards = Math.Max(_difficulty == "Easy" ? 1 : _difficulty == "Normal" ? 2 : 3, 1); // Ensure at least one guard
            for (int i = 0; i < numGuards; i++)
            {
                Entities.Add(new GameEntity
                {
                    Fill = Brushes.Red,
                    X = _random.Next(1, _mapSize - 1),
                    Y = _random.Next(1, _mapSize - 1),
                    EntityType = EntityType.Guard
                });
            }
        }

        public void MovePlayer(string direction)
        {
            var player = Entities.FirstOrDefault(e => e.EntityType == EntityType.Player);
            if (player == null) return;
            switch (direction)
            {
                case "Up":
                    if (player.Y > 0) player.Y--;
                    break;
                case "Down":
                    if (player.Y < _mapSize - 1) player.Y++;
                    break;
                case "Left":
                    if (player.X > 0) player.X--;
                    break;
                case "Right":
                    if (player.X < _mapSize - 1) player.X++;
                    break;
            }

            CheckForCollisions();
        }

        private void CheckForCollisions()
        {
            var player = Entities.FirstOrDefault(e => e.EntityType == EntityType.Player);
            if (player == null) return;

            var exit = Entities.FirstOrDefault(e => e.EntityType == EntityType.Exit);
            if (player.X == exit?.X && player.Y == exit?.Y)
            {
                Console.WriteLine("You reached the exit! You Win!");
            }
            var guard = Entities.FirstOrDefault(e => e.EntityType == EntityType.Guard && e.X == player.X && e.Y == player.Y);
            if (guard != null)
            {
                Console.WriteLine("Game Over! The guard caught you.");
            }
        }

        public void MoveGuards()
        {
            foreach (var guard in Entities.Where(e => e.EntityType == EntityType.Guard))
            {
                int direction = _random.Next(0, 4);

                switch (direction)
                {
                    case 0: // Up
                        if (guard.Y > 0) guard.Y--;
                        else guard.Y++;
                        break;
                    case 1: // Down
                        if (guard.Y < _mapSize - 1) guard.Y++;
                        else guard.Y--;
                        break;
                    case 2: // Left
                        if (guard.X > 0) guard.X--;
                        else guard.X++;
                        break;
                    case 3: // Right
                        if (guard.X < _mapSize - 1) guard.X++;
                        else guard.X--;
                        break;
                }
            }
            CheckForCollisions();
        }
    }

    public class GameEntity
    {
        public Brush Fill { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public EntityType EntityType { get; set; }
    }

    public enum EntityType
    {
        Player,
        Guard,
        Exit
    }
}
