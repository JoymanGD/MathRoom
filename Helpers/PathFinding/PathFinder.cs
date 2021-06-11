using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MathRoom.Helpers.PathFinding
{
    public static class PathFinder{
        public static List<Point> FindPath(int[,] _frontendField, Point _start, Point _goal){
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
                
                foreach (var neighbourNode in GetNeighbours(currentNode, _goal, _frontendField))
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

        private static Collection<PathNode> GetNeighbours(PathNode _node, Point _goal, int[,] _frontendField){
            Collection<PathNode> neighbours = new Collection<PathNode>();
            
            Point[] neighboursPoints = new Point[]{
                new Point(_node.Position.X + 1, _node.Position.Y),
                new Point(_node.Position.X - 1, _node.Position.Y),
                new Point(_node.Position.X, _node.Position.Y + 1),
                new Point(_node.Position.X, _node.Position.Y - 1)
            };

            foreach (var point in neighboursPoints)
            {
                if (point.X < 0 || point.X >= _frontendField.GetLength(0))
                    continue;
                if (point.Y < 0 || point.Y >= _frontendField.GetLength(1))
                    continue;
                if (_frontendField[point.X, point.Y] != 1)
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