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
            // TODO: read a value from the input ContentReader.
            TRead readed = new TRead();
            readed.name = input.ReadString();
            readed.x = input.ReadInt32();
            readed.y = input.ReadInt32();
            readed.isWorld = input.ReadBoolean();

            return readed;
        }
    }
}
