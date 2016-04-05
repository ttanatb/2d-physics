using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace physics2D
{
    public class PhysicsManager
    {
        InputManager input;
        Random random;
        //Camera camera;
        //GameObject cameraNull;

        SpriteFont font;
        List<GameObject> objects;
        List<Floor> floors;
        Texture2D objTexture;

        const float GRAVITY = 200f;

        public PhysicsManager(Viewport viewPort)
        {
            random = new Random();
            input = new InputManager();
            floors = new List<Floor>();
            objects = new List<GameObject>();

            floors.Add(new Floor(new Rectangle(2, 700, 900, 54)));
            //cameraNull = new GameObject(new Rectangle(viewPort.Width/2 - 1, viewPort.Height/2 - 1, 3, 3));
            //camera = new Camera(viewPort);

        }

        //public Camera Camera
        //{
        //get { return camera; }
        //}

        public void LoadContent(ContentManager content, string objTexture, string testFont)
        {
            this.objTexture = content.Load<Texture2D>(objTexture);
            //cameraNull.Texture = this.objTexture;
            font = content.Load<SpriteFont>(testFont);
            floors[0].Texture = this.objTexture;

        }

        public void Update(double deltaTime)
        {
            Vector2 iniVelo = new Vector2(0, 0);
            if (input.KeyPressed(Keys.A))
            {
                iniVelo.X -= random.Next(160);
                AddObj(new GameObject(new Rectangle(input.MouseState.X, input.MouseState.Y, 64, 64),
                    iniVelo));
            }
            else if (input.KeyPressed(Keys.D))
            {
                iniVelo.X += random.Next(160);
                AddObj(new GameObject(new Rectangle(input.MouseState.X, input.MouseState.Y, 64, 64),
                    iniVelo));

            }
            else if (input.KeyPressed(Keys.S))
            {
                iniVelo.Y -= random.Next(160);
                AddObj(new GameObject(new Rectangle(input.MouseState.X, input.MouseState.Y, 64, 64),
                     iniVelo));
            }
            else if (input.KeyPressed(Keys.W))
            {
                iniVelo.Y += random.Next(160);
                AddObj(new GameObject(new Rectangle(input.MouseState.X, input.MouseState.Y, 64, 64),
                    iniVelo));
            }


            deltaTime = deltaTime / 1000.0;

            //UPDATE POSITION

            for(int i = 0; i < objects.Count; i++)
            {
                objects[i].UpdatePos(deltaTime);
            }

            /*
            Console.WriteLine("The top normal unit vector of the floor is: " + floors[0].getNormalUnitVector(Direction.top).ToString());
            Console.WriteLine("The left normal unit vector of the floor is: " + floors[0].getNormalUnitVector(Direction.left).ToString());
            Console.WriteLine("The bottom normal unit vector of the floor is: " + floors[0].getNormalUnitVector(Direction.bottom).ToString());
            Console.WriteLine("The right normal unit vector of the floor is: " + floors[0].getNormalUnitVector(Direction.right).ToString());
            */

            //cameraNull.UpdatePos(deltaTime);


            //UPDATE INPUT AND CAMERA
            input.Update();            
            //camera.Update(cameraNull);
            

            //ADD FORCE
            for(int i = 0; i < objects.Count; i++)
            {
                objects[i].Force = Vector2.Zero;
                objects[i].AddForce(new Vector2(0, objects[i].Mass * GRAVITY));


                int j = i + 1;
                while (j < objects.Count)
                {
                    if (objects[i].CheckCollision(objects[j]))
                        ResolveCollision(objects[i], objects[j], deltaTime);

                    j++;
                }

                if (objects[i].CheckCollision(floors[0]))
                    ResolveCollision(objects[i], floors[0], deltaTime);
            }

            //MoveCamera(input, cameraNull);



            //UPDATE ACCELERATION AND THEN VELOCITY
            for(int i = 0; i <objects.Count; i++)
            {
                objects[i].UpdateAcc();
                objects[i].UpdateVelocity(deltaTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i< objects.Count; i++)
            {
                objects[i].Draw(spriteBatch);
            }

            if (objects.Count > 0)
            {
                spriteBatch.DrawString(font,objects[0].ToString(), new Vector2(20,20), Color.Black);
            }

            for (int i = 0; i < floors.Count; i ++)
            {
                floors[i].Draw(spriteBatch);
            }

            //cameraNull.Draw(spriteBatch);

            //draw string somewhere?
        }

        private void AddObj(GameObject obj)
        {
            obj.Texture = objTexture;
            objects.Add(obj);
        }

        private void ResolveCollision(GameObject obj1, GameObject obj2, double deltaTime)
        {
            float massTotal = obj1.Mass + obj2.Mass;
            float ratio1 = obj1.Mass / massTotal;
            float ratio2 = obj2.Mass / massTotal;

            Vector2 relativeVelocity = obj1.Velocity - obj2.Velocity;
            Direction direction = GetCollisionDirection(obj1, obj2, relativeVelocity);
            Vector2 normal = obj1.GetNormalUnitVector(direction);
            float normalVelocity = Vector2.Dot(relativeVelocity, normal);

           

            float e = Math.Min(obj1.Restitution, obj2.Restitution);

            float inverseMass1 = 1.0f / obj1.Mass;
            float inverseMass2 = 1.0f / obj2.Mass;


            float j = -(1 + e) * normalVelocity;
            j = j / (inverseMass1 + inverseMass2);

            Vector2 impulse = j * normal;
            float difference;


            switch (direction)
            {
                case Direction.top:
                    difference = obj1.Max.Y - obj2.Y;
                    obj1.Y -= ratio1 * difference;
                    obj2.Y += ratio2 * difference;
                    break;
                case Direction.bottom:
                    difference = obj2.Max.Y - obj1.Y;
                    obj1.Y += ratio1 * difference;
                    obj2.Y -= ratio2 * difference;
                    break;
                case Direction.left:
                    difference = obj1.Max.X - obj2.X;
                    obj1.X -= ratio1 * difference;
                    obj2.X += ratio2 * difference;
                    break;
                case Direction.right:
                    difference = obj2.Max.X - obj1.X;
                    obj1.X += ratio1 * difference;
                    obj2.X -= ratio2 * difference;
                    break;
            }

            Console.WriteLine("The direction was: " + direction);
            Console.WriteLine("The normal was: " + normal.ToString());
            Console.WriteLine("Added force of " + new Vector2(impulse.X / (float)deltaTime, impulse.Y / (float)deltaTime).ToString());

            obj1.AddForce(new Vector2(impulse.X * ratio1 / (float)deltaTime, impulse.Y * ratio1 / (float)deltaTime));
            obj2.AddForce(new Vector2(-impulse.X * ratio2 / (float)deltaTime,- impulse.Y * ratio2 / (float)deltaTime));

        }

        private void ResolveCollision(GameObject obj1, Floor obj2, double deltaTime)
        {
            Vector2 relativeVelocity = obj1.Velocity - obj2.Velocity;
            Direction direction = GetCollisionDirection(obj1, obj2, relativeVelocity);
            Vector2 normal = obj1.GetNormalUnitVector(direction);
            float normalVelocity = Vector2.Dot(relativeVelocity, normal);

            float e = Math.Min(obj1.Restitution, obj2.Restitution);

            float inverseMass1 = 1.0f / obj1.Mass;
            float inverseMass2 = 1.0f / obj2.Mass;

            float j = -(1 + e) * normalVelocity;
            j = j / (inverseMass1 + inverseMass2);

            Vector2 impulse = j * normal;


            if (direction == Direction.top)
            {
                obj1.Y = obj2.Y - obj1.Height;
               // obj1.Velocity = new Vector2(obj1.Velocity.X, 0);
            }
            else if (direction == Direction.left)
            {
                obj1.X = obj2.X - obj1.Width;
                //obj1.Velocity = new Vector2(0, obj1.Velocity.Y);

            }
            else if (direction == Direction.right)
            {
                obj1.X = obj2.Max.X;
               // obj1.Velocity = new Vector2(0, obj1.Velocity.Y);

            }
            else if (direction == Direction.bottom)
            {
                obj1.Y = obj2.Max.Y;
                //obj1.Velocity = new Vector2(obj1.Velocity.X, 0);
            }

            /*
            Console.WriteLine("The direction was: " + direction);
            Console.WriteLine("The normal was: " + normal.ToString());
            Console.WriteLine("Added force of " + new Vector2(impulse.X / (float)deltaTime, impulse.Y / (float)deltaTime).ToString());
            */

            obj1.AddForce(new Vector2(impulse.X / (float)deltaTime, impulse.Y / (float)deltaTime));
        }


        private Direction GetCollisionDirection(GameObject obj1, GameObject obj2, Vector2 relativeVelocity)
        {
            //too slow won't get triggered
            /*
            //checks if the thing moves in a straight line
            if (obj1.PrevPos.X == obj1.X)
            {
                if (obj1.PrevPos.Y < obj1.Y)
                    return Direction.top;
                else return Direction.bottom;
            }

            //checks if the thing moves in a straight line
            if (obj1.PrevPos.Y == obj1.Y)
            {
                if (obj1.PrevPos.X < obj1.X)
                    return Direction.left;
                else return Direction.bottom;
            }
            */

            //Vector2 difference = obj1. Position - obj1.PrevPos;

            if (relativeVelocity.X < 0 && relativeVelocity.Y > 0) //obj1 is traveling to the left && down
            {
                Vector2 distance = obj1.BottomLeft - obj2.TopRight;

                if (distance.X / relativeVelocity.X < distance.Y / relativeVelocity.Y)
                    return Direction.right;
                else return Direction.top;
            }

            if (relativeVelocity.X > 0 && relativeVelocity.Y > 0) //obj1 is traveling to the right & down
            {
                Vector2 distance = obj1.Max.ToVector2() - obj2.Position;

                if (distance.X / relativeVelocity.X < distance.Y / relativeVelocity.Y)
                    return Direction.left;
                else return Direction.top;
            }

            if (relativeVelocity.X > 0 && relativeVelocity.Y < 0) //obj1 is traveling to the right & up
            {
                Vector2 distance = obj1.TopRight - obj2.BottomLeft;

                if (distance.X / relativeVelocity.X < distance.Y / relativeVelocity.Y)
                    return Direction.left;
                else return Direction.bottom;
            }

            if (relativeVelocity.X < 0 && relativeVelocity.Y < 0) //obj1 is traveling to the left and up
            {
                Vector2 distance = obj1.BottomLeft - obj2.Max.ToVector2();

                if (distance.X / relativeVelocity.X < distance.Y / relativeVelocity.Y)
                    return Direction.right;
                else return Direction.bottom;
            }

            if (relativeVelocity.X == 0)
            {
                if (relativeVelocity.Y < 0)
                    return Direction.bottom;
                else return Direction.top;
            }

            else
            {
                if (relativeVelocity.X < 0)
                    return Direction.right;
                else return Direction.left;
            }


            //returns obj1's Direction
        }


        /*
        private void MoveCamera(InputManager input, GameObject cameraCenter)
        {
            if (input.KeysDown(Keys.A, Keys.Left) && input.KeysDown(Keys.D, Keys.Right))
            {
                cameraCenter.AddForce(new Vector2(-400, 0));
            }

            if (input.KeysDown(Keys.D, Keys.Right) && input.KeysDown(Keys.A, Keys.Left))
            {
                cameraCenter.AddForce(new Vector2(400, 0));
            }

            if (input.KeysDown(Keys.W, Keys.Up) && input.KeysDown(Keys.S, Keys.Down))
            {
                cameraCenter.AddForce(new Vector2(0, -400));
            }

            if (input.KeysDown(Keys.S, Keys.Down) && input.KeysDown(Keys.W, Keys.Up))
            {
                cameraCenter.AddForce(new Vector2(0, 400));
            }
        }
        */
    }
}