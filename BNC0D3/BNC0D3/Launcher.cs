
using Android.App;
using Android.OS;
using Android.Widget;

namespace BNC0D3
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,Label = "Block&Code!",MainLauncher = true, Icon = "@drawable/icon",Theme = "@android:style/Theme.NoTitleBar")]
    public class Launcher : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Launcher);
            Button playbtn = (Button)FindViewById(Resource.Id.playbtn);
            Button optbtn = (Button)FindViewById(Resource.Id.optbtn);
            Button exitbtn = (Button)FindViewById(Resource.Id.exitbtn);
            Button lecturebtn = (Button)FindViewById(Resource.Id.lecturebtn);
            var dir = new Java.IO.File(Environment.ExternalStorageDirectory.AbsolutePath + "/BNC_SAVE/");
            if (!dir.Exists())
                dir.Mkdirs();
             playbtn.Click += delegate {
                StartActivity(typeof(MainActivity));
                OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
                
            };
            optbtn.Click += delegate {
                StartActivity(typeof(userPref));
                OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
            };
            lecturebtn.Click += delegate
            {
                StartActivity(typeof(LectureActivity));
                OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
            };
            exitbtn.Click += delegate { Finish(); };

        }
    }
}