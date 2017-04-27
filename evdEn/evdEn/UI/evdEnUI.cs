using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace evdEn
{
    public static class evdEnUI
    {
        public static Color MenuColor = Color.White;
        public static Color MenuSelectedColor = Color.Yellow;
        public static Color MenuReadonlyColor = Color.White;
        public static Color BackgroundColor = Color.Black;
        public static Color MsgBoxColor = Color.Tomato;

        public static SpriteFont fntMenuLarge = null;
        public static SpriteFont fntMenuMed = null;
        public static SpriteFont fntMenuSmall = null;

        public static Rectangle mainMenuToolTipArea = new Rectangle(1280 - 300, 720 - 200, 300, 200);

        public static Point hScrollBounds = new Point(28, 169);
        public static Texture2D hScrollRailTexture = null;
        public static Texture2D hScrollSlideTexture = null;

        public static Rectangle msgBoxInnerBox = new Rectangle(30, 30, 640, 300); /* Inside Backgound Texture */
        public static Vector2 msgBoxUpPoint = new Vector2(647, 14); /* Inside Backgound Texture */
        public static Vector2 msgBoxDownPoint = new Vector2(646, 332); /* Inside Backgound Texture */
        public static Texture2D msgBoxBackTextureYN = null;
        public static Texture2D msgBoxBackTextureOC = null;
        public static Texture2D msgBoxBackTextureYNC = null;
        public static Texture2D msgBoxBackTexture = null;
        public static Texture2D msgBoxUpEnabledTexture = null; /* we assume, that "disabled" already on the back texture */
        public static Texture2D msgBoxDownEnabledTexture = null; /* we assume, that "disabled" already on the back texture */

        public static Point loadingFrameSize = new Point(32, 32);
        public static int loadingFramesCount = 4;
        public static Texture2D loadingFrames = null;
        public static Rectangle loadingDest = new Rectangle(32, 32, 32, 32);
        public static double loadingFPS = 1;
        
        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            fntMenuLarge = contentManager.Load<SpriteFont>("menuLarge");
            fntMenuMed = contentManager.Load<SpriteFont>("menuMed");
            fntMenuSmall = contentManager.Load<SpriteFont>("menuSmall");
            
            hScrollSlideTexture = contentManager.Load<Texture2D>("UI\\slide");
            hScrollRailTexture = contentManager.Load<Texture2D>("UI\\rail");
            msgBoxBackTexture = contentManager.Load<Texture2D>("UI\\mbBackO");
            msgBoxBackTextureOC = contentManager.Load<Texture2D>("UI\\mbBackOC");
            msgBoxBackTextureYN = contentManager.Load<Texture2D>("UI\\mbBackYN");
            msgBoxBackTextureYNC = contentManager.Load<Texture2D>("UI\\mbBackYNC");
            msgBoxUpEnabledTexture = contentManager.Load<Texture2D>("UI\\mbUpArrow");
            msgBoxDownEnabledTexture = contentManager.Load<Texture2D>("UI\\mbDownArrow");
            loadingFrames = contentManager.Load<Texture2D>("UI\\loadingFrames");
        }
    }
}
