using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace evdEn
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    public class ToonMenuScreen : MenuScreen
    {
        #region Fields

        bool newMode;

        MenuEntryList ratingEntry;

        ContentManager content = null;
        Texture2D backgroundTexture = null;
        Vector2 bkgTexturePos;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ToonMenuScreen(bool aNewMode)
            : base(aNewMode ? "New Character" : "Character Screen")
        {
            newMode = aNewMode;

            List<string> sl = new List<string>() { "a", "b" };
            sl.Add("ÌÝ");
            sl.Add("ÆÎ");

            LabelWidth = 200;

            // Create our menu entries.
            ratingEntry = new MenuEntryList(Messages.optMenuRating, sl, evdEnGlobals.Options.Rating);

            MenuEntry backMenuEntry = new MenuEntry(Messages.msgGoBack);

            //string s = Messages.ResourceManager.GetString("text", Messages.Culture);

            // Hook up menu event handlers.
            backMenuEntry.Selected += BackMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(ratingEntry);
            MenuEntries.Add(new MenuEntry(string.Empty, true));
            MenuEntries.Add(backMenuEntry);
            selectedEntry = 1;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            try
            {
                backgroundTexture = content.Load<Texture2D>(("Backs\\toonScreen"));
                bkgTexturePos = (new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height)
                    - new Vector2(backgroundTexture.Width, backgroundTexture.Height)
                    ) / 2;
            }
            catch
            {
                backgroundTexture = null;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (null != backgroundTexture)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                byte fade = TransitionAlpha;

                spriteBatch.Begin();

                spriteBatch.Draw(backgroundTexture, bkgTexturePos, new Color(fade, fade, fade));

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>

        void BackMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //evdEnGlobals.Options.Rating = ratingEntry.Value;

            //evdEnGlobals.SaveOptions();

            base.OnCancel(sender, e);
        }


        #endregion
    }
}
