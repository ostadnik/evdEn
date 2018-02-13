using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace evdEn
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    public class MainMenuScreen : MenuScreen
    {

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base(Messages.mainMenu)
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry(Messages.mainMenuNew);
            MenuEntry loadGameMenuEntry = new MenuEntry(Messages.mainMenuLoad);
            MenuEntry optionsMenuEntry = new MenuEntry(Messages.mainMenuOptions);
            MenuEntry exitMenuEntry = new MenuEntry(Messages.mainMenuExit);

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            loadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;
            exitMenuEntry.ToolTip = "It's when you are ready to quit this game, or when you aren't ready, but have to quit this game, or if there are some other reasons, even unreasonable, non-logical, or even mystical, which require you to quit this game.";
            this.ToolTipArea = evdEnUI.mainMenuToolTipArea;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(new MenuEntry(string.Empty, true));
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(new MenuEntry(string.Empty, true));
            MenuEntries.Add(new MenuEntry(Messages.mainMenuTrophies));
            MenuEntries.Add(new MenuEntry(Messages.mainMenuCredits));
            MenuEntries.Add(new MenuEntry(string.Empty, true));
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            SmthLoadingScreen.LoadSmth(ScreenManager, evdEnGlobals.theGame.NewGameActions[0], true, e.PlayerIndex,
                                new GameplayScreen(true));
        }

        /// <summary>
        /// Event handler for when the Load Game menu entry is selected.
        /// </summary>
        void LoadGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadSaveScreen lss = new LoadSaveScreen(LoadSaveMode.Load, "Load Game");
            lss.ScreenExiting += LoadGameMenuEntryContinued;
            ScreenManager.AddScreen(lss, e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Load Game menu entry is selected.
        /// </summary>
        void LoadGameMenuEntryContinued(object sender, ActiveMenuItemEventArgs e)
        {
            LoadSaveScreen lss = (LoadSaveScreen)sender;
            string ls = lss.chosenOne.Trim();
            if (lss.isConfirmed && !string.IsNullOrEmpty(ls))
            {
                lss = null;
                SmthLoadingScreen.LoadSmth(ScreenManager, evdEnGlobals.theGame.ContinueActions[0], true, e.PlayerIndex,
                         new GameplayScreen(false, ls));
            }
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(Messages.msgExitConfirm, MsgBoxOptions.boxYesNo);

            confirmExitMessageBox.Yessed += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion
    }
}
