using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using System.Collections.Generic;
using evdEnData;


namespace evdEn
{
    public static class evdEnGlobals
    {
        #region publics
        public static bool gamePadConnected = false;
        public static PlayerIndex gamePadIndex;
        public static ContentManager Content = null;
        public static Random random = new Random(DateTime.Now.Millisecond);
        public static AudioEngine audioEngine = null;
        public static SoundBank soundBank = null;
        public static SoundBank backSBank = null;
        public static WaveBank waveBank = null;
        public static WaveBank backWBank = null;
        public static AudioCategory musicCategory;
        public static AudioCategory effectCategory;
        public static AudioCategory speachCategory;
        public static string GameName = "Evdokim";
        public const string OptionsFileName = "options.feep";
        public static StorageDevice Storage = null;
        public static ScreenManager screenManager = null;
        public static Dictionary<string, Texture2D> textures = null;
        public static evdGame theGame = null;
        public static evdRunningGame myGame;

        //public static feepData.dsGame theGame = null;

        public static bool gamePaused = true;

        public static OptionsData Options;

        //public static PrimitiveBatch primitiveBatch = null;
        #endregion

        public static bool LoadOptions()
        {
            if ((null != Storage) && Storage.IsConnected)
            {
                // Open a storage container.
                IAsyncResult result =
                    evdEnGlobals.Storage.BeginOpenContainer(evdEnGlobals.GameName, null, null);

                // Wait for the WaitHandle to become signaled.
                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = evdEnGlobals.Storage.EndOpenContainer(result);

                // Get the path of the save game.
                if (container.FileExists(OptionsFileName))
                {
                    // Open the file.
                    Stream stream = container.OpenFile(OptionsFileName, FileMode.Open, FileAccess.Read);

                    BinaryFormatter formatter = new BinaryFormatter();

                    try
                    {
                        evdEnGlobals.Options = (OptionsData)formatter.Deserialize(stream);
                    }
                    catch
                    {
                        stream.Close();
                        container.Dispose();
                        // better to show some diagnostics...
                        return evdEnGlobals.SaveOptions();
                    }

                    stream.Close();
                    container.Dispose();

                    return true;
                }
                else
                {
                    return evdEnGlobals.SaveOptions();
                }
            }
            else if (null != Storage)
            {
                // Storage was disconnected
                return false;
            }
            return false;
        }

        public static bool SaveOptions()
        {
            if ((null != Storage) && Storage.IsConnected)
            {
                // Open a storage container.
                IAsyncResult result =
                    evdEnGlobals.Storage.BeginOpenContainer(evdEnGlobals.GameName, null, null);

                // Wait for the WaitHandle to become signaled.
                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = evdEnGlobals.Storage.EndOpenContainer(result);

                try
                {
                    // Open the file, creating it if necessary.
                    Stream stream = container.OpenFile(OptionsFileName, FileMode.OpenOrCreate, FileAccess.Write);

                    // Convert the object to XML data and put it in the stream.
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, evdEnGlobals.Options);
                    stream.Close();

                }
                catch
                {
                    container.Dispose();
                    return false;
                }

                container.Dispose();

                return true;
            }
            else if (null != Storage)
            {
                // Storage was disconnected
                return false;
            }
            return false;
        }

        public static Queue<GameEventArgs> queue = new Queue<GameEventArgs>();
    }

    [Serializable]
    public struct OptionsData
    {
        public string PlayerName;
        public string Rating;

        public bool ShowCaptions;
        public float MusicVolume;
        public float EffectVolume;
        public float SpeachVolume;

    }

    public class GameEventArgs : EventArgs
    {
        // public enum DoWhat { ShowMessageBox, WaitForAnyKey, ShowChoiceBox, };
        public string sArg1;
        public string sArg2;
        public int iArg1;
        public int iArg2;
        public double fArg1;
        public double fArg2;
        public DateTime Start;
        public DateTime End;
    }

    public delegate void GameEventHandler(object sender, GameEventArgs ge);
}
