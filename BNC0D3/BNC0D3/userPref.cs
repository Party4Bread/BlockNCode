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