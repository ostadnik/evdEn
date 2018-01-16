using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace evdEnData
{
    public class evdGame : evdObject
    {
        public string GameFolder;
        public string Maps_OpenWorld;
        public string Maps;
        public string Thumbnail;
        public string StartDung;
        public int StartX;
        public int StartY;
        public string[] NewGameActions;
        public string[] ContinueActions = null;

        public evdGame() : base()
        {
        }

        public static evdGame Load(string fileName)
        {

            return new evdGame();
        }
    }
}
