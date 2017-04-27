using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace evdEn
{
    /// <summary>
    /// A popup dialog box screen, used to display and navigate
    /// through dialogs. Cancel's not possible.
    /// </summary>
    public class DialogBoxScreen : GameScreen
    {
        #region Fields

        List<string> msgList;
        string msg;
        int totLines;
        int curLine;
        int shownLines;
        Vector2 bkgTexturePos;
        Vector2 textPosition;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public DialogBoxScreen(string message)
        {
            IsPopup = true;
            msg = message;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            msgList = evdEnUtils.SplitTextIntoList(msg, ScreenManager.Font, evdEnUI.msgBoxInnerBox.Width);
            curLine = 0;
            shownLines = -1;
            totLines = msgList.Count;
            msg = string.Empty;

            bkgTexturePos = (new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height)
                - new Vector2(evdEnUI.msgBoxBackTexture.Width, evdEnUI.msgBoxBackTexture.Height)
                ) / 2;
            textPosition = bkgTexturePos + new Vector2(evdEnUI.msgBoxInnerBox.X, evdEnUI.msgBoxInnerBox.Y);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (input.IsMenuDown(ControllingPlayer))
            {
                if (curLine < (totLines - 1)) curLine++;
                else curLine = totLines - 1;
            }
            else if (input.IsMenuUp(ControllingPlayer))
            {
                if (curLine > 0) curLine--;
                else curLine = 0;
            }
        }

        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            spriteBatch.Begin();

            spriteBatch.Draw(evdEnUI.msgBoxBackTexture, bkgTexturePos, color);

            shownLines = evdEnUtils.DrawTextInRect(msgList,
                new Rectangle((int)textPosition.X, (int)textPosition.Y, evdEnUI.msgBoxInnerBox.Width, evdEnUI.msgBoxInnerBox.Height),
                font, evdEnUI.MsgBoxColor, ref spriteBatch, curLine);

            if (curLine > 0)
                spriteBatch.Draw(evdEnUI.msgBoxUpEnabledTexture, bkgTexturePos + evdEnUI.msgBoxUpPoint, color);

            if ((curLine + shownLines) < totLines)
                spriteBatch.Draw(evdEnUI.msgBoxDownEnabledTexture, bkgTexturePos + evdEnUI.msgBoxDownPoint, color);

            spriteBatch.End();
        }


        #endregion
    }
}
