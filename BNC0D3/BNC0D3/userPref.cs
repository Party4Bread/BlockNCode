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
<def type='0' value='0'>sum</def>
<loop con='a&lt;10'>
<code>
<calc>a=a+1</calc>
<calc>sum=sum+a</calc>
<ivk type='0'>sum</ivk>
</code>
</loop>
</code> ", (string i) => { tv2.Text += i+ System.Environment.NewLine; });
            bm.Run();
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