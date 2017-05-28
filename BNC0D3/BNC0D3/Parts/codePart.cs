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
    class codePart : FlowPart
    {
        public List<FlowPart> code;
        public override string Digest()
        {
            string codeExport="";
            foreach(FlowPart part in code)
            {
                codeExport = part.Digest() + '\n';
            }
            return "";
        }

        public override XmlElement XmlDigest(XmlDocument doc)
        {
            string codeExport = "";
            XmlElement xmlcode = doc.CreateElement("code");
            foreach (FlowPart part in code)
            {
                xmlcode.AppendChild(part.XmlDigest(doc));
            }
            return xmlcode;
        }
    }
}