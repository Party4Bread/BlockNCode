using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System;
using Android.Graphics;
using Android.Util;
using BNC0D3.Parts;

namespace BNC0D3
{
    [Activity(Label = "BNC0D3", WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | Android.Views.SoftInput.StateVisible, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        private ArrayAdapter<string> m_Adapter;
        private EditText conIpt;
        private ListView conOpt;
        private Button consumit;
        private Button defbtn;
        private Button calcbtn;
        private SlidingDrawer slider;

        public GridLayout gridflow { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            conOpt = (ListView)FindViewById(Resource.Id.consoleOpt);
            conIpt = (EditText)FindViewById(Resource.Id.consoleIpt);
            consumit = (Button)FindViewById(Resource.Id.consoleSummit);
            defbtn = (Button)FindViewById(Resource.Id.def_button);
            calcbtn = (Button)FindViewById(Resource.Id.calc_button);
            gridflow = (GridLayout)FindViewById(Resource.Id.gridView1);
            slider = (SlidingDrawer)FindViewById(Resource.Id.slidingDrawer1);
            DisplayMetrics displayMetrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            int height = displayMetrics.HeightPixels;
            int width = displayMetrics.WidthPixels;

            //preset
            m_Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            conOpt.Adapter = m_Adapter;
            conIpt.ImeOptions = Android.Views.InputMethods.ImeAction.Done;
            //value4code
            List<FlowPart> codeBlock = new List<FlowPart>();

            //events
            conIpt.EditorAction += conIpt_editeract;

            slider.DrawerOpen += delegate
            {
                foreach (FlowPart i in codeBlock)
                {
                    m_Adapter.Add(i.Digest());
                }
            };
            slider.DrawerClose += delegate { m_Adapter.Clear(); };


            consumit.Click += delegate {
                //VTvm.vm.io.read(conIpt.Text);
                m_Adapter.Add(conIpt.Text);
                conIpt.Text = "";
            };

            defbtn.Click += delegate {
                Button defflow = new Button(this)
                {
                    Text = "선언",
                    Tag = ""
                };
                defflow.SetTextColor(Color.Rgb(0, 0, 0));
                defflow.SetBackgroundColor(Color.Rgb(128, 0, 128));
                defflow.SetMinHeight(width / 5);
                defflow.SetMinWidth(width / 5);
                defflow.SetTextSize(Android.Util.ComplexUnitType.Dip, 25);
                gridflow.AddView(defflow);
                codeBlock.Add(new definePart(DefType.Number, "a", "4"));
            };

            calcbtn.Click += delegate {

            };

        }
        int diptppx(int dip)
        {
            return (int)((dip) * Resources.DisplayMetrics.Density); ;
        }
        private void conIpt_editeract(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
            {
                //VTvm.vm.io.read(((EditText)sender).Text);
                m_Adapter.Add(((EditText)sender).Text);
                conIpt.Text = "";
            }
        }
    }
}