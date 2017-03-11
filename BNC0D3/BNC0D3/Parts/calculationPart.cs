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
    class calculationPart : FlowPart
    {
        string formula;
        public calculationPart(string formula)
        {
            this.formula = formula;
        }
        public override string Digest()
        {
            int locationOfequal = formula.LastIndexOf('=');
            return formula.Substring(locationOfequal + 1, formula.Length - locationOfequal - 1) + "="
                + formula.Substring(0, locationOfequal)+";";
        }
    }
}