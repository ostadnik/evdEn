using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace evdEnData
{
    [Serializable]
    public class evdGame : evdObject
    {
        public string GameFolder;
        public string Maps_OpenWorld;
        public string Maps;
        public string Thumbnail;
        public string StartDung;
        public int StartX;
        public int StartY;
        public string[] NewGameActions = null;
        public string[] ContinueActions = null;
        public string[] Variables = null;

        public evdGame() : base()
        {
        }

        public static evdGame Load(string fileName)
        {

            return new evdGame();
        }
    }

    [Serializable]
    public struct evdRunningGame 
    {
        int version = 1;
        string gameName = "";
        string map = "";
        int x = 0;
        int y = 0;
        Dictionary<string, string> variables = new Dictionary<string, string>();


    }
}
