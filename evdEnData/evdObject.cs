using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace evdEnData
{
    public class evdObject
    {
        public string name;
        public int x;
        public int y;

        public evdObject()
        {
            name = "";
            x = int.MinValue;
            y = int.MinValue;
        }
    }
}
