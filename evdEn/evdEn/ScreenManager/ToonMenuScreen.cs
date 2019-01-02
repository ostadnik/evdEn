using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

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

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ToonMenuScreen(bool aNewMode)
            : base(aNewMode ? "New Character" : "Character Screen")
        {
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
            newMode = aNewMode;

            List<string> sl = new List<string>() { "ÌÝ", "ÆÎ" };
            
            LabelWidth = 200;

            // Create our menu entries.
            ratingEntry = new MenuEntryList("Gender", sl, "ÆÎ");

            MenuEntry backMenuEntry = new MenuEntry(Messages.msgGoBack);

            //string s = Messages.ResourceManager.GetString("text", Messages.Culture);

            // Hook up menu event handlers.
            backMenuEntry.Selected += BackMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(ratingEntry);
            MenuEntries.Add(new MenuEntry(string.Empty, true));
            MenuEntries.Add(backMenuEntry);
            selectedEntry = 0;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            try
            {
                this.BkgTexture = content.Load<Texture2D>(("Backs\\toonScreen"));
            }
            catch
            {
                Texture = null;
            }
        }

        public override void Draw(GameTime gameTime)
        {
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
