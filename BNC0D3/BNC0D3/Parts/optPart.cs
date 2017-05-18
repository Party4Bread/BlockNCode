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
    class optPart : FlowPart
    {
        public string formula;

        public optPart(string formula)
        {
            this.formula = formula;
        }

        public override string Digest()
        {
            return "print ("+formula+");";
        }
        
        public override XmlElement XmlDigest(XmlDocument doc)
        {
            XmlElement optElement = doc.CreateElement("ivk");
            optElement.SetAttribute("type", "0");
            optElement.InnerText = formula;
            return optElement;
        }
    }
}