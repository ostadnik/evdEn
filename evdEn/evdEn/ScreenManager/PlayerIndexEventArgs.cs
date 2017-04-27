using System;
using Microsoft.Xna.Framework;


namespace evdEn
{
    /// <summary>
    /// Custom event argument which includes the index of the player who
    /// triggered the event. This is used by the MenuEntry.Selected event.
    /// </summary>
    public class PlayerIndexEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }


        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        PlayerIndex playerIndex;
    }

    public class ActiveMenuItemEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ActiveMenuItemEventArgs(PlayerIndex playerIndex, int menuIndex)
        {
            this.playerIndex = playerIndex;
            this.menuIndex = menuIndex;
        }


        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public int MenuIndex
        {
            get { return menuIndex; }
        }

        int menuIndex;
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        PlayerIndex playerIndex;
    }
}
