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
using Android.Preferences;
using BreadMachine.Android;
using System.Threading.Tasks;
using BNC0D3.Parts;
using System.Xml;

namespace BNC0D3
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,Label = "userPref")]
    public class userPref : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.userPref);
            Switch codeswitch = FindViewById<Switch>(Resource.Id.scSwitch);
            codeswitch.Checked=Application.Context.GetSharedPreferences("BNCODE", FileCreationMode.Private).GetBoolean("codeShow", false);
            codeswitch.CheckedChange += Codeswitch_CheckedChange;
            TextView tv2 = FindViewById<TextView>(Resource.Id.textView2);
            /*
             @"<code>
            <def type='0' value='0'>a</def>
            <def type='0' value='0'>i</def>
            <ivk type='1' vtype='0'>a</ivk>
            <loop con='i&lt;10'>
            <code>
            <calc>i=i+1</calc>
            <ivk type='0'>(a*i).ToString()+""=""+a.ToString()+""*""+i.ToString()</ivk>
            </code>
            </loop>
            </code> " 선택받은 단 출력예제
             */
            BMachine bm = new BMachine(@"<code>
<def type='0' value='0'>a</def>
<def type='1' value='0'>ipt</def>
<loop con='a&lt;4'>
<code>
<calc>a=a+1</calc>
<ivk type='1' vtype='1'>ipt</ivk>
<ivk type='0'>ipt</ivk>
</code>
</loop>
</code> ", (string i) => {RunOnUiThread(()=> { tv2.Text += i + System.Environment.NewLine; }); });
            bm.Run();
            System.Threading.Thread.Sleep(10000);
            while (!bm.Input("5")) {
                bm.Input("5");
            }
            codePart cp = new codePart();
            cp.Add(new definePart(DefType.Number, "nigg", "2"));
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(cp.XmlDigest(doc));
            tv2.Text += doc.OuterXml + System.Environment.NewLine;

            //while (!bm.Input("5")) { }
            //while (!bm.Input("5")) { }
            //while (!bm.Input("5")) { }
            // Create your application here
        }

        private void Codeswitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var prefs = Application.Context.GetSharedPreferences("BNCODE", FileCreationMode.Private);
            var editor = prefs.Edit();
            editor.PutBoolean("codeShow",e.IsChecked);
            editor.Apply();
        }
    }
}