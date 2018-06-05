using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        public string[] NewGameActions = null;
        public string[] ContinueActions = null;
        public string[] Variables = null;

        public evdGame() : base()
        {
        }

        public evdRunningGame MakeItRun()
        {
            evdRunningGame g = new evdRunningGame();

            g.gameName = this.name;
            g.map = this.StartDung;
            g.x = this.StartX;
            g.y = this.StartY;

            foreach (string var in this.Variables)
            {
                int idx = var.IndexOf('=');
                if (idx < 0)
                {
                    g.variables.Add(var.Trim(), "");
                }
                else
                {
                    g.variables.Add(var.Substring(0, idx).Trim(), var.Substring(idx + 1));
                }
            }

            return g;
        }
        
        //public static evdGame Load(string fileName)
        //{

        //    return new evdGame();
        //}
    }

    public class evdRunningGame 
    {
        public int version = 1;
        public string gameName = "";
        public string map = "";
        public float x = 0;
        public float y = 0;
        public Dictionary<string, string> variables = new Dictionary<string, string>();

        public static evdRunningGame Load(Stream sr)
        {
            int i, j;

            evdRunningGame g = new evdRunningGame();

            using (BinaryReader br = new BinaryReader(sr))
            {
                g.version = br.ReadInt32();

                if (g.version >= 1)
                {
                    g.gameName = br.ReadString();
                    g.map = br.ReadString();
                    g.x = br.ReadSingle();
                    g.y = br.ReadSingle();

                    int cnt = br.ReadInt32();
                    for(i = 0; i < cnt; i++)
                    {
                        g.variables.Add(br.ReadString(), br.ReadString());
                    }



                    if (g.version >= 2)
                    {
                    }
                }
            }

            return g;
        }

        public static void Save(Stream sr, evdRunningGame g)
        {
            int i, j;
            using (BinaryWriter bw = new BinaryWriter(sr))
            {
                i = 1;

                bw.Write(i);

                bw.Write(g.gameName);
                bw.Write(g.map);
                bw.Write(g.x);
                bw.Write(g.y);

                j = g.variables.Count;
                bw.Write(j);
                foreach (var p in g.variables)
                {
                    bw.Write(p.Key);
                    bw.Write(p.Value);
                }



                //tail
                { 
                    Int64 z = 0;
                    bw.Write(z);
                    bw.Write(z);
                    bw.Write(z);
                    bw.Write(z);
                }
            }
        }
    }
}
