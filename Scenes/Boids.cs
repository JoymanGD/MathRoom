using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace MathRoom.Scenes
{
    public class Boids : IScene
    {
        public int MaxFlockmatesCount = 30;
        public float MaxDistance = 100;

        private Flock Flock;
        private Flockmate Leader;

        public override void Initialize()
        {
            var center = new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);
            
            Leader = new Flockmate(center, Vector2.Zero, true);
            
            Flock = new Flock(MaxFlockmatesCount, MaxDistance, Leader.Position);

            Flock.Flockmates.Add(Leader);
            
            base.Initialize();
        }

        public override void Update(GameTime _gameTime)
        {
            foreach (var item in Flock.Flockmates)
            {
                if(item.Leader) continue;

                item.Flocking(Flock);
            }

            Flock.Update(_gameTime);

            var mousePosition = MouseExtended.GetState().Position.ToVector2();

            Leader.MoveTowards(GetDirection(Leader.Position, mousePosition));
        }
        
        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            foreach (var item in Flock.Flockmates)
            {
                _spriteBatch.DrawCircle(item.Position, 5, 5, Color.White, 5);
            }
            _spriteBatch.End();
        }

        public override void Reset()
        {
            
        }

        private Vector2 GetDirection(Vector2 from, Vector2 to){
            var direction = to - from;
            direction.Normalize();

            return direction;
        }
    }
}

public class Flock{
    public List<Flockmate> Flockmates;
    public float MaxDistance;

    public Flock(List<Flockmate> flockmates, float maxDistance)
    {
        Flockmates = flockmates;
        MaxDistance = maxDistance;
    }
    
    public Flock(float maxFlockmates, float maxDistance)
    {
        var flockmates = new List<Flockmate>();

        for (int i = 0; i < maxFlockmates; i++)
        {
            var newFlockmate = new Flockmate();
            flockmates.Add(newFlockmate);
        }

        Flockmates = flockmates;
        MaxDistance = maxDistance;
    }
    
    public Flock(float maxFlockmates, float maxDistance, Vector2 leaderPosition)
    {
        var flockmates = new List<Flockmate>();
        var random = new Random();

        for (int i = 0; i < maxFlockmates; i++)
        {
            var randomPosition = new Vector2(leaderPosition.X + random.NextSingle(-200, 200), leaderPosition.Y + random.NextSingle(-200, 200));
            var newFlockmate = new Flockmate(randomPosition, Vector2.Zero);
            flockmates.Add(newFlockmate);
        }

        Flockmates = flockmates;
        MaxDistance = maxDistance;
    }

    public void Update(GameTime gameTime){
        foreach (var item in Flockmates)
        {
            item.MoveTowards(item.Direction * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }

}

public class Flockmate{
    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; private set; }
    public bool Leader { get; private set; }

    public Flockmate(Vector2 position, Vector2 direction, bool leader = false){
        Position = position;
        Direction = direction;
        Leader = leader;
    }

    public Flockmate(){}

    public void Move(Vector2 position){
        Position = position;
    }
    
    public void MoveTowards(Vector2 direction){
        Position += direction;
    }

    public void Flocking(Flock flock){
        var flocking = new Vector2();
        
        flocking += Steering(flock);
        //flocking += Avoiding(flock);
        flocking += Cohessing(flock);
        //flocking /= 3;

        Direction = flocking;
        Direction.Normalize();
    }

    private bool Intersects(Flockmate other, float distance){
        if(other == this) return false;

        if(Vector2.Distance(other.Position, Position) < distance) return true;

        return false;
    }

    private Vector2 Steering(Flock flock){
        var flockmates = flock.Flockmates;

        int intersectingsCount = 0;

        var steering = new Vector2();
        foreach (var item in flockmates)
        {
            if(Intersects(item, flock.MaxDistance)){
                steering += item.Direction;
                intersectingsCount++;
            }
        }

        if(intersectingsCount > 0){
            steering /= intersectingsCount;
            steering -= Direction;
        }

        return steering;
    }

    private Vector2 Avoiding(Flock flock){
        var flockmates = flock.Flockmates;

        int intersectingsCount = 0;

        var avoiding = new Vector2();
        var averagePos = new Vector2();

        foreach (var item in flockmates)
        {
            if(Intersects(item, flock.MaxDistance)){
                averagePos += item.Position;
                intersectingsCount++;
            }
        }

        if(intersectingsCount > 0){
            averagePos /= intersectingsCount;
            avoiding = Position - averagePos;
        }

        return avoiding;
    }
    
    private Vector2 Cohessing(Flock flock){
        var flockmates = flock.Flockmates;

        int intersectingsCount = 0;

        var cohessing = new Vector2();
        var averagePos = new Vector2();

        foreach (var item in flockmates)
        {
            if(Intersects(item, flock.MaxDistance)){
                averagePos += item.Position;
                intersectingsCount++;
            }
        }

        if(intersectingsCount > 0){
            averagePos /= intersectingsCount;
            cohessing = averagePos - Position;
        }

        return cohessing;
    }

    public override bool Equals(object obj) => obj is Flockmate other;

    public override int GetHashCode() => (Position, Direction).GetHashCode();

    public static bool operator ==(Flockmate a, Flockmate b) => a.Position == b.Position && a.Direction == b.Direction;

    public static bool operator !=(Flockmate a, Flockmate b) => a.Position != b.Position || a.Direction != b.Direction;
}