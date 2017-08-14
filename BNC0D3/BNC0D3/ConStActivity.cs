using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System;
using Android.Graphics;
using Android.Util;
using BNC0D3.Parts;
using Android.Views;
using System.Text.RegularExpressions;
using Android.Content;
using System.Xml;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System.Text;
using BreadMachine.Android;
using Android.Views.InputMethods;
using DynamicExpresso;
using Android.Runtime;

namespace BNC0D3
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Label = "BNC0D3", WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | SoftInput.StateAlwaysHidden, Theme = "@android:style/Theme.NoTitleBar")]
    public class ConStActivity : Activity
    {
        #region CREATE_VAR
        private Button conbtn,conokbtn,connotbtn;
        private Button calcbtn;
        private Button loopbtn, constbtn;
        private Button optbtn;
        private Button exitbtn,deletebtn;
        codePart trueBlock,falseBlock;
        int width;
        long lastexittry = 0;
        AlertDialog.Builder dialog;
        AlertDialog dialogger;
        public GridLayout trueflow, falseflow;
        List<variable> varList;
        InputMethodManager mgr;
        string condition;
        bool isSelectedBlockTrue;
        private int focusindex;
        #endregion
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.conStSetting);
            #region SET_VAR_VALUE
            conbtn = FindViewById<Button>(Resource.Id.con_button);
            calcbtn = FindViewById<Button>(Resource.Id.calc_button);
            optbtn = FindViewById<Button>(Resource.Id.opt_button);
            trueflow = (GridLayout)FindViewById(Resource.Id.gridView1);
            falseflow = FindViewById<GridLayout>(Resource.Id.gridView2);
            conokbtn = FindViewById<Button>(Resource.Id.conok_button);
            connotbtn = FindViewById<Button>(Resource.Id.connot_button);
            exitbtn = FindViewById<Button>(Resource.Id.exitbtn);
            loopbtn = FindViewById<Button>(Resource.Id.loop_button);
            constbtn = FindViewById<Button>(Resource.Id.conditianal_button);
            deletebtn = FindViewById<Button>(Resource.Id.deletebtn);
            DisplayMetrics displayMetrics = new DisplayMetrics();
            dialog = new AlertDialog.Builder(this); 
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            width = displayMetrics.WidthPixels;
            mgr = (InputMethodManager)GetSystemService(InputMethodService);
            //preset
            isSelectedBlockTrue = true;
            //value4code
            trueBlock = new codePart();
            falseBlock = new codePart();
            varList = TempStorage.tempOBJ as List<variable>;
            switch (Intent.GetStringExtra("mode"))
            {
                case "create":
                    deletebtn.Visibility = ViewStates.Gone;                    
                    break;
                case "fix":
                    deletebtn.Visibility = ViewStates.Visible;
                    deletebtn.Click += Deletebtn_Click;
                    conditionalPart tempcp = TempStorage.tempFP as conditionalPart;
                    condition=tempcp.condition;
                    //trueBlock=tempcp.truePart;
                    //falseBlock = tempcp.falsePart;
                    for(int idx=0;idx<tempcp.truePart.Count;idx++)
                    {
                        var i = tempcp.truePart[idx];
                        if(i is calculationPart)
                        {
                            try
                            {
                                codePart currentBlock = trueBlock;
                                GridLayout currentGrid = trueflow;
                                string formula = (i as calculationPart).formula;
                                FlowPart fp = i;
                                fp.compoId = (i as calculationPart).compoId;
                                fp.index = (i as calculationPart).index;

                                Button calcflow = new Button(this)
                                {
                                    Text = "연산",
                                    Tag = fp.index,
                                    Id = fp.compoId
                                };
                                calcflow.SetTextColor(Color.Rgb(0, 0, 0));
                                calcflow.SetBackgroundColor(Color.Rgb(137, 46, 228));
                                calcflow.SetMinHeight(width / 5);
                                calcflow.SetMinWidth(width / 5);
                                calcflow.SetTextSize(ComplexUnitType.Dip, 25);
                                calcflow.Click += (sednder, Dialo) =>
                                {
                                    int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
                                    View la = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
                                    TextView mN = (TextView)la.FindViewById(Resource.Id.MenuName);
                                    mN.Text = "계산식";
                                    LinearLayout ll = (LinearLayout)la.RootView;
                                    Button delb = new Button(this) { Text = "삭제" };
                                    delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
                                    delb.SetTextColor(Color.Rgb(0, 0, 0));
                                    delb.Click += delegate
                                    {
                                        currentBlock.RemoveAt(index);
                                        currentGrid.RemoveViewAt(index);
                                        dialogger.Cancel();
                                    };
                                    ll.AddView(delb);
                                    dialog.SetView(la);
                                    EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                                    formTb.Text = ((calculationPart)(currentBlock[index])).formula;
                                    dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                                    {
                                        try
                                        {
                                            string form = formTb.Text;
                                            checkFormular(varList, formTb.Text);
                                            currentBlock[index] = new calculationPart(formula);
                                        }
                                        catch (Exception ee)
                                        {
                                            Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                                        }
                                    });
                                    dialogger = dialog.Create();
                                    dialogger.Show();
                                };
                                currentBlock.Add(fp);
                                currentGrid.AddView(calcflow);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        }
                        else if(i is optPart)
                        {
                            try
                            {
                                codePart currentBlock = trueBlock;
                                GridLayout currentGrid = trueflow;
                                string formula = (i as optPart).formula;
                                FlowPart fp = i;
                                fp.compoId = i.compoId;
                                fp.index = i.index;

                                Button optflow = new Button(this)
                                {
                                    Text = "출력",
                                    Tag = currentBlock.Count,
                                    Id = fp.compoId
                                };
                                optflow.SetTextColor(Color.Rgb(0, 0, 0));
                                optflow.SetBackgroundColor(Color.Rgb(255, 72, 72));
                                optflow.SetMinHeight(width / 5);
                                optflow.SetMinWidth(width / 5);
                                optflow.SetTextSize(ComplexUnitType.Dip, 25);
                                optflow.Click += (sednder, Dialo) =>
                                {
                                    int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
                                    View la = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
                                    TextView mN = (TextView)la.FindViewById(Resource.Id.MenuName);
                                    mN.Text = "출력 수식";
                                    LinearLayout ll = (LinearLayout)la.RootView;
                                    Button delb = new Button(this) { Text = "삭제" };
                                    delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
                                    delb.SetTextColor(Color.Rgb(0, 0, 0));
                                    delb.Click += delegate
                                    {
                                        currentBlock.RemoveAt(index);
                                        currentGrid.RemoveViewAt(index);
                                        dialogger.Cancel();
                                    };
                                    ll.AddView(delb);
                                    dialog.SetView(la);
                                    EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                                    formTb.Text = ((optPart)(currentBlock[index])).formula;
                                    dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                                    {
                                        try
                                        {
                                            string form = formTb.Text;
                                            checkFormular(varList, formTb.Text);
                                            currentBlock[index] = new optPart(formula);
                                        }
                                        catch (Exception ee)
                                        {
                                            Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                                        }
                                    });
                                    dialogger = dialog.Create();
                                    dialogger.Show();
                                };
                                currentGrid.AddView(optflow);
                                currentBlock.Add(fp);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        }
                        else if(i is conditionalPart)
                        {
                            FlowPart fpc = i;

                            fpc.compoId = i.compoId;
                            fpc.index = i.index;

                            Button conditionflow = new Button(this)
                            {
                                Text = "선택",
                                Tag = fpc.index,
                                Id = fpc.compoId
                            };
                            conditionflow.SetTextColor(Color.Rgb(0, 0, 0));
                            conditionflow.SetBackgroundColor(Color.Rgb(0x2A, 0xDA, 0x64));
                            conditionflow.SetMinHeight(width / 5);
                            conditionflow.SetMinWidth(width / 5);
                            conditionflow.SetTextSize(ComplexUnitType.Dip, 25);
                            conditionflow.Click += (sednder, Dialo) =>
                            {
                                Intent inte = new Intent(this, typeof(ConStActivity));
                                inte.PutExtra("mode", "fix");
                                TempStorage.tempFP = fpc;
                                TempStorage.tempINT = fpc.index;
                                StartActivityForResult(inte, (int)Activitycode.conditionfix);
                                //conditionfixactivity need vvxccccvvx
                            };
                            trueBlock.Add(fpc);
                            trueflow.AddView(conditionflow);
                        }
                        else if(i is loopPart)
                        {
                            FlowPart fp = i;

                            fp.compoId = i.compoId;
                            fp.index = i.index;

                            Button loopflow = new Button(this)
                            {
                                Text = "반복",
                                Tag = fp.index,
                                Id = fp.compoId
                            };
                            loopflow.SetTextColor(Color.Rgb(0, 0, 0));
                            loopflow.SetBackgroundColor(Color.Rgb(204, 131, 20));
                            loopflow.SetMinHeight(width / 5);
                            loopflow.SetMinWidth(width / 5);
                            loopflow.SetTextSize(ComplexUnitType.Dip, 25);
                            loopflow.Click += (sednder, Dialo) =>
                            {
                                Intent inte = new Intent(this, typeof(LoopActivity));
                                inte.PutExtra("mode", "fix");
                                TempStorage.tempFP = fp;
                                StartActivityForResult(inte, (int)Activitycode.loopfix);
                                //loopfixactivity need 
                            };
                            trueflow.AddView(loopflow);
                            trueBlock.Add(fp);
                        }
                    }
                    for (int idx = 0; idx < tempcp.falsePart.Count; idx++)
                    {
                        var i = tempcp.falsePart[idx];
                        if (i is calculationPart)
                        {
                            try
                            {
                                codePart currentBlock = falseBlock;
                                GridLayout currentGrid = falseflow;
                                string formula = (i as calculationPart).formula;
                                FlowPart fp = i;
                                fp.compoId = (i as calculationPart).compoId;
                                fp.index = (i as calculationPart).index;

                                Button calcflow = new Button(this)
                                {
                                    Text = "연산",
                                    Tag = fp.index,
                                    Id = fp.compoId
                                };
                                calcflow.SetTextColor(Color.Rgb(0, 0, 0));
                                calcflow.SetBackgroundColor(Color.Rgb(137, 46, 228));
                                calcflow.SetMinHeight(width / 5);
                                calcflow.SetMinWidth(width / 5);
                                calcflow.SetTextSize(ComplexUnitType.Dip, 25);
                                calcflow.Click += (sednder, Dialo) =>
                                {
                                    int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
                                    View la = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
                                    TextView mN = (TextView)la.FindViewById(Resource.Id.MenuName);
                                    mN.Text = "계산식";
                                    LinearLayout ll = (LinearLayout)la.RootView;
                                    Button delb = new Button(this) { Text = "삭제" };
                                    delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
                                    delb.SetTextColor(Color.Rgb(0, 0, 0));
                                    delb.Click += delegate
                                    {
                                        currentBlock.RemoveAt(index);
                                        currentGrid.RemoveViewAt(index);
                                        dialogger.Cancel();
                                    };
                                    ll.AddView(delb);
                                    dialog.SetView(la);
                                    EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                                    formTb.Text = ((calculationPart)(currentBlock[index])).formula;
                                    dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                                    {
                                        try
                                        {
                                            string form = formTb.Text;
                                            checkFormular(varList, formTb.Text);
                                            currentBlock[index] = new calculationPart(formula);
                                        }
                                        catch (Exception ee)
                                        {
                                            Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                                        }
                                    });
                                    dialogger = dialog.Create();
                                    dialogger.Show();
                                };
                                currentBlock.Add(fp);
                                currentGrid.AddView(calcflow);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        }
                        else if (i is optPart)
                        {
                            try
                            {
                                codePart currentBlock = falseBlock;
                                GridLayout currentGrid = falseflow;
                                string formula = (i as optPart).formula;
                                FlowPart fp = i;
                                fp.compoId = i.compoId;
                                fp.index = i.index;

                                Button optflow = new Button(this)
                                {
                                    Text = "출력",
                                    Tag = currentBlock.Count,
                                    Id = fp.compoId
                                };
                                optflow.SetTextColor(Color.Rgb(0, 0, 0));
                                optflow.SetBackgroundColor(Color.Rgb(255, 72, 72));
                                optflow.SetMinHeight(width / 5);
                                optflow.SetMinWidth(width / 5);
                                optflow.SetTextSize(ComplexUnitType.Dip, 25);
                                optflow.Click += (sednder, Dialo) =>
                                {
                                    int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
                                    View la = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
                                    TextView mN = (TextView)la.FindViewById(Resource.Id.MenuName);
                                    mN.Text = "출력 수식";
                                    LinearLayout ll = (LinearLayout)la.RootView;
                                    Button delb = new Button(this) { Text = "삭제" };
                                    delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
                                    delb.SetTextColor(Color.Rgb(0, 0, 0));
                                    delb.Click += delegate
                                    {
                                        currentBlock.RemoveAt(index);
                                        currentGrid.RemoveViewAt(index);
                                        dialogger.Cancel();
                                    };
                                    ll.AddView(delb);
                                    dialog.SetView(la);
                                    EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                                    formTb.Text = ((optPart)(currentBlock[index])).formula;
                                    dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                                    {
                                        try
                                        {
                                            string form = formTb.Text;
                                            checkFormular(varList, formTb.Text);
                                            currentBlock[index] = new optPart(formula);
                                        }
                                        catch (Exception ee)
                                        {
                                            Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                                        }
                                    });
                                    dialogger = dialog.Create();
                                    dialogger.Show();
                                };
                                currentGrid.AddView(optflow);
                                currentBlock.Add(fp);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        }
                        else if (i is conditionalPart)
                        {
                            FlowPart fpc = i;

                            fpc.compoId = i.compoId;
                            fpc.index = i.index;

                            Button conditionflow = new Button(this)
                            {
                                Text = "선택",
                                Tag = fpc.index,
                                Id = fpc.compoId
                            };
                            conditionflow.SetTextColor(Color.Rgb(0, 0, 0));
                            conditionflow.SetBackgroundColor(Color.Rgb(0x2A, 0xDA, 0x64));
                            conditionflow.SetMinHeight(width / 5);
                            conditionflow.SetMinWidth(width / 5);
                            conditionflow.SetTextSize(ComplexUnitType.Dip, 25);
                            conditionflow.Click += (sednder, Dialo) =>
                            {
                                Intent inte = new Intent(this, typeof(ConStActivity));
                                inte.PutExtra("mode", "fix");
                                TempStorage.tempFP = fpc;
                                TempStorage.tempINT = fpc.index;
                                StartActivityForResult(inte, (int)Activitycode.conditionfix);
                                //conditionfixactivity need vvxccccvvx
                            };
                            falseBlock.Add(fpc);
                            falseflow.AddView(conditionflow);
                        }
                        else if (i is loopPart)
                        {
                            FlowPart fp = i;

                            fp.compoId = i.compoId;
                            fp.index = i.index;

                            Button loopflow = new Button(this)
                            {
                                Text = "반복",
                                Tag = fp.index,
                                Id = fp.compoId
                            };
                            loopflow.SetTextColor(Color.Rgb(0, 0, 0));
                            loopflow.SetBackgroundColor(Color.Rgb(204, 131, 20));
                            loopflow.SetMinHeight(width / 5);
                            loopflow.SetMinWidth(width / 5);
                            loopflow.SetTextSize(ComplexUnitType.Dip, 25);
                            loopflow.Click += (sednder, Dialo) =>
                            {
                                Intent inte = new Intent(this, typeof(LoopActivity));
                                inte.PutExtra("mode", "fix");
                                TempStorage.tempFP = fp;
                                StartActivityForResult(inte, (int)Activitycode.loopfix);
                                //loopfixactivity need 
                            };
                            falseflow.AddView(loopflow);
                            falseBlock.Add(fp);
                        }                        
                    }
                    break;
            }
            #endregion

            #region COMPONENT_EVENT
            conbtn.Click += Conbtn_Click;
            connotbtn.Click += Connotbtn_Click;
            conokbtn.Click += Conokbtn_Click;
            calcbtn.Click += Calcbtn_Click;
            optbtn.Click += Optbtn_Click;
            exitbtn.Click += Exitbtn_Click;
            loopbtn.Click += Loopbtn_Click;
            constbtn.Click += Constbtn_Click;
            #endregion
            TempStorage.tempFP = null;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            codePart currentBlock = GetCurrentBlock();
            GridLayout currentGrid = GetCurrentGrid();
            if (data.GetStringExtra("status") == "deleted")
            {
                currentBlock.RemoveAt(focusindex);
                currentGrid.RemoveViewAt(focusindex);
                return;
            }
            switch ((Activitycode)requestCode)
            {
                case Activitycode.loop:
                    if (TempStorage.tempFP == null)
                    {
                        return;
                    }

                    FlowPart fp = TempStorage.tempFP;

                    fp.compoId = View.GenerateViewId();
                    fp.index = currentBlock.Count;

                    Button loopflow = new Button(this)
                    {
                        Text = "반복",
                        Tag = currentBlock.Count,
                        Id = fp.compoId
                    };
                    loopflow.SetTextColor(Color.Rgb(0, 0, 0));
                    loopflow.SetBackgroundColor(Color.Rgb(204, 131, 20));
                    loopflow.SetMinHeight(width / 5);
                    loopflow.SetMinWidth(width / 5);
                    loopflow.SetTextSize(ComplexUnitType.Dip, 25);
                    loopflow.Click += (sednder, Dialo) =>
                    {
                        Intent i = new Intent(this, typeof(LoopActivity));
                        i.PutExtra("mode", "fix");
                        TempStorage.tempFP = fp;
                        TempStorage.tempFP2 = currentBlock;
                        TempStorage.tempOBJ = currentGrid;
                        focusindex = fp.index;
                        StartActivityForResult(i, (int)Activitycode.loopfix);
                        //loopfixactivity need 
                    };
                    currentGrid.AddView(loopflow);
                    currentBlock.Add(fp);
                    break;
                case Activitycode.condition:
                    if (TempStorage.tempFP == null)
                    {
                        return;
                    }
                    FlowPart fpc = TempStorage.tempFP;

                    fpc.compoId = View.GenerateViewId();
                    fpc.index = currentBlock.Count;

                    Button conditionflow = new Button(this)
                    {
                        Text = "선택",
                        Tag = currentBlock.Count,
                        Id = fpc.compoId
                    };
                    conditionflow.SetTextColor(Color.Rgb(0, 0, 0));
                    conditionflow.SetBackgroundColor(Color.Rgb(0x2A, 0xDA, 0x64));
                    conditionflow.SetMinHeight(width / 5);
                    conditionflow.SetMinWidth(width / 5);
                    conditionflow.SetTextSize(ComplexUnitType.Dip, 25);
                    conditionflow.Click += (sednder, Dialo) =>
                    {
                        Intent i = new Intent(this, typeof(ConStActivity));
                        i.PutExtra("mode", "fix");
                        TempStorage.tempFP = fpc;
                        TempStorage.tempINT = focusindex = fpc.index;
                        TempStorage.tempFP2 = currentBlock;
                        TempStorage.tempOBJ = currentGrid;
                        StartActivityForResult(i, (int)Activitycode.conditionfix);
                        //conditionfixactivity need vvxccccvvx
                    };
                    currentGrid.AddView(conditionflow);
                    currentBlock.Add(fpc);
                    break;
                case Activitycode.conditionfix:
                    if (Intent.GetStringExtra("status") == "deleted")
                    {
                        (TempStorage.tempFP2 as codePart).RemoveAt(TempStorage.tempINT);
                        (TempStorage.tempOBJ as GridLayout).RemoveViewAt(TempStorage.tempINT);
                    }
                    else
                    {
                        (TempStorage.tempFP2 as codePart)[TempStorage.tempINT] = TempStorage.tempFP;
                    }
                    break;
                case Activitycode.loopfix:
                    if (Intent.GetStringExtra("status") == "deleted")
                    {
                        (TempStorage.tempFP2 as codePart).RemoveAt(TempStorage.tempINT);
                        (TempStorage.tempOBJ as GridLayout).RemoveViewAt(TempStorage.tempINT);
                    }
                    else
                    {
                        (TempStorage.tempFP2 as codePart)[TempStorage.tempINT] = TempStorage.tempFP;
                    }
                    break;
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.PutExtra("status","deleted");
            if (condition == "" || condition == null)
            {
                Toast.MakeText(this, "조건식란이 비어있습니다", ToastLength.Long).Show();
                return;
            }

            TempStorage.tempFP = new conditionalPart(trueBlock, condition, falseBlock);
            SetResult(Result.Ok,intent);
            Finish();
        }

        private void Constbtn_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ConStActivity));
            i.PutExtra("mode", "create");
            TempStorage.tempOBJ = varList;
            StartActivityForResult(i, (int)Activitycode.condition);
        }

        private void Loopbtn_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(LoopActivity));
            i.PutExtra("mode", "create");
            TempStorage.tempOBJ = varList;
            StartActivityForResult(i, (int)Activitycode.loop);
        }
         
        private void Exitbtn_Click(object sender, EventArgs e)
        {
            handleClose();
        }

        public override void OnBackPressed()
        {
            handleClose();
        }

        void handleClose()
        {
            //Intent intent = new Intent();
            //XmlDocument doc = new XmlDocument();
            //doc.AppendChild(GetCurrentBlock().XmlDigest(doc));
            //intent.PutExtra("code",doc.OuterXml);
            Intent intent = new Intent();

            //Upper code might be useless.
            //make checkfunction plz
            if (condition == "" || condition == null)
            {
                Toast.MakeText(this, "조건식란이 비어있습니다. 뒤로 가려면 2번 누르세요", ToastLength.Long).Show();
                if(new TimeSpan(DateTime.Now.Ticks - lastexittry).TotalSeconds < 1)
                {
                    intent.PutExtra("status", "GOOD");
                    SetResult(Result.Canceled,intent);
                    Finish();
                }
                else
                {
                    lastexittry = DateTime.Now.Ticks;
                }
                return;
            }
            intent.PutExtra("status", "GOOD");
            TempStorage.tempFP = new conditionalPart(trueBlock,condition,falseBlock);
            SetResult(Result.Ok,intent);
            Finish();
        }

        GridLayout GetCurrentGrid()
        {
            return isSelectedBlockTrue ? trueflow : falseflow;
        }

        codePart GetCurrentBlock()
        {
            return isSelectedBlockTrue ? trueBlock : falseBlock;
        }

        #region component_Function
        private void Conokbtn_Click(object sender, EventArgs e)
        {
            isSelectedBlockTrue = true;
            conokbtn.SetBackgroundColor(Color.Rgb(0xee, 0, 0xee));
            connotbtn.SetBackgroundColor(Color.Rgb(0xcc, 0, 0xcc));
        }

        private void Connotbtn_Click(object sender, EventArgs e)
        {
            isSelectedBlockTrue = false;
            conokbtn.SetBackgroundColor(Color.Rgb(0xcc, 0, 0xcc));
            connotbtn.SetBackgroundColor(Color.Rgb(0xee, 0, 0xee));
        }

        private void Conbtn_Click(object sender, EventArgs e)
        {
            View layout = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
            dialog.SetView(layout);
            TextView menuName = (TextView)layout.FindViewById(Resource.Id.MenuName);
            menuName.Text = "조건식";
            EditText formularTb = (EditText)layout.FindViewById(Resource.Id.formular);
            formularTb.Text = condition ?? "";
            //레이아웃설정
            dialog.SetPositiveButton(Android.Resource.String.Ok, (sendder, ee) =>
            {
                try
                {
                    string formula = formularTb.Text;
                    var tpvl = varList;

                    //checkCondition(varList, formularTb.Text); may need it at final version
                    condition = formula;
                }
                catch (Exception eee)
                {
                    Toast.MakeText(this, eee.Message, ToastLength.Long).Show();
                }
            });
            dialogger = dialog.Create();
            dialogger.Show();
        }

        private void Optbtn_Click(object s, EventArgs eA)
        {
            View layout = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
            dialog.SetView(layout);
            TextView menuName = (TextView)layout.FindViewById(Resource.Id.MenuName);
            menuName.Text = "출력 수식";
            EditText formularTb = (EditText)layout.FindViewById(Resource.Id.formular);
            dialog.SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
            {
                try
                {
                    codePart currentBlock = GetCurrentBlock();
                    GridLayout currentGrid = GetCurrentGrid();
                    string formula = formularTb.Text;
                    //checkFormular(varList, formularTb.Text); TODO:넣기
                    FlowPart fp = new optPart(formula);
                    fp.compoId = View.GenerateViewId();
                    fp.index = currentBlock.Count;

                    Button optflow = new Button(this)
                    {
                        Text = "출력",
                        Tag = currentBlock.Count,
                        Id = fp.compoId
                    };
                    optflow.SetTextColor(Color.Rgb(0, 0, 0));
                    optflow.SetBackgroundColor(Color.Rgb(255, 72, 72));
                    optflow.SetMinHeight(width / 5);
                    optflow.SetMinWidth(width / 5);
                    optflow.SetTextSize(ComplexUnitType.Dip, 25);
                    optflow.Click += (sednder, Dialo) =>
                    {
                        int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
                        View la = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
                        TextView mN = (TextView)la.FindViewById(Resource.Id.MenuName);
                        mN.Text = "출력 수식";
                        LinearLayout ll = (LinearLayout)la.RootView;
                        Button delb = new Button(this) { Text = "삭제" };
                        delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
                        delb.SetTextColor(Color.Rgb(0, 0, 0));
                        delb.Click += delegate
                        {
                            currentBlock.RemoveAt(index);
                            currentGrid.RemoveViewAt(index);
                            dialogger.Cancel();
                        };
                        ll.AddView(delb);
                        dialog.SetView(la);
                        EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                        formTb.Text = ((optPart)(currentBlock[index])).formula;
                        dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                        {
                            try
                            {
                                string form = formTb.Text;
                                checkFormular(varList, formTb.Text);
                                currentBlock[index] = new optPart(formula);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        });
                        dialogger = dialog.Create();
                        dialogger.Show();
                    };
                    currentGrid.AddView(optflow);
                    currentBlock.Add(fp);
                }
                catch (Exception ee)
                {
                    Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                }
            });
            dialogger = dialog.Create();
            dialogger.Show();
        }

        private void Calcbtn_Click(object s, EventArgs ea)
        {
            View layout = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
            dialog.SetView(layout);
            TextView menuName = (TextView)layout.FindViewById(Resource.Id.MenuName);
            menuName.Text = "계산식";
            EditText formularTb = (EditText)layout.FindViewById(Resource.Id.formular);
            //레이아웃설정
            dialog.SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
            {
                try
                {
                    codePart currentBlock = GetCurrentBlock();
                    GridLayout currentGrid = GetCurrentGrid();
                    string formula = formularTb.Text;
                    checkFormular(varList, formularTb.Text);
                    FlowPart fp = new calculationPart(formula);
                    fp.compoId = View.GenerateViewId();
                    fp.index = currentBlock.Count;

                    Button calcflow = new Button(this)
                    {
                        Text = "연산",
                        Tag = currentBlock.Count,
                        Id = fp.compoId
                    };
                    calcflow.SetTextColor(Color.Rgb(0, 0, 0));
                    calcflow.SetBackgroundColor(Color.Rgb(137, 46, 228));
                    calcflow.SetMinHeight(width / 5);
                    calcflow.SetMinWidth(width / 5);
                    calcflow.SetTextSize(ComplexUnitType.Dip, 25);
                    calcflow.Click += (sednder, Dialo) =>
                    {
                        int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
                        View la = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
                        TextView mN = (TextView)la.FindViewById(Resource.Id.MenuName);
                        mN.Text = "계산식";
                        LinearLayout ll = (LinearLayout)la.RootView;
                        Button delb = new Button(this) { Text = "삭제" };
                        delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
                        delb.SetTextColor(Color.Rgb(0, 0, 0));
                        delb.Click += delegate
                        {
                            currentBlock.RemoveAt(index);
                            currentGrid.RemoveViewAt(index);
                            dialogger.Cancel();
                        };
                        ll.AddView(delb);
                        dialog.SetView(la);
                        EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                        formTb.Text = ((calculationPart)(currentBlock[index])).formula;
                        dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                        {
                            try
                            {
                                string form = formTb.Text;
                                checkFormular(varList, formTb.Text);
                                currentBlock[index] = new calculationPart(formula);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        });
                        dialogger = dialog.Create();
                        dialogger.Show();
                    };
                    currentBlock.Add(fp);
                    currentGrid.AddView(calcflow);
                }
                catch (Exception ee)
                {
                    Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                }
            });
            dialogger = dialog.Create();
            dialogger.Show();
        }        
        #endregion

        #region CHECK_FUNCTION
        bool checkFormular(List<variable> vars, string s)
        {
            /*
            Interpreter i = new Interpreter();
            foreach (variable ji in vars)
            {
                i.SetVariable(ji.name, ji.value, ji.type == type.letter ? typeof(string) : typeof(double));
            }
            try
            {
                i.Eval(s);
            }
            catch (Exception e)
            {
                throw e;
            }*/
            return true;
        }
        bool checkCondition(List<variable> vars, string s)
        {
            Interpreter i = new Interpreter();
            foreach (variable ji in vars)
            {
                i.SetVariable(ji.name, ji.value, ji.type == type.letter ? typeof(string) : typeof(double));
            }
            try
            {
                i.Eval<bool>(s);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
        bool checkVar(DefType t, string name, ref string value)
        {
            if (!Regex.IsMatch(name, "^[a-z_]\\w*$"))
            {
                throw new Exception("변수이름이 형식에 맞지 않습니다.");
            }
            if (t == DefType.Number)
            {
                decimal temp;
                if (!decimal.TryParse(value, out temp))
                {
                    throw new Exception("변수가 숫자가 아닙니다.");
                }
            }
            else if (t == DefType.String)
            {
                value = value.Replace("\\", "\\\\");
                value = value.Replace("'", "\\'");
                value = value.Replace("\"", "\\\"");
            }
            else
            {
                throw new Exception("UnKnownERROR 0x1.");
            }

            return true;
        }
        #endregion
    }
}