using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


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
            XmlDocument map = new XmlDocument();
            map.Load(fileName);
            return new evdMap();
        }
    }
}
