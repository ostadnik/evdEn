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
using evdEnData;

namespace evdEn
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class evdEn : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public ScreenManager screenManager;

        bool zhopa = false;
        bool screenMgrAdded = false;

        public string[] args { get; set; }

        public evdEn()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            try
            {
                this.graphics.PreferredBackBufferWidth = 1280; 
                this.graphics.PreferredBackBufferHeight = 720;

                this.graphics.IsFullScreen = false;

                this.graphics.ApplyChanges();

                this.Components.Add(new GamerServicesComponent(this));
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                Console.WriteLine(s);

                zhopa = true;
            }

            evdEnGlobals.Content = this.Content;
            evdEnGlobals.Options.Rating = "AO";
            evdEnGlobals.Options.ShowCaptions = true;
            evdEnGlobals.Options.SpeachVolume = 1.0f;
            evdEnGlobals.Options.MusicVolume = 0.5f;
            evdEnGlobals.Options.EffectVolume = 0.75f;

            screenManager = new ScreenManager(this);
            
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
  
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (zhopa) this.Exit();

            evdEnGlobals.audioEngine = new AudioEngine("Content/Audio/Sounds.xgs");
            evdEnGlobals.soundBank = new SoundBank(evdEnGlobals.audioEngine, "Content/Audio/SoundBank.xsb");
            evdEnGlobals.waveBank = new WaveBank(evdEnGlobals.audioEngine, "Content/Audio/WaveBank.xwb");
            evdEnGlobals.backSBank = new SoundBank(evdEnGlobals.audioEngine, "Content/Audio/BackBank.xsb");
            evdEnGlobals.backWBank = new WaveBank(evdEnGlobals.audioEngine, "Content/Audio/BackBank.xwb", 0, 4);
            evdEnGlobals.musicCategory = evdEnGlobals.audioEngine.GetCategory("Music");
            evdEnGlobals.effectCategory = evdEnGlobals.audioEngine.GetCategory("Effect");
            evdEnGlobals.speachCategory = evdEnGlobals.audioEngine.GetCategory("Speach");
            evdEnGlobals.audioEngine.Update();

            // Play the sound.
            evdEnGlobals.backSBank.PlayCue("Okkerville");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            if (zhopa) this.Exit();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            evdGame theGame = Content.Load<evdGame>("Evdokim");


            evdEnUI.LoadContent(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (null != evdEnGlobals.audioEngine && !evdEnGlobals.gamePaused)
            {
                evdEnGlobals.audioEngine.Update();
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (!screenMgrAdded)
            {
                Components.Add(screenManager);
                screenMgrAdded = true;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
