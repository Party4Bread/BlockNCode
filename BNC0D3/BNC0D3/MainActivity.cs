using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System;

namespace BNC0D3
{
    [Activity(Label = "BNC0D3", WindowSoftInputMode = Android.Views.SoftInput.AdjustPan|Android.Views.SoftInput.StateVisible, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        ArrayAdapter<string> m_Adapter;
        EditText conIpt;
        ListView conOpt;
        Button consumit;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            conOpt = (ListView)FindViewById(Resource.Id.consoleOpt);
            conIpt = (EditText)FindViewById(Resource.Id.consoleIpt);
            consumit = (Button)FindViewById(Resource.Id.consoleSummit);
            m_Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            conOpt.Adapter = m_Adapter;
            conIpt.ImeOptions = Android.Views.InputMethods.ImeAction.Done;
            conIpt.EditorAction += conIpt_editeract;
            consumit.Click += delegate {
                //VTvm.vm.io.read(conIpt.Text);
                m_Adapter.Add(conIpt.Text);
                conIpt.Text = "";
            };
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