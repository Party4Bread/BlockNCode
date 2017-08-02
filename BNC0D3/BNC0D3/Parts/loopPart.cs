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
        public string condition;

        public loopPart(codePart codeinloop, string condition)
        {
            this.codeinloop = codeinloop;
            this.condition = condition;
        }
        public loopPart(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            if(doc.Name!="loop")
            {
                throw new Exception("XML이 형식에 맞지 않습니다.");
            }
            condition=doc.Attributes["con"].Value;
            codeinloop = new codePart(doc.InnerXml);
        }
        public override string Digest()
        {
            return "while(1){"+codeinloop.Digest()+"}";
        }

        public override XmlElement XmlDigest(XmlDocument doc)
        {
            XmlElement loopElement = doc.CreateElement("loop");
            XmlAttribute conditionAtt = doc.CreateAttribute("con");
            conditionAtt.Value = condition;
            loopElement.Attributes.Append(conditionAtt);
            loopElement.AppendChild(codeinloop.XmlDigest(doc));
            return loopElement;
        }
    }
}