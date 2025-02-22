﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Threading;

namespace EscapeFromPrison
{
    public partial class GameWindow : Window
    {
        private int _mapSize;
        private string _difficulty;
        private Random _random = new Random();
        private int _playerX;
        private int _playerY;
        private List<Entity> _entities;
        private Entity _exit;
        private Entity _winTile;
        private DispatcherTimer _guardMoveTimer;

        public GameWindow(int mapSize, string difficulty)
        {
            InitializeComponent();
            _mapSize = mapSize;
            _difficulty = difficulty;
            InitializeGame();
            StartGuardMoveTimer();
        }

        private void InitializeGame()
        {
            _entities = new List<Entity>();
            for (int i = 0; i < _mapSize; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            InitializeEntities();
            DrawEntities();
        }

        private void InitializeEntities()
        {
            _playerX = 0;
            _playerY = 0;
            _entities.Add(new Entity { X = _playerX, Y = _playerY, Fill = Brushes.Blue, Role = "Player" });

            int numGuards = _difficulty == "Easy" ? 1 : _difficulty == "Normal" ? 2 : 3;
            for (int i = 0; i < numGuards; i++)
            {
                int guardX = _random.Next(1, _mapSize);
                int guardY = _random.Next(1, _mapSize);
                _entities.Add(new Entity { X = guardX, Y = guardY, Fill = Brushes.Red, Role = "Guard" });
            }

            _exit = new Entity { X = _mapSize - 1, Y = _mapSize - 1, Fill = Brushes.Green, Role = "Exit" };
            _entities.Add(_exit);

            _winTile = new Entity { X = _mapSize / 2, Y = _mapSize / 2, Fill = Brushes.Gray, Role = "WinTile" };
            _entities.Add(_winTile);
        }

        private void DrawEntities()
        {
            GameGrid.Children.Clear();
            double tileSize = this.ActualWidth / _mapSize;

            for (int row = 0; row < _mapSize; row++)
            {
                for (int col = 0; col < _mapSize; col++)
                {
                    var border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Background = Brushes.Gray,
                        Width = tileSize,
                        Height = tileSize
                    };
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);
                    GameGrid.Children.Add(border);
                }
            }

            foreach (var entity in _entities)
            {
                var ellipse = new Ellipse
                {
                    Fill = entity.Fill,
                    Width = 0.8 * (this.ActualWidth / _mapSize),
                    Height = 0.8 * (this.ActualWidth / _mapSize)
                };

                Grid.SetRow(ellipse, entity.Y);
                Grid.SetColumn(ellipse, entity.X);
                GameGrid.Children.Add(ellipse);
            }
        }

        private void MovePlayer(string direction)
        {
            if (_exit == null || !IsPlayerAlive()) return;
            switch (direction)
            {
                case "Up":
                    if (_playerY > 0) _playerY--;
                    break;
                case "Down":
                    if (_playerY < _mapSize - 1) _playerY++;
                    break;
                case "Left":
                    if (_playerX > 0) _playerX--;
                    break;
                case "Right":
                    if (_playerX < _mapSize - 1) _playerX++;
                    break;
            }

            UpdateEntities();

            if (_playerX == _exit.X && _playerY == _exit.Y)
            {
                MessageBox.Show("You escaped the prison!");
                Close();
            }

            DrawEntities();
        }

        private void UpdateEntities()
        {
            var player = _entities.First(e => e.Role == "Player");
            player.X = _playerX;
            player.Y = _playerY;
        }

        private void MoveGuards()
        {
            foreach (var guard in _entities.Where(e => e.Role == "Guard"))
            {
                int direction = _random.Next(4);

                if (direction == 0 && guard.Y > 0) guard.Y--;
                else if (direction == 0) guard.Y++;
                if (direction == 1 && guard.Y < _mapSize - 1) guard.Y++;
                else if (direction == 1) guard.Y--;
                if (direction == 2 && guard.X > 0) guard.X--;
                else if (direction == 2) guard.X++;
                if (direction == 3 && guard.X < _mapSize - 1) guard.X++;
                else if (direction == 3) guard.X--;
            }
            CheckForCollisions();
        }

        private void CheckForCollisions()
        {
            var player = _entities.FirstOrDefault(e => e.Role == "Player");
            if (player == null) return;

            var guard = _entities.FirstOrDefault(e => e.Role == "Guard" && e.X == player.X && e.Y == player.Y);
            if (guard != null)
            {
                MessageBox.Show("Game Over! The guard caught you.");
                Close();
            }
        }

        private bool IsPlayerAlive()
        {
            var player = _entities.First(e => e.Role == "Player");
            return !_entities.Any(e => e.Role == "Guard" && e.X == player.X && e.Y == player.Y);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_exit == null || !IsPlayerAlive()) return;
            switch (e.Key)
            {
                case Key.W:
                    MovePlayer("Up");
                    break;
                case Key.S:
                    MovePlayer("Down");
                    break;
                case Key.A:
                    MovePlayer("Left");
                    break;
                case Key.D:
                    MovePlayer("Right");
                    break;
            }
        }

        private void StartGuardMoveTimer()
        {
            _guardMoveTimer = new DispatcherTimer();
            _guardMoveTimer.Interval = TimeSpan.FromSeconds(1); // Set the interval as needed
            _guardMoveTimer.Tick += (s, e) => MoveGuards();
            _guardMoveTimer.Start();
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            _guardMoveTimer.Stop();
            InitializeGame();
            StartGuardMoveTimer();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Brush Fill { get; set; }
        public string Role { get; set; }
    }
}
