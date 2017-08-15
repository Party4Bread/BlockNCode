using System;
using System.Collections;
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
    public class codePart : FlowPart, IList<FlowPart>
    {
        public List<FlowPart> code;

        public codePart()
        {
            code = new List<FlowPart>();
        }

        public codePart(string xml)
        {
            code = new List<FlowPart>();
            XmlDocument a = new XmlDocument();
            a.LoadXml(xml);
            foreach (XmlNode i in a.ChildNodes[0].ChildNodes)
            {
                switch (i.Name)
                {
                    case "def":
                        code.Add(new definePart(i.Attributes["type"].Value=="0"?DefType.Number:DefType.String,i.InnerText,i.Attributes["value"].Value));
                        break;
                    case "calc":
                        code.Add(new calculationPart(i.InnerText));
                        break;
                    case "loop":
                        code.Add(new loopPart(i.OuterXml));
                        break;
                    case "sel":
                        code.Add(new conditionalPart(i.OuterXml));
                        break;
                    case "ivk":
                        code.Add(new optPart(i.InnerText));
                        break;
                    case "break":
                        break;
                    default:
                        break;
                }
            }
        }

        public FlowPart this[int index] { get => ((IList<FlowPart>)code)[index]; set => ((IList<FlowPart>)code)[index] = value; }

        public int Count => ((IList<FlowPart>)code).Count;

        public bool IsReadOnly => ((IList<FlowPart>)code).IsReadOnly;

        public void Add(FlowPart item)
        {
            ((IList<FlowPart>)code).Add(item);
        }
        public void Clear()
        {
            ((IList<FlowPart>)code).Clear();
        }
        public void Concats(codePart item)
        {
            code.Concat(item.code);
        }
        public static codePart operator +(codePart c1, codePart c2)
        {
            c1.code.AddRange(c2.code);
            return c1;
        }
        public bool Contains(FlowPart item)
        {
            return ((IList<FlowPart>)code).Contains(item);
        }

        public void CopyTo(FlowPart[] array, int arrayIndex)
        {
            ((IList<FlowPart>)code).CopyTo(array, arrayIndex);
        }

        public override string Digest()
        {
            string codeExport="";
            foreach(FlowPart part in code)
            {
                codeExport = part.Digest() + '\n';
            }
            return "";
        }

        public IEnumerator<FlowPart> GetEnumerator()
        {
            return ((IList<FlowPart>)code).GetEnumerator();
        }

        public int IndexOf(FlowPart item)
        {
            return ((IList<FlowPart>)code).IndexOf(item);
        }

        public void Insert(int index, FlowPart item)
        {
            ((IList<FlowPart>)code).Insert(index, item);
        }

        public bool Remove(FlowPart item)
        {
            return ((IList<FlowPart>)code).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<FlowPart>)code).RemoveAt(index);
        }

        public override XmlElement XmlDigest(XmlDocument doc)
        {
            //string codeExport = "";
            XmlElement xmlcode = doc.CreateElement("code");
            foreach (FlowPart part in code)
            {
                xmlcode.AppendChild(part.XmlDigest(doc));
            }
            return xmlcode;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<FlowPart>)code).GetEnumerator();
        }
    }
}