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
    class calculationPart : FlowPart
    {
        public string formula;
        public calculationPart(string formula)
        {
            this.formula = formula;
        }
        public override string Digest()
        {
            int locationOfequal = formula.IndexOf('=');
            return formula.Substring(locationOfequal + 1, formula.Length - locationOfequal - 1) + "="
                + formula.Substring(0, locationOfequal)+";";
        }

        public override XmlElement XmlDigest(XmlDocument doc)
        {
            int locationOfequal = formula.IndexOf('=');
            XmlElement defElement = doc.CreateElement("calc");
            defElement.InnerText = formula.Substring(locationOfequal + 1, formula.Length - locationOfequal - 1) + "="
                + formula.Substring(0, locationOfequal);
            return defElement;
            
        }
    }
}