using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace evdEnData
{
    public class evdTileset : evdObject
    {
        public int firstGid;

        private int columns;
        private int tileCount;

        private Texture2D _texture;
        public Texture2D texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                if (null != value && 0 != x && 0 != y)
                {
                    columns = _texture.Width / x;
                    tileCount = columns * (_texture.Height / y);
                }
                else
                {
                    columns = 0;
                    tileCount = 0;
                }
            }
        }

        public evdTileset() : base()
        {
            _texture = null;
            columns = 0;
            tileCount = 0;
        }

        /// <summary>
        /// this one is for reading from the tmx-maps
        /// </summary>
        /// <param name="filename">.tmx file,   used for reporting only</param>
        /// <param name="xtile">xml-element of the tileset</param>
        /// <returns>evtTileset</returns>
        public static evdTileset Load(string filename, XmlElement xtile)
        {
            evdTileset tile = new evdTileset();

            int ii;
            string s;

            //firstgid="1" name="tile0002s" tilewidth="64" tileheight="64"
            s = xtile.GetAttribute("source");
            if(!string.IsNullOrEmpty(s)) throw new InvalidDataException(string.Format("file: [{0}]; sourced tilesets aren't supported; use image directly", filename));

            s = xtile.GetAttribute("firstgid");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [firstgid]; value: [{1}]", filename, s));
            tile.firstGid = ii;

            s = xtile.GetAttribute("tilewidth");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [tilewidth]; value: [{1}]", filename, s));
            tile.x = ii;

            s = xtile.GetAttribute("tileheight");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [tileheight]; value: [{1}]", filename, s));
            tile.y = ii;

            s = xtile.GetAttribute("name");
            if (string.IsNullOrEmpty(s.Trim())) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [name]; value: [{1}]", filename, s));
            tile.name = s.Trim();

            return tile;
        }

        /// <summary>
        /// this one is for reading tsx-files
        /// </summary>
        /// <param name="filename">.tsx file</param>
        /// <returns>evdTileset</returns>
        public static evdTileset Load(string filename)
        {
            evdTileset tileset = new evdTileset();
            XmlDocument xtile = new XmlDocument();
            xtile.Load(filename);

            int ii;
            string s;

            //tilewidth="64" tileheight="64"
            s = xtile.DocumentElement.GetAttribute("tilewidth");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [tilewidth]; value: [{1}]", filename, s));
            tileset.x = ii;

            s = xtile.DocumentElement.GetAttribute("tileheight");
            if (!int.TryParse(s, out ii)) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [tileheight]; value: [{1}]", filename, s));
            tileset.y = ii;

            s = xtile.DocumentElement.GetAttribute("name");
            if (string.IsNullOrEmpty(s.Trim())) throw new InvalidDataException(string.Format("file: [{0}]; attibute: [name]; value: [{1}]", filename, s));
            tileset.name = s.Trim();

            return tileset;
        }
    }
}
