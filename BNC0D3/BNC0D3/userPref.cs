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
            BMachine bm = new BMachine(@"<code>
<def type='0' value='0'>a</def>
<def type='0' value='0'>i</def>
<ivk type='1' vtype='0'>a</ivk>
<loop con='i&lt;10'>
<code>
<calc>i=i+1</calc>
<ivk type='0'>(a*i).ToString()+""=""+a.ToString()+""*""+i.ToString()</ivk>
</code>
</loop>
</code> ", (string i) => {RunOnUiThread(()=> { tv2.Text += i + System.Environment.NewLine; }); });
            bm.Run();
            while (bm.status!=Status.WaitForInput)
            {
            }
            bm.Input("5");   
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