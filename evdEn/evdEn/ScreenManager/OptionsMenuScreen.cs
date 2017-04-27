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
    public class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntryList ratingEntry;
        MenuEntryCheckBox captionsEntry;
        MenuEntryHSlider musicVolumeEntry;
        MenuEntryHSlider effectVolumeEntry;
        MenuEntryHSlider speachVolumeEntry;

        ContentManager content = null;
        Texture2D backgroundTexture = null;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
           
            List<string> sl = new List<string>();
            sl.Add("AO");
            sl.Add("M");
            sl.Add("T");
            sl.Add("E10+");
            sl.Add("E");
            sl.Add("EC");

            LabelWidth = 200;

            // Create our menu entries.
            ratingEntry = new MenuEntryList(Messages.optMenuRating, sl, evdEnGlobals.Options.Rating);
            captionsEntry = new MenuEntryCheckBox(Messages.optMenuSubtitles, evdEnGlobals.Options.ShowCaptions);
            musicVolumeEntry = new MenuEntryHSlider(Messages.optMenuMusic, 0, 100, (int)(evdEnGlobals.Options.MusicVolume*100));
            effectVolumeEntry = new MenuEntryHSlider(Messages.optMenuSFX, 0, 100, (int)(evdEnGlobals.Options.EffectVolume * 100));
            speachVolumeEntry = new MenuEntryHSlider(Messages.optMenuSpeach, 0, 100, (int)(evdEnGlobals.Options.SpeachVolume * 100));

            MenuEntry backMenuEntry = new MenuEntry(Messages.msgGoBack);

            //string s = Messages.ResourceManager.GetString("text", Messages.Culture);

            // Hook up menu event handlers.
            backMenuEntry.Selected += BackMenuEntrySelected;
            
            // Add entries to the menu.
            MenuEntries.Add(ratingEntry);
            MenuEntries.Add(captionsEntry);
            MenuEntries.Add(new MenuEntry(Messages.optMenuVolumes, true));
            MenuEntries.Add(musicVolumeEntry);
            MenuEntries.Add(effectVolumeEntry);
            MenuEntries.Add(speachVolumeEntry);
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
                backgroundTexture = content.Load<Texture2D>(("Backs\\options"));
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
                Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
                byte fade = TransitionAlpha;

                spriteBatch.Begin();

                spriteBatch.Draw(backgroundTexture, fullscreen, new Color(fade, fade, fade));

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
            evdEnGlobals.Options.ShowCaptions = captionsEntry.Value;
            evdEnGlobals.Options.Rating = ratingEntry.Value;
            evdEnGlobals.Options.MusicVolume = (float)musicVolumeEntry.Value / 100.0f;
            evdEnGlobals.Options.EffectVolume = (float)effectVolumeEntry.Value / 100.0f;
            evdEnGlobals.Options.SpeachVolume = (float)speachVolumeEntry.Value / 100.0f;
            
            evdEnGlobals.SaveOptions();
            
            base.OnCancel(sender, e);
        }


        #endregion
    }
}
