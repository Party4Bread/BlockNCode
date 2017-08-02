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
using BNC0D3.Parts;

namespace BNC0D3
{
    public static class TempStorage
    {
        public static FlowPart tempFP = null;
        public static string tempSTR = null;
        public static int tempINT = 0;
        public static object tempOBJ = null;
    }
}