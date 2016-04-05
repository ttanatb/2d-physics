using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
    Class: Camera  
    Purpose: controls the viewport of the game
*/


namespace physics2D
{
    /// <summary>
    /// The class that controls the camera movement based on the position of the player
    /// </summary>
    public class Camera
    {
        Matrix transform;   //draws camera
        Viewport view;      //where we are looking

        public Camera(Viewport newView)
        {
            transform = new Matrix();
            view = newView;
        }

        /// <summary>
        /// Updates the view of the camera
        /// </summary>
        /// <param name="player">The player to center the camera at</param>
        public void Update(GameObject centerObj)
        {
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(new Vector3(-centerObj.X, -centerObj.Y, 0));
        }
    }
}
