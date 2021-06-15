using Microsoft.Xna.Framework;

namespace MathRoom.Helpers.PathFinding
{
    public class PathNode{
        public Point Position { get; private set; }
        public int G { get; private set; } //distance to start
        public int H { get; private set; } //distance to aim
        public int F { get { return (G + H) / 2; }} //from start to aim
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
}