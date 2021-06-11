using System;
using System.Collections.Generic;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MathRoom.Helpers.Structs;
using System.Collections.ObjectModel;
using System.Linq;

namespace MathRoom.Scenes
{
    public class AStar : IScene
    {
        GraphicsDevice GraphicsDevice;

        public AStar(string _name, int _id) : base(_name, _id){}

        public override void Initialize()
        {
            GraphicsDevice = MathRoom.Instance.GraphicsDevice;
            
            base.Initialize();
        }

        public override void Update(GameTime _gameTime)
        {
            
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            _spriteBatch.End();
        }

        public override void Reset()
        {
            
        }
    }

    public static class PathFinder{
        class PathNode{
            public Point Position { get; private set; }
            public int G { get; private set; } //distance to start
            public int H { get; private set; } //distance to aim
            public int F { get { return G + H; }} //from start to aim
            public PathNode CameFrom { get; private set; }

            public PathNode(Point _position, int _g, int _h, PathNode _cameFrom){
                Position = _position;
                G = _g;
                H = _h;
                CameFrom = _cameFrom;
            }

            public void SetG(int _g){
                G = _g;
            }

            public void SetCameFrom(PathNode _cameFrom){
                CameFrom = _cameFrom;
            }
        }

        public static List<Point> FindPath(int[,] _field, Point _start, Point _goal){
            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();
            
            PathNode startNode = new PathNode(_start, 0, GetPointsPathLength(_start, _goal), null);
            
            openSet.Add(startNode);
            
            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node => node.F).First();

                if (currentNode.Position == _goal)
                    return GetPathToNode(currentNode);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                
                foreach (var neighbourNode in GetNeighbours(currentNode, _goal, _field))
                {
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;
                    
                    var openNode = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);
                    
                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else if (openNode.G > neighbourNode.G)
                    {
                        openNode.SetCameFrom(currentNode);
                        openNode.SetG(neighbourNode.G);
                    }
                }
            }
            return null;
        }

        private static Collection<PathNode> GetNeighbours(PathNode _node, Point _goal, int[,] _field){
            Collection<PathNode> neighbours = new Collection<PathNode>();
            
            Point[] neighboursPoints = new Point[]{
                new Point(_node.Position.X + 1, _node.Position.Y + 1),
                new Point(_node.Position.X + 1, _node.Position.Y - 1),
                new Point(_node.Position.X - 1, _node.Position.Y + 1),
                new Point(_node.Position.X - 1, _node.Position.Y - 1)
            };

            foreach (var point in neighboursPoints)
            {
                if (point.X < 0 || point.X >= _field.GetLength(0))
                    continue;
                if (point.Y < 0 || point.Y >= _field.GetLength(1))
                    continue;
                if ((_field[point.X, point.Y] != 0) && (_field[point.X, point.Y] != 1))
                    continue;

                var neighbourNode = new PathNode(point, _node.G + GetPathToNeighbourLength(), GetPointsPathLength(point, _goal), _node);

                neighbours.Add(neighbourNode);
            }

            return neighbours;
        }

        private static int GetPointsPathLength(Point _from, Point _to){
            return _from.X - _to.X + _from.Y + _to.Y;
        }

        private static List<Point> GetPathToNode(PathNode _node){
            List<Point> finalPath = new List<Point>();
            var currentNode = _node;

            while(currentNode != null){
                finalPath.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }

            finalPath.Reverse();

            return finalPath;
        }

        private static int GetPathToNeighbourLength(){
            return 1;
        }
    }
}