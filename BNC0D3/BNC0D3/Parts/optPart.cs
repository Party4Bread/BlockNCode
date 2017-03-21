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
    }
}