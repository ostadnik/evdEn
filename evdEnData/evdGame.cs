using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace evdEnData
{
    public class evdGame : evdObject
    {
        string GameFolder;
        string Maps_OpenWorld;
        string Maps;
        string Thumbnail;

        public evdGame() : base()
        {
        }

        public static evdGame Load(string fileName)
        {

            return new evdGame();
        }
    }
}
