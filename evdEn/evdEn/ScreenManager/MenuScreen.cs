using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace evdEn
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract public class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected int selectedEntry = 0;
        string caption = string.Empty;
        Rectangle toolTipArea = Rectangle.Empty;
        Texture2D texture = null;
        Texture2D activeTexture = null;
        Texture2D passiveTexture = null;
        Texture2D readonlyTexture = null;

        #endregion

        #region Properties

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public Rectangle ToolTipArea
        {
            get { return toolTipArea; }
            set { toolTipArea = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Texture2D ActiveTexture
        {
            get { return activeTexture; }
            set { activeTexture = value; }
        }

        public Texture2D PassiveTexture
        {
            get { return passiveTexture; }
            set { passiveTexture = value; }
        }

        public Texture2D ReadonlyTexture
        {
            get { return readonlyTexture; }
            set { readonlyTexture = value; }
        }

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        public int SelectedEntry
        {
            get { return selectedEntry; }
        }

        public int LabelWidth = 100;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string caption)
        {
            this.caption = caption;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion

        public event EventHandler<ActiveMenuItemEventArgs> ActiveMenuItemChanged;
        public event EventHandler<ActiveMenuItemEventArgs> ScreenExiting;

        #region Handle Input

        protected internal virtual void OnActiveMenuItemChanged(PlayerIndex playerIndex)
        {
            if (ActiveMenuItemChanged != null)
                ActiveMenuItemChanged(this, new ActiveMenuItemEventArgs(playerIndex, selectedEntry));
        }

        protected internal virtual void OnScreenExiting(PlayerIndex playerIndex)
        {
            if (ScreenExiting != null)
                ScreenExiting(this, new ActiveMenuItemEventArgs(playerIndex, selectedEntry));
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            int usedToBe = selectedEntry;

            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer))
            {
                evdEnGlobals.soundBank.PlayCue("menu");
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                // here we do only one attempt to skip read-only item
                while (menuEntries[selectedEntry].ReadOnly && (selectedEntry != usedToBe))
                {
                    selectedEntry--;
                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }
                OnActiveMenuItemChanged(ControllingPlayer.HasValue ? ControllingPlayer.Value : PlayerIndex.One);
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer))
            {
                evdEnGlobals.soundBank.PlayCue("menu");
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                // here we do only one attempt to skip read-only item
                while (menuEntries[selectedEntry].ReadOnly && (selectedEntry != usedToBe))
                {
                    selectedEntry++;
                    if (selectedEntry >= menuEntries.Count)
                        selectedEntry = 0;
                }
                OnActiveMenuItemChanged(ControllingPlayer.HasValue ? ControllingPlayer.Value : PlayerIndex.One);
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            if (input.IsMenuLeft(ControllingPlayer, out playerIndex))
            {
                evdEnGlobals.soundBank.PlayCue("menu");
                OnLeftEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuRight(ControllingPlayer, out playerIndex))
            {
                evdEnGlobals.soundBank.PlayCue("menu");
                OnRightEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                evdEnGlobals.soundBank.PlayCue("menu");
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                evdEnGlobals.soundBank.PlayCue("menu");
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry(playerIndex);
        }

        protected virtual void OnLeftEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnLeftEntry(playerIndex);
        }

        protected virtual void OnRightEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnRightEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
            OnScreenExiting(ControllingPlayer.HasValue ? ControllingPlayer.Value : PlayerIndex.One);
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Vector2 position = new Vector2(100, 150);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, position, isSelected, gameTime);

                position.Y += menuEntry.GetHeight(this);
            }

            // Draw the menu title.
            Vector2 titlePosition = new Vector2(426, 80);
            Vector2 titleOrigin = font.MeasureString(caption) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            if (null != Texture)
            {
                spriteBatch.Draw(Texture, 
                    new Vector2(titlePosition.X-(Texture.Width/2),
                                titlePosition.Y-(Texture.Height/2)),
                                Color.White);
            }

            spriteBatch.DrawString(font, caption, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            if (!toolTipArea.IsEmpty && !string.IsNullOrEmpty(menuEntries[selectedEntry].ToolTip))
            {
                evdEnUtils.DrawTextInRect(menuEntries[selectedEntry].ToolTip, 
                    toolTipArea, evdEnUI.fntMenuSmall, Color.LightGray, ref spriteBatch);
            }

            spriteBatch.End();
        }


        #endregion
    }
}
