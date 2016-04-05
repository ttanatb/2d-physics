using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace physics2D
{
    class Floor : GameObject
    {
        new const float MASS = 99999999999f;

        public Floor(Rectangle rectangle) : base(rectangle)
        {
            color = Color.Red;
        }
    }
}
