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

namespace BNC0D3
{
    [Activity(Label = "Launcher",MainLauncher = true, Icon = "@drawable/icon",Theme = "@android:style/Theme.NoTitleBar")]
    public class Launcher : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Launcher);
            Button playbtn = (Button)FindViewById(Resource.Id.playbtn);
            Button exitbtn = (Button)FindViewById(Resource.Id.exitbtn);
            playbtn.Click += delegate {
                StartActivity(typeof(MainActivity));
                OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
                
            };
            
            exitbtn.Click += delegate { Finish(); };

        }
    }
}