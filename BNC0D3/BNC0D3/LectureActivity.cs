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
using System;
using System.Net;
using Android.Media;
using Android.Util;
using System.IO;
using Android.Graphics.Drawables;

namespace BNC0D3
{
    [Activity(Label = "LectureActivity", Theme = "@android:style/Theme.NoTitleBar")]
    public class LectureActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Lecture);
            GridLayout grd = FindViewById<GridLayout>(Resource.Id.lecturegrid);
            Android.Util.DisplayMetrics displayMetrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            int width = displayMetrics.WidthPixels/4;
            for (int i = 1; i <=16; i++)
            {
                Button lbtn = new Button(this) { Text = i.ToString() };
                lbtn.Click += (sender, evt) => {
                    string html = string.Empty;
                    string url = @"http://party4bread.xyz/BNCD/getactivelecture.php?no=" + (sender as Button).Text;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (System.IO.Stream stream = response.GetResponseStream())
                            using (System.IO.StreamReader reader = new StreamReader(stream))
                                html = reader.ReadToEnd();
                    string msg = html;
                    if (!msg.Contains("http"))
                    {
                        Toast.MakeText(this, msg,ToastLength.Long).Show();
                    }
                    else
                    {
                        Intent intent = new Intent(this,typeof(MainActivity));
                        intent.PutExtra("lecture",msg);
                        StartActivity(intent);
                    }
                };
                GridLayout.LayoutParams lp = new GridLayout.LayoutParams();
                //lp.SetGravity(GravityFlags.Fill);
                lp.Width = width;
                lp.Height = width;
                lp.SetMargins(0, 0, 0, 0);
                int rbpar = i * 16<255? i * 16:255, gpar = rbpar==255?255-i:255;
                GradientDrawable gd = new GradientDrawable();
                gd.SetStroke(5,new Android.Graphics.Color(0,0,0));
                gd.SetCornerRadius(10);
                gd.SetColor(new Android.Graphics.Color(rbpar, gpar, rbpar));
                lbtn.SetBackgroundDrawable(gd);
                
                lbtn.LayoutParameters = lp;
                
                rbpar = i * 17 < 255 ? i * 17 : 255;
                gpar = rbpar == 255 ? 255 - i*2 : 255;
                grd.SetBackgroundColor(new Android.Graphics.Color(rbpar, gpar, rbpar));
                grd.AddView(lbtn);
            }
            // Create your application here
        }
    }
}