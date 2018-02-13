using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO.Compression;
using System.IO;


namespace evdEnData
{
    public class evdMap : evdObject
    {
        public bool isWorld;
        public int height;
        public int width;
        public int tileHeight;
        public int tileWidth;
        public int[,] underLayer;
        public int[,] groundLayer;
        public int[,] onLayer;
        public int[,] overLayer;
        public int[,] collision;
        public List<evdTileset> tileSets;
        public List<string> objects;
        public Dictionary<string, string> properties;

        public evdMap()
            : base()
        {
            isWorld = false;
            tileSets = new List<evdTileset>();
            objects = new List<string>();
            properties = new Dictionary<string,string>();
        }

        public static evdMap Load(string filename)
        {
            evdMap map = new evdMap();

            XmlDocument xmap = new XmlDocument();
            xmap.Load(filename);

            int ii;
            string s;

            //width="32" height="32" tilewidth="64" tileheight="64"
            s = xmap.DocumentElement.GetAttribute("width");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [width]; value: [{1}]", filename, s));
            map.width = ii;

            s = xmap.DocumentElement.GetAttribute("height");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [height]; value: [{1}]", filename, s));
            map.height = ii;

            s = xmap.DocumentElement.GetAttribute("tilewidth");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [tilewidth]; value: [{1}]", filename, s));
            map.tileWidth = ii;

            s = xmap.DocumentElement.GetAttribute("tileheight");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [tileheight]; value: [{1}]", filename, s));
            map.tileHeight = ii;

            map.underLayer = new int[map.width, map.height];
            map.groundLayer = new int[map.width, map.height];
            map.onLayer = new int[map.width, map.height];
            map.overLayer = new int[map.width, map.height];
            map.collision = new int[map.width, map.height];

            //tilesets
            foreach (XmlElement xtile in xmap.GetElementsByTagName("tileset"))
            {
                map.tileSets.Add(evdTileset.Load(filename, xtile));
            }

            //layers
            foreach (XmlElement xlayer in xmap.GetElementsByTagName("layer"))
            {
                string name = xlayer.GetAttribute("name");

                foreach (XmlElement data in xlayer.GetElementsByTagName("data"))
                {
                    switch (name)
                    {
                        case "under":
                            LoadThatData(map.underLayer, data, map.width, map.height);
                            break;
                        case "ground":
                            LoadThatData(map.groundLayer, data, map.width, map.height);
                            break;
                        case "on":
                            LoadThatData(map.onLayer, data, map.width, map.height);
                            break;
                        case "over":
                            LoadThatData(map.overLayer, data, map.width, map.height);
                            break;
                        case "collision":
                            LoadThatData(map.collision, data, map.width, map.height);
                            break;
                    }
                }
            }

            //objects
            foreach (XmlElement xobjgrp in xmap.GetElementsByTagName("objectgroup"))
            {
                string name = xobjgrp.GetAttribute("name");
                if (name.Equals("objects"))
                {
                    foreach (XmlElement xobj in xmap.GetElementsByTagName("object"))
                    {
                        string ss = "";
                        foreach (XmlAttribute attr in xobj.Attributes)
                            ss += attr.Name + "=\"" + attr.Value + "\"|";
                        map.objects.Add(ss);
                    }

                    break; //there can be only one
                }
            }

            //properties
            foreach (XmlElement xprops in xmap.GetElementsByTagName("properties"))
            {
                foreach (XmlElement xobj in xmap.GetElementsByTagName("property"))
                {
                    // this will override duplicate keys, insetead of casting an exception 
                    map.properties[xobj.GetAttribute("name").Trim()] = xobj.GetAttribute("value");
                }
            }

            return map;
        }

        /// <summary>
        /// internal
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xdata"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void LoadThatData(int[,] data, XmlElement xdata, int width, int height)
        {
            int i = 0;
            int j = 0;

            string baseEncode = xdata.GetAttribute("encoding");

            if (string.IsNullOrEmpty(baseEncode))
            {
                foreach (XmlElement xtile in xdata.GetElementsByTagName("tile"))
                {
                    string s = xtile.GetAttribute("gid");
                    data[i, j] = int.Parse(s);

                    i++;
                    if (i >= width)
                    {
                        i = 0;
                        j++;
                        if (j >= height) break;
                    }
                }
            }
            else if (baseEncode.Equals("base64"))
            {
                string compression = xdata.GetAttribute("compression");
                string ss = xdata.InnerText;
                Byte[] bdata = Convert.FromBase64String(ss);

                if (string.IsNullOrEmpty(compression))
                {
                    for (int iData = 0; iData < bdata.Length; iData += sizeof(UInt32))
                    {
                        data[i, j] = BitConverter.ToInt32(bdata, iData);

                        i++;
                        if (i >= width)
                        {
                            i = 0;
                            j++;
                            if (j >= height) break;
                        }
                    }
                }
                else if (compression.Equals("gzip"))
                {
                    GZipStream gz = new GZipStream(new MemoryStream(bdata), CompressionMode.Decompress);
                    Byte[] buffer = new Byte[sizeof(UInt32)];
                    while (gz.Read(buffer, 0, buffer.Length) == buffer.Length)
                    {
                        data[i, j] = BitConverter.ToInt32(buffer, 0);

                        i++;
                        if (i >= width)
                        {
                            i = 0;
                            j++;
                            if (j >= height) break;
                        }
                    }
                }
                else if (compression.Equals("zlib"))
                {
                    // zlib data - first two bytes zlib specific and not part of deflate
                    MemoryStream ms = new MemoryStream(bdata);
                    ms.ReadByte();
                    ms.ReadByte();
                    DeflateStream gz = new DeflateStream(ms, CompressionMode.Decompress);
                    Byte[] buffer = new Byte[sizeof(UInt32)];
                    while (gz.Read(buffer, 0, buffer.Length) == buffer.Length)
                    {
                        data[i, j] = BitConverter.ToInt32(buffer, 0);

                        i++;
                        if (i >= width)
                        {
                            i = 0;
                            j++;
                            if (j >= height) break;
                        }
                    }

                }
                else
                {
                    throw new InvalidDataException(string.Format("unknown compression method: [{0}]", compression));
                }
            }
            else if (baseEncode.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
            {
                string s = xdata.InnerText;
                var ss = s.Split(new Char[] { ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int iData = 0; iData < ss.Length; iData++)
                {
                    int ii;
                    if (!int.TryParse(ss[iData], out ii)) continue;
                    data[i, j] = ii;

                    i++;
                    if (i >= width)
                    {
                        i = 0;
                        j++;
                        if (j >= height) break;
                    }
                }
            }
            else
            {
                throw new InvalidDataException(string.Format("unknown encoding method: [{0}]", baseEncode));
            }
        }
    }
}
