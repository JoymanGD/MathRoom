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
        public int MaxFlockmatesCount = 100;
        public float MaxDistance = 50;
        public float LinearDrag = 10;

        private Flock Flock;
        private Flockmate Leader;
        private Vector2 MousePosition;
        private bool leftMouseDown = false;

        public override void Initialize()
        {
            var center = new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);
            
            Leader = new Flockmate(center, Vector2.Zero, Color.Cyan, true);
            
            Flock = new Flock(MaxFlockmatesCount, MaxDistance, Leader.Position, LinearDrag);

            Flock.Flockmates.Add(Leader);
            
            base.Initialize();
        }

        public override void Update(GameTime _gameTime)
        {
            var deltaTime = (float)_gameTime.ElapsedGameTime.TotalSeconds;

            Flock.Update(deltaTime);

            var mouseState = MouseExtended.GetState();

            leftMouseDown = mouseState.IsButtonDown(MouseButton.Left);

            MousePosition = mouseState.Position.ToVector2();

            if(leftMouseDown){
                Leader.MoveTowards(GetDirection(Leader.Position, MousePosition));
            }
        }
        
        public override void Draw(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            foreach (var item in Flock.Flockmates)
            {
                _spriteBatch.DrawCircle(item.Position, 5, 10, item.Color, 5);

            }

            #region Pointer
            Color pointerColor = Color.Aqua;
            
            if(leftMouseDown) pointerColor = Color.OrangeRed;

            var start = Leader.Position;
            var end = MousePosition;
            while (Vector2.Distance(start, end) > 5){
                _spriteBatch.DrawPoint(start, pointerColor, 3);
                start = Vector2.Lerp(start, end, 0.1f);
            }
            #endregion

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
    public List<Flockmate> Flockmates { get; private set; }
    public float MaxDistance { get; private set; }
    public float LinearDrag { get; private set; }
    public static float MaxForce = 40;

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
    
    public Flock(float maxFlockmates, float maxDistance, Vector2 leaderPosition, float linearDrag = 0)
    {
        var flockmates = new List<Flockmate>();
        var random = new Random();

        for (int i = 0; i < maxFlockmates; i++)
        {
            var randomPosition = new Vector2(leaderPosition.X + random.NextSingle(-200, 200), leaderPosition.Y + random.NextSingle(-200, 200));
            var newFlockmate = new Flockmate(randomPosition, Vector2.Zero, Color.White);
            flockmates.Add(newFlockmate);
        }

        Flockmates = flockmates;
        MaxDistance = maxDistance;
        LinearDrag = linearDrag;
    }

    public void Update(float deltaTime){
        foreach (var item in Flockmates)
        {
            item.Update(this, deltaTime, LinearDrag);
        }
    }

}

public class Flockmate{
    public Vector2 Position { get; private set; }
    public Vector2 Acceleration { get; private set; }
    public Vector2 Velocity { get; private set; }
    public bool Leader { get; private set; }
    public float Speed { get; private set; }
    public Color Color { get; private set; }

    public Flockmate(Vector2 position, Vector2 Velocity, Color color, bool leader = false, float speed = 1){
        Color = color;
        Position = position;
        this.Velocity = Velocity;
        Leader = leader;
        Acceleration = Vector2.Zero;
        Speed = speed;
    }

    public Flockmate(){}

    public void Move(Vector2 position){
        Position = position;
    }
    
    public void MoveTowards(Vector2 direction){
        Acceleration = direction * Speed;
    }

    public void Update(Flock flock, float deltaTime, float linearDrag){
        // if(Leader) return;
        
        Position += Velocity;
        Velocity += Acceleration * deltaTime * Speed;
        
        if(Acceleration.Length() > 0) Acceleration = Vector2.Lerp(Acceleration, Vector2.Zero, linearDrag * deltaTime);
        if(Velocity.Length() > 0) Velocity = Vector2.Lerp(Velocity, Vector2.Zero, linearDrag * deltaTime * deltaTime);

        if(!Leader) Flocking(flock, deltaTime);
    }

    public void Flocking(Flock flock, float deltaTime){
        Acceleration = Vector2.Zero;
        
        Acceleration += Align(flock);
        Acceleration += Cohession(flock);
        Acceleration += Separation(flock);
    }

    private bool Intersects(Flockmate other, float distance){
        if(other == this) return false;

        if(Vector2.Distance(other.Position, Position) < distance) return true;

        return false;
    }

    private Vector2 Align(Flock flock){
        var flockmates = flock.Flockmates;

        int intersectingsCount = 0;

        var aligning = new Vector2();
        foreach (var item in flockmates)
        {
            if(Intersects(item, flock.MaxDistance)){
                aligning += item.Velocity;
                intersectingsCount++;
            }
        }

        if(intersectingsCount > 0){
            aligning /= intersectingsCount;
            aligning -= Velocity;
            aligning *= Flock.MaxForce;
        }

        return aligning;
    }
    
    private Vector2 Cohession(Flock flock){
        var flockmates = flock.Flockmates;

        int intersectingsCount = 0;

        var cohession = new Vector2();

        foreach (var item in flockmates)
        {
            var maxDistance = item.Leader ? flock.MaxDistance * 2 : flock.MaxDistance;
            if(Intersects(item, maxDistance)){
                cohession += item.Position;
                intersectingsCount++;
                
                if(item.Leader){
                    cohession += (item.Position - Position) * Flock.MaxForce;
                } 
            }
        }

        if(intersectingsCount > 0){
            cohession /= intersectingsCount;
            cohession -= Position;
            cohession -= Velocity;

            cohession = Vector2.Clamp(cohession, new Vector2(-Flock.MaxForce, -Flock.MaxForce), new Vector2(Flock.MaxForce, Flock.MaxForce));
        }

        return cohession;
    }

    private Vector2 Separation(Flock flock){
        var flockmates = flock.Flockmates;

        int intersectingsCount = 0;

        var separation = new Vector2();

        foreach (var item in flockmates)
        {
            if(Intersects(item, flock.MaxDistance)){
                var distance = Vector2.Distance(item.Position, Position);
                var antiVector = Position - item.Position;
                antiVector /= distance;
                separation += antiVector;
                intersectingsCount++;
            }
        }

        if(intersectingsCount > 0){
            separation /= intersectingsCount;
            separation -= Velocity;
            separation *= Flock.MaxForce * 1.2f;
        }

        return separation;
    }

    public override bool Equals(object obj) => obj is Flockmate other;

    public override int GetHashCode() => (Position, Velocity).GetHashCode();

    public static bool operator ==(Flockmate a, Flockmate b) => a.Position == b.Position && a.Velocity == b.Velocity;

    public static bool operator !=(Flockmate a, Flockmate b) => a.Position != b.Position || a.Velocity != b.Velocity;
}