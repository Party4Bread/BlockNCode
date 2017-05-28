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
            XmlElement xmlcode = doc.CreateElement("sel");
            if (!elsestate)
            {
                xmlcode.SetAttribute("con", condition);
                xmlcode.AppendChild(truePart.XmlDigest(doc));
                xmlcode.SetAttribute("else", "false");
            }
            else
            {
                xmlcode.SetAttribute("con",condition);
                xmlcode.AppendChild(truePart.XmlDigest(doc));
                xmlcode.AppendChild(falsePart.XmlDigest(doc));
                xmlcode.SetAttribute("else", "true");
            }
            return xmlcode;
        }
    }
}