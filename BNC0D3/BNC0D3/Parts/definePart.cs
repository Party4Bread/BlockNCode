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
    public enum DefType { Number, String };
    public class definePart : FlowPart
    {
        string defName;
        string defValue;
        bool inputAble;
        private DefType defType;
        public definePart(DefType defType,string defName,string defValue,bool inputAble=false)
        {
            this.defName = defName;
            this.defType = defType;
            this.defValue = defValue;
            this.inputAble = inputAble;
        }
        public override string Digest()
        {
            if (inputAble)
            {
                if(defType==DefType.Number)
                {
                    return "var "+defName+":number=read();"; 
                }
                else
                {
                    return "var "+defName+":string=read();";
                }
            }
            else
            {
                if (defType == 0)
                {
                    return "var "+defName+":number="+defValue+";";
                }
                else
                {
                    return "var "+defName+":string="+defValue+";";
                }
            }
        }
    }
}