using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Storage;

namespace evdEn
{
    public enum LoadSaveMode { Load, Save };

    public class LoadSaveScreen : MenuScreen
    {
        public bool isConfirmed;
        public string chosenOne;

        MenuEntry[] slot = new MenuEntry[10];
        Texture2D[] pix = new Texture2D[10];

        LoadSaveMode mode;

        public LoadSaveScreen(LoadSaveMode mode, string title)
            : base(title)
        {
            this.mode = mode;
            isConfirmed = false;
            chosenOne = "";

            for (int i = 0; i < 10; i++)
            {
                slot[i] = new MenuEntry(string.Format("{0:00}.", i+1), "-- empty --");
            }

            BuildSavesList();

            LabelWidth = 60;

            for (int i = 0; i < 10; i++)
            {
                MenuEntries.Add(slot[i]);
                slot[i].Selected += OnSaveSelected;
            }

            MenuEntries.Add(new MenuEntry("", true));
            MenuEntry exit = new MenuEntry("Cancel");
            MenuEntries.Add(exit);
            exit.Selected += OnCancel;
            ActiveMenuItemChanged += activeMenuItemChanged;
            activeMenuItemChanged(null, new ActiveMenuItemEventArgs(PlayerIndex.One, 0);
        }

        void activeMenuItemChanged(object sender,ActiveMenuItemEventArgs e)
        {
            if (e.MenuIndex < 10)
            {
                string s = slot[e.MenuIndex].ToolTip;

            }
        }

        void OnSaveSelected(object sender, PlayerIndexEventArgs e)
        {
            chosenOne = ((MenuEntry)sender).ToolTip;
            isConfirmed = true;
            OnCancel(sender, e);
        }

        void BuildSavesList()
        {
            if ((null == evdEnGlobals.Storage) || !evdEnGlobals.Storage.IsConnected)
            {
                ExitScreen();
            }
            try
            {
                IAsyncResult result = evdEnGlobals.Storage.BeginOpenContainer(evdEnGlobals.GameName, null, null);

                // Wait for the WaitHandle to become signaled.
                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = evdEnGlobals.Storage.EndOpenContainer(result);

                // Close the wait handle.
                result.AsyncWaitHandle.Close();

                string[] FileList = container.GetFileNames("save?.evden");

                foreach (string filename in FileList)
                {
                    FileStream ff = (FileStream)container.OpenFile(filename, FileMode.Open);
                    string ss = ff.Name;
                    ff.Close();
                    string s = Path.GetFileNameWithoutExtension(filename);
                    if (s.Length == 5)
                    {
                        s = s.Substring(4);
                        int i;
                        if (int.TryParse(s, out i))
                        {
                            // we got our candidate
                            DateTime dt = File.GetLastWriteTime(ss);
                            slot[i].Text = dt.ToString("s").Replace('T', ' ');
                            slot[i].ToolTip = filename;
                        }
                    }
                    else
                    {
                        // not real save... where it came from???
                    }
                }
            }
            catch
            {

            }

        }

    }
}
