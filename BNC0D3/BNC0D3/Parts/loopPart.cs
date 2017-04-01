using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BNC0D3.Parts
{
    class loopPart : FlowPart
    {
        public codePart codeinloop;

        public override string Digest()
        {
            return "while(1){"+codeinloop.Digest()+"}";
        }

        public override XmlElement XmlDigest(XmlDocument doc)
        {
            XmlElement loopElement = doc.CreateElement("loop");
            loopElement.AppendChild(codeinloop.XmlDigest(doc));
            return loopElement;
        }
    }
}