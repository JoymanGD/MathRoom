using System;
using System.Linq;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MathRoom.Helpers.PathFinding;
using MonoGame.Extended.Tweening;

namespace MathRoom.Scenes
{
    public class AStar : IScene
    {
        GraphicsDevice GraphicsDevice;
        GraphicsDeviceManager Graphics;
        Tweener Tweener;
        const int Width = 30;
        const int Height = 30;
        int Resolution;
        Player CurrentPlayer;
        int[,] FrontendField;
        BoundingRectangle[,] BackendField;
        Point Offset;
        List<Point> Path;

        public AStar(string _name, int _id) : base(_name, _id){}

        public override void Initialize()
        {
            AdditionalInfo = "Move to point: LMB";
            GraphicsDevice = MathRoom.Instance.GraphicsDevice;
            Graphics = MathRoom.Instance.Graphics;
            Tweener = new Tweener();

            Resolution = Graphics.PreferredBackBufferHeight/ (int)(Height*1.1f);
            Offset = new Point(Graphics.PreferredBackBufferWidth/2 - Width*Resolution/2, Graphics.PreferredBackBufferHeight/2 - Height*Resolution/2);
            
            CurrentPlayer = new Player(Point.Zero, 12);

            FrontendField = new int[Width, Height];
            BackendField = new BoundingRectangle[Width, Height];
            Path = new List<Point>();
            
            FillFrontendField(FrontendField);
            FillBackendField(BackendField);
            SetPlayer(FrontendField, CurrentPlayer);
            
            base.Initialize();
        }

        public override void Update(GameTime _gameTime)
        {
            Tweener.Update(_gameTime.GetElapsedSeconds());
            UpdateField(BackendField, FrontendField, CurrentPlayer);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
                DrawFrontendField(FrontendField, _spriteBatch);
                DrawBackendField(BackendField, FrontendField, _spriteBatch);
                DrawPath(Path, _spriteBatch);
                DrawPlayer(CurrentPlayer, _spriteBatch);
            _spriteBatch.End();
        }

        public override void Reset()
        {
            FillFrontendField(FrontendField);
            Tweener.CancelAll();
            SetPlayer(FrontendField, CurrentPlayer);
            Path.Clear();
        }

        private void UpdateField(BoundingRectangle[,] _backendField, int[,] _frontendField, Player _player){
            var mouseState = MouseExtended.GetState();
            if(mouseState.WasButtonJustUp(MouseButton.Left)){
                for (var i = 0; i < Width; i++)
                {
                    for (var j = 0; j < Height; j++)
                    {
                        if(_frontendField[i, j] == 0) continue;

                        var rect = _backendField[i, j];

                        if(rect.Contains(mouseState.Position)){
                            Path.Clear();
                            Path = PathFinder.FindPath(_frontendField, _player.Position.ToPoint(), new Point(i, j));
                            MoveAlongThePath(_player, Path, Tweener);
                            return;
                        }
                    }
                }
            }
        }

        private void MoveAlongThePath(Player _player, List<Point> _path, Tweener _tweener){
            _tweener.CancelAll();
            MoveToNextPoint(_player, _path, _tweener);
        }

        private void MoveToNextPoint(Player _player, List<Point> _path, Tweener _tweener){
            if(_path.Count > 1){
                _path.RemoveAt(0);
                Vector2 first = _path.First().ToVector2();
                _tweener.TweenTo(_player,  p=> p.Position, first, 2/_player.Speed).Easing(EasingFunctions.Linear).OnEnd((tween)=> { MoveToNextPoint(_player, _path, _tweener); });
            }
        }

        private void DrawFrontendField(int[,] _frontendField, SpriteBatch _spriteBatch){
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    var offsetPosition = GetOffsetPosition(i, j);

                    //Draw cells
                    if(_frontendField[i,j] > 0){
                        _spriteBatch.DrawPoint(offsetPosition.ToVector2(), Color.White, Resolution);
                    }
                    else{
                        _spriteBatch.DrawPoint(offsetPosition.ToVector2(), Color.IndianRed, Resolution);
                    }
                }
            }
        }

        private void DrawBackendField(BoundingRectangle[,] _backendField, int[,] _frontendField, SpriteBatch _spriteBatch){
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    var rect = _backendField[i, j];
                    var fieldValue = _frontendField[i, j];

                    if(rect.Contains(MouseExtended.GetState().Position) && fieldValue == 1)
                        _spriteBatch.DrawRectangle(rect, Color.Gray, Resolution);

                    //Draw cells strokes
                    _spriteBatch.DrawRectangle(rect, Color.Black, .5f);
                }
            }
        }

        private void DrawPlayer(Player _player, SpriteBatch _spriteBatch){
            _spriteBatch.DrawPoint(_player.Position.X*Resolution + Offset.X, _player.Position.Y*Resolution + Offset.Y, Color.Blue, Resolution);
        }

        private void DrawPath(List<Point> _path, SpriteBatch _spriteBatch){
            foreach (var point in _path)
            {
                _spriteBatch.DrawPoint(point.X*Resolution + Offset.X, point.Y*Resolution + Offset.Y, Color.Green, Resolution/2);
            }
        }
        
        private void FillFrontendField(int[,] _frontendField){
            List<int> matchingWalls = new List<int>();
            Random random = new Random();

            //fill every cell with 1
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    _frontendField[i,j] = 1;
                }
            }

            //settle walls
            for (var i = 1; i < Width-1; i++)
            {
                if(i%3==0) continue;

                for (var j = 1; j < Height-1; j++)
                {
                    if(j%5==0) continue;

                    if(matchingWalls.Count > 4){
                        matchingWalls.Clear();
                        continue;
                    }

                    var randomInt = random.Next(2);
                    _frontendField[i,j] = randomInt;

                    if(randomInt > 0){
                        matchingWalls.Clear();
                    }
                    else{
                        matchingWalls.Add(randomInt);
                    }
                }
            }
        }

        private void FillBackendField(BoundingRectangle[,] _backendField)
        {
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    var offsetPosition = GetOffsetPosition(i, j);
                    _backendField[i, j] = new BoundingRectangle(offsetPosition, new Size2(Resolution/2, Resolution/2));
                }
            }
        }

        private void SetPlayer(int[,] _frontendField, Player _player){
            bool positionFound = false;
            Random random = new Random();

            while(!positionFound){
                var x = random.Next(0, Width);
                var y = random.Next(0, Height);
                var result = _frontendField[x,y];

                if(result > 0){
                    _player.Move(new Point(x,y));
                    positionFound = true;
                }
            }
        }

        private Point GetOffsetPosition(Point _position){
            return new Point(_position.X * Resolution, _position.Y * Resolution) + Offset;
        }

        private Point GetOffsetPosition(int _x, int _y){
            return new Point(_x * Resolution, _y * Resolution) + Offset;
        }

        class Player{
            public Vector2 Position { get; set; }
            public float Speed { get; private set; }

            public Player(Point _position, float _speed){
                Position = _position.ToVector2();
                Speed = _speed;
            }

            public void Move(Point _newPosition){
                Position = _newPosition.ToVector2();
            }
        }
    }
}