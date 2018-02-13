using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// TODO: replace this with the type you want to read.
using TRead = evdEnData.evdMap;

namespace evdEn
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class tmxTypeReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            TRead map = new TRead();
            map.name = input.ReadString();
            map.x = input.ReadInt32();
            map.y = input.ReadInt32();
            map.isWorld = input.ReadBoolean();

            map.height = input.ReadInt32();
            map.width = input.ReadInt32();
            map.tileHeight = input.ReadInt32();
            map.tileWidth = input.ReadInt32();

            map.underLayer = new int[map.width, map.height];
            map.groundLayer = new int[map.width, map.height];
            map.onLayer = new int[map.width, map.height];
            map.overLayer = new int[map.width, map.height];
            map.collision = new int[map.width, map.height];

            // map data
            for (int j = 0; j < map.height; j++)
            {
                for (int i = 0; i < map.width; i++)
                {
                    map.underLayer[i, j] = input.ReadInt32();
                    map.groundLayer[i, j] = input.ReadInt32();
                    map.onLayer[i, j] = input.ReadInt32();
                    map.overLayer[i, j] = input.ReadInt32();
                    map.collision[i, j] = input.ReadInt32();
                }
            }

            // tiles info
            Int32 cnt = input.ReadInt32();
            for (int i = 0; i < cnt; i++)
            {
                evdEnData.evdTileset tile = new evdEnData.evdTileset();
                tile.name =input.ReadString();
                tile.x=input.ReadInt32();
                tile.y=input.ReadInt32();
                tile.firstGid=input.ReadInt32();
                map.tileSets.Add(tile);
            }

            // objects
            cnt = input.ReadInt32();
            for (int i = 0; i < cnt; i++)
            {
                map.objects.Add(input.ReadString());
            }

            // properties
            cnt = input.ReadInt32();
            for (int i = 0; i < cnt; i++)
            {
                map.properties[input.ReadString()] = input.ReadString();
            }

            return map;
        }
    }
}
