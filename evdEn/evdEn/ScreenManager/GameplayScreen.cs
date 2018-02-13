using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace evdEn
{
    /// <summary>
    /// This screen implements the actual game logic. 
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        string gameToLoad;

        bool newGame;
        //Cell test;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);
        Texture2D playerTexture;
        bool hit = false;

        Random random = new Random();

        #endregion

        #region Initialization

        /// <summary>
        /// constrictor
        /// </summary>
        /// <param name="isNewGame"></param>
        /// <param name="loadedGame"></param>
        public GameplayScreen(bool isNewGame, string loadedGame = "")
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            newGame = isNewGame;
            gameToLoad = loadedGame;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            evdEnGlobals.gamePaused = true;
            gameFont = evdEnUI.fntMenuLarge;
            playerTexture = content.Load<Texture2D>("Tiles\\averagefemale");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            string[] loadActions = newGame ? (evdEnGlobals.theGame.NewGameActions) : (evdEnGlobals.theGame.ContinueActions);
            bool isFirst = true;
            foreach (string loadAction in loadActions)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }
                string value = "";
                string key = "";
                if (loadAction.IndexOf(':') < 0)
                {
                    key = loadAction;
                }
                else
                {
                    key = loadAction.Substring(0, loadAction.IndexOf(':'));
                    value = loadAction.Substring(loadAction.IndexOf(':') + 1);
                }

                switch (key)
                {
                    case "delay":
                        Thread.Sleep(int.Parse(value) * 1000);
                        break;

                }


            }


            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
            evdEnGlobals.gamePaused = false;
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                evdEnGlobals.gamePaused = false;
                //test.Update(gameTime);
            }
            else
            {
                evdEnGlobals.gamePaused = true;
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                MessageBoxScreen cnfXMB = new MessageBoxScreen(Messages.msgReturnToGame, MsgBoxOptions.boxYesNo);
                cnfXMB.Noed += ConfirmExitMessageBoxAccepted;
                ScreenManager.AddScreen(cnfXMB, ControllingPlayer);

                evdEnGlobals.gamePaused = true;
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
            }
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            #region save
            if ((null == evdEnGlobals.Storage) || !evdEnGlobals.Storage.IsConnected)
            { }
            else
            {
                try
                {
                    IAsyncResult result = evdEnGlobals.Storage.BeginOpenContainer(evdEnGlobals.GameName, null, null);

                    // Wait for the WaitHandle to become signaled.
                    result.AsyncWaitHandle.WaitOne();

                    StorageContainer container = evdEnGlobals.Storage.EndOpenContainer(result);

                    // Close the wait handle.
                    result.AsyncWaitHandle.Close();
                    string filename = string.Format("save{0}.evden", evdEnGlobals.random.Next(10));

                    // Check to see whether the save exists.
                    if (container.FileExists(filename))
                        // Delete it so that we can create one fresh.
                        container.DeleteFile(filename);

                    // Create the file.
                    Stream stream = container.CreateFile(filename);
                    

                    // Convert the object to XML data and put it in the stream.
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(stream, evdEnGlobals.theGame);
                    // Close the file.
                    stream.Close();
                    // Dispose the container, to commit changes.
                    container.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            #endregion
            ExitScreen();
            evdEnGlobals.screenManager.AddScreen(new BackgroundScreen(), null);
            evdEnGlobals.screenManager.AddScreen(new MainMenuScreen(), null);
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Navy, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(playerTexture, playerPosition, new Rectangle(0, 0, 48, 90), Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
