using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace evdEnData
{
    public class evdMap : evdObject
    {
        public bool isWorld;

        public evdMap() : base()
        {
            isWorld = false;
        }

        public static evdMap Load(string fileName)
        {
            
            return new evdMap();
        }
    }
}
