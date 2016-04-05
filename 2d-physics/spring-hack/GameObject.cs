using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace physics2D
{
    public enum Direction
    {
        left,
        right,
        top,
        bottom,
    }


    /// <summary>
    /// <para>The parent class of all objects in the game</para>
    /// </summary>
    public class GameObject
    {
        #region variables
        protected const float MASS = 20.0f;
        protected const float RESTITUTION = 0.80f;

        protected Texture2D texture;
        protected Vector2 position;
        protected Color color;
        protected int width;
        protected int height;

        protected Vector2 previousPos;
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected Vector2 prevAcc;
        protected float rotation;
        protected Vector2 force;
        protected Vector2 center;
        #endregion

        #region constructor
        /// <param name="rectangle">Target Rectangle</param>
        public GameObject(Rectangle rectangle)
        {
            width = rectangle.Width;
            height = rectangle.Height;
            position = new Vector2(rectangle.X, rectangle.Y);
            previousPos = position;
            color = Color.White;

            velocity = new Vector2(0, 0);
            acceleration = new Vector2(0, 0);
            rotation = 0f;
            center = new Vector2(rectangle.X + width / 2, rectangle.Y + height / 2);
        }

        public GameObject(Rectangle rectangle, Vector2 initialVelocity)
        {
            width = rectangle.Width;
            height = rectangle.Height;
            position = new Vector2(rectangle.X, rectangle.Y);
            previousPos = position;
            color = Color.White;

            velocity = initialVelocity;
            acceleration = new Vector2(0, 0);
            rotation = 0f;
            center = new Vector2(rectangle.X + width / 2, rectangle.Y + height / 2);
        }

        #endregion

        #region properties
        public Vector2 Center
        {
            get { return center; }
        }

        /// <summary>
        /// The X position of upper left corner
        /// </summary>
        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }
        
        /// <summary>
        /// The Y position of the upper left corner
        /// </summary>
        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Vector2 Position
        {
             get { return position; }
        }
        /// <summary>
        /// The rectangle
        /// </summary>
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)X,(int)Y,width,height); }
        }

        /// <summary>
        /// The current texture
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// The point that represents the top-left corner of the object's AABB
        /// </summary>
        public Point Min
        {
            get { return position.ToPoint(); }
        }

        /// <summary>
        /// The point that represents the bottom-right corner of the object's AABB
        /// </summary>
        public Point Max
        {
            get { return new Point((int)position.X + width, (int)position.Y + height); }
        }

        /// <summary>
        /// The width of the object
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// The height of the object
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 PrevPos
        {
            get { return previousPos; }
        }

        public float Mass
        {
            get { return MASS; }
        }

        public float Restitution
        {
            get { return RESTITUTION; }
        }

        public Vector2 TopRight
        {
            get { return new Vector2(position.X + width, position.Y); }
        }

        public Vector2 BottomLeft
        {
            get { return new Vector2(position.X, position.Y + height); }
        }

        public Vector2 Force
        {
            get { return force;}
            set { force = value; }
        }
        #endregion

        #region methods

        /// <summary>
        /// Moves the character based on the previous position, velocity, time step, and acceleration
        /// </summary>
        /// <param name="deltaTime">The time step from the previous update call</param>
        public virtual void UpdatePos(double deltaTime)
        {
            previousPos = position;
            position.X += (float)(velocity.X * deltaTime + (0.5 * prevAcc.X * Math.Pow(deltaTime, 2.0)));
            position.Y += (float)(velocity.Y * deltaTime + (0.5 * prevAcc.Y * Math.Pow(deltaTime, 2.0)));
            prevAcc = acceleration;
        }

        /// <summary>
        /// Updates the velocity of a character based on the average acceleration and time step
        /// </summary>
        /// <param name="deltaTime">Time step in miliseconds</param>
        public void UpdateVelocity(double deltaTime)
        {
            velocity.Y += (float)(((prevAcc.Y + acceleration.Y) * deltaTime) / 2);
            velocity.X += (float)(((prevAcc.X + acceleration.X) * deltaTime) / 2);
        }

        public void UpdateAcc()
        {
            acceleration = force / MASS;
        }

        public void AddForce(Vector2 newForce)
        {
            force += newForce;
        }

        public bool CheckCollision(GameObject obj)
        {

            if (X < obj.Max.X && Max.X > obj.X && Y < obj.Max.Y && Max.Y > obj.Y)
            {
                if (!(obj is Floor))
                    Console.WriteLine("Collided!" + X);
                return true;

            }
            else return false;
        }

        public Vector2 GetNormalUnitVector(Direction direction)
        {
            Vector2 normalVector = Vector2.Zero;
            
            //doesn't deal with rotated boxes
            //still not implemented so fuck you future me
            //yeah fuck you too past me
            //fuck exact edge to edge collision
            //that will never happen because I say so
            //yeah fuck you too

            switch (direction)
            {
                case Direction.left:
                    normalVector = new Vector2(position.X, position.Y + height) - position;
                    break;
                case Direction.right: 
                    normalVector = new Vector2(position.X + width, position.Y) - Max.ToVector2();
                    break;
                case Direction.top: 
                    normalVector = position - new Vector2(position.X + width, position.Y);
                    break;
                case Direction.bottom:
                    normalVector = Max.ToVector2() - new Vector2(position.X, position.Y + height);
                    break;
            }

            normalVector = new Vector2(-normalVector.Y, normalVector.X);
            normalVector = normalVector / (normalVector.Length());

            return normalVector;
        }

        /// <summary>
        /// Basic draw method, doesn't draw if texture is not instantiated. 
        /// Default color is white.
        /// </summary>
        /// <param name="spriteBatch">The current spriteBatch</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, position, new Rectangle(0, 0, width, height), color, rotation, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            else throw new Exception();
        }

        public override string ToString()
        {
            string msg = "";
            msg += "Position: (" + X + ", " + Y + ")\n";
            msg += "Velocity: (" + velocity.X + ", " + velocity.Y + ")\n";
            msg += "Acceleration: (" + acceleration.X + ", " + acceleration.Y + ")";
            return msg;
        }
        #endregion
    }
}
