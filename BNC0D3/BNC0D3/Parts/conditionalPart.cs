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
    class conditionalPart : FlowPart
    {
        public string condition = "";
        public bool elsestate = false;
        public codePart truePart;
        public codePart falsePart;
        public override string Digest()
        {
            if (!elsestate)
            {
                return "if(" + condition + "){" + truePart.Digest() + "}";
            }
            else
            {
                return "if(" + condition + "){" + truePart.Digest() + "}else{" + falsePart.Digest() + "}";
            }
        }

        public override XmlElement XmlDigest(XmlDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}