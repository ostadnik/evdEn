using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using TWrite = evdEnData.evdMap;

namespace evdEnPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class tmxTypeWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            // props
            output.Write(value.name);
            output.Write(value.x);
            output.Write(value.y);
            output.Write(value.isWorld);

            output.Write(value.height);
            output.Write(value.width);
            output.Write(value.tileHeight);
            output.Write(value.tileWidth);

            // map data
            for (int j = 0; j < value.height; j++)
            {
                for (int i = 0; i < value.width; i++)
                {
                    output.Write(value.underLayer[i, j]);
                    output.Write(value.groundLayer[i, j]);
                    output.Write(value.onLayer[i, j]);
                    output.Write(value.overLayer[i, j]);
                    output.Write(value.collision[i, j]);
                }
            }

            // tiles info
            Int32 cnt = value.tileSets.Count;
            output.Write(cnt);
            for (int i = 0; i < cnt; i++)
            {
                output.Write(value.tileSets[i].name);
                output.Write(value.tileSets[i].x);
                output.Write(value.tileSets[i].y);
                output.Write(value.tileSets[i].firstGid);
            }

            // objects
            cnt = value.objects.Count;
            output.Write(cnt);
            for (int j = 0; j < cnt; j++)
            {
                output.Write(value.objects[j]);
            }

            // properties
            cnt = value.properties.Count;
            output.Write(cnt);
            foreach(var pair in value.properties)
            {
                output.Write(pair.Key);
                output.Write(pair.Value);
            }

            // just in case
            // terminator
            output.Write(0L);
            output.Write(0L);
            output.Write(0L);
            output.Write(0L);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "evdEn.tmxTypeReader, evdEn";
        }
    }
}
