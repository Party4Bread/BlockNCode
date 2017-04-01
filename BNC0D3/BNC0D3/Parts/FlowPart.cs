using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Xml;

namespace BNC0D3.Parts
{
    public abstract class FlowPart
    {
        public int index;
        public int compoId;
        public abstract string Digest();
        public abstract XmlElement XmlDigest(XmlDocument doc);
    }
}