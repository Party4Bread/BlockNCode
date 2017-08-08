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
    public enum type { number, letter };
    public struct variable
    {
        public int id;
        public string name;
        public object value;
        public type type;
    }
    public enum Activitycode { main,loop,loopfix,condition,conditionfix};
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,Label = "BNC0D3", WindowSoftInputMode = Android.Views.SoftInput.AdjustPan |SoftInput.StateAlwaysHidden, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        #region CREATE_VAR
        private ArrayAdapter<string> m_Adapter;
        private EditText conIpt;
        private ListView conOpt;
        private Button consumit,defbtn,calcbtn,loopbtn,optbtn,savebtn,constbtn;        
        bool codeShow;
        private SlidingDrawer slider;
        List<FlowPart> codeBlock;
        int width;
        AlertDialog.Builder dialog;
        AlertDialog dialogger;
        public GridLayout gridflow { get; private set; }
        List<variable> varList;
        private Button loadbtn;
        private BMachine vm;
        InputMethodManager mgr;
        #endregion
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            #region SET_VAR_VALUE
            codeShow = Application.Context.GetSharedPreferences("BNCODE", FileCreationMode.Private).GetBoolean("codeShow", false);
            conOpt = FindViewById<ListView>(Resource.Id.consoleOpt);
            conIpt = FindViewById<EditText>(Resource.Id.consoleIpt);
            consumit = FindViewById<Button>(Resource.Id.consoleSummit);
            defbtn = FindViewById<Button>(Resource.Id.def_button);
            calcbtn = FindViewById<Button>(Resource.Id.calc_button);
            optbtn = FindViewById<Button>(Resource.Id.opt_button);
            loadbtn = FindViewById<Button>(Resource.Id.load_button);
            gridflow = FindViewById<GridLayout>(Resource.Id.gridView1);
            slider = FindViewById<SlidingDrawer>(Resource.Id.slidingDrawer1);
            savebtn = FindViewById<Button>(Resource.Id.save_button);
            loopbtn = FindViewById<Button>(Resource.Id.loop_button);
            constbtn = FindViewById<Button>(Resource.Id.conditianal_button);
            DisplayMetrics displayMetrics = new DisplayMetrics();
            dialog = new AlertDialog.Builder(this);
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            width = displayMetrics.WidthPixels;
            mgr = (InputMethodManager)GetSystemService(InputMethodService);
            //preset
            m_Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            conOpt.Adapter = m_Adapter;
            conIpt.ImeOptions = Android.Views.InputMethods.ImeAction.Done;
            //value4code
            codeBlock = new List<FlowPart>();
            varList = new List<variable>();
            #endregion

            #region COMPONENT_EVENT
            conIpt.EditorAction += ConIpt_EditorAction;
            slider.DrawerOpen += Slider_DrawerOpen;
            slider.DrawerClose += Slider_DrawerClose; ;
            consumit.Click += Consumit_Click;
            defbtn.Click += Defbtn_Click;
            calcbtn.Click += Calcbtn_Click;
            optbtn.Click += Optbtn_Click;
            savebtn.Click += Savebtn_Click;
            loadbtn.Click += Loadbtn_ClickAsync;
            loopbtn.Click += Loopbtn_Click;
            constbtn.Click += Constbtn_Click;
            #endregion
            /*
            LinearLayout ll = (LinearLayout)la.RootView;
            Button delb = new Button(this) { Text = "삭제" };
            delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
            delb.SetTextColor(Color.Rgb(0, 0, 0));
            delb.Click += delegate {
                codeBlock.RemoveAt(index);
                gridflow.RemoveViewAt(index);
            };
            ll.AddView(delb);*/
        }

        private void Constbtn_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ConStActivity));
            TempStorage.tempOBJ = varList;
            StartActivityForResult(i, (int)Activitycode.condition);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch ((Activitycode)requestCode)
            {
                case Activitycode.loop:
                    if (TempStorage.tempFP == null)
                    {
                        return;
                    }

                    FlowPart fp = TempStorage.tempFP;
                    
                    fp.compoId = View.GenerateViewId();
                    fp.index = codeBlock.Count;
                    codeBlock.Add(fp);

                    Button loopflow = new Button(this)
                    {
                        Text = "반복",
                        Tag = codeBlock.Count - 1,
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
                        StartActivityForResult(i, (int)Activitycode.loopfix);
                        //loopfixactivity need vvcccc
                    };
                    gridflow.AddView(loopflow);
                    break;
                case Activitycode.condition:
                    if (TempStorage.tempFP == null)
                    {
                        return;
                    }
                    FlowPart fpc = TempStorage.tempFP;

                    fpc.compoId = View.GenerateViewId();
                    fpc.index = codeBlock.Count;
                    codeBlock.Add(fpc);

                    Button conditionflow = new Button(this)
                    {
                        Text = "선택",
                        Tag = codeBlock.Count - 1,
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
                        StartActivityForResult(i, (int)Activitycode.conditionfix);
                        //loopfixactivity need vvcccc
                    };
                    gridflow.AddView(conditionflow);
                    break;
            }
        }

        private void Loopbtn_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(LoopActivity));
            TempStorage.tempOBJ = varList;
            StartActivityForResult(i,(int)Activitycode.loop);
        }

        private async void Loadbtn_ClickAsync(object sender, EventArgs e)
        {
            FileData loadxmlfile = await CrossFilePicker.Current.PickFile();
            XmlDocument loaddoc = new XmlDocument();
            loaddoc.LoadXml(Encoding.UTF8.GetString(loadxmlfile.DataArray));
            foreach(XmlElement i in loaddoc.ChildNodes)
            {
                switch(i.OuterXml)
                {
                    case "def":
                        //FlowPart a = new definePart();
                        break;
                    case "calc":
                        break;
                    case "loop":
                        break;
                    case "opt":
                        break;
                    case "":
                        break;
                }
            }
        }

        private void Savebtn_Click(object sender, EventArgs e)
        {
            XmlDocument savedoc = new XmlDocument();
            //CrossFilePicker.Current.SaveFile(new FileData() {})
            View layout = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
            dialog.SetView(layout);
            TextView menuName = (TextView)layout.FindViewById(Resource.Id.MenuName);
            menuName.Text = "파일이름";
            EditText filenameTb = (EditText)layout.FindViewById(Resource.Id.formular);
            //레이아웃설정
            dialog.SetPositiveButton(Android.Resource.String.Ok, (sendder, ee) =>
            {
                foreach (FlowPart f in codeBlock)
                    savedoc.AppendChild(f.XmlDigest(savedoc));
                savedoc.Save(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/BNC_SAVE/" + filenameTb.Text);
                Toast.MakeText(this, "Done!", ToastLength.Short).Show();
            });
            dialogger = dialog.Create();
            dialogger.Show();
        }

          #region component_Function
        private void Consumit_Click(object sender, EventArgs e)
        {
            //VTvm.vm.io.read(conIpt.Text);
            m_Adapter.Add(conIpt.Text);
            conIpt.Text = "";
        }

        private void Defbtn_Click(object s, EventArgs eA)
        {
            List<variable> newVal = new List<variable>();
            View layout = LayoutInflater.Inflate(Resource.Layout.defSetting, null);
            LinearLayout ll = layout.FindViewById<LinearLayout>(Resource.Id.defRoot);

            #region addExistingVars
            foreach (var i in varList)
            {
                var vl = LayoutInflater.Inflate(Resource.Layout.defVarFrag, null);
                vl.Id = i.id;
                TextView varName = vl.FindViewById<TextView>(Resource.Id.varNameFrag);
                TextView varValue = vl.FindViewById<TextView>(Resource.Id.varValueFrag);
                TextView varType = vl.FindViewById<TextView>(Resource.Id.varTypeFrag);
                Button delBtn = vl.FindViewById<Button>(Resource.Id.varDelFragBtn);
                varName.Text = i.name;
                varValue.Text = i.value.ToString();
                if (i.type == type.letter)
                    varType.Text = "문자";
                else
                    varType.Text = "숫자";
                delBtn.Click += delegate
                {
                    
                    //EVENT who is the the del 
                };

                //아이디지정된 엘리먼트에 값추가.--- 기억:이건 있던 변수 추가하는것뿐

                ll.AddView(vl);
            } 
            #endregion
            Button addValBtn = layout.FindViewById<Button>(Resource.Id.varAddbtn);
            addValBtn.Click += delegate {
                variable nv = new variable();
                var dVP = LayoutInflater.Inflate(Resource.Layout.defVarPart, null);

                AlertDialog.Builder dlg = new AlertDialog.Builder(this);
                dlg.SetView(dVP);
                //변수 변경점 적용 기능
                dlg.SetPositiveButton(Android.Resource.String.Ok, (sender, e) => {
                    EditText newvarname = dVP.FindViewById<EditText>(Resource.Id.varName);
                    EditText newvarvalue = dVP.FindViewById<EditText>(Resource.Id.varValue);
                    RadioButton nstr = dVP.FindViewById<RadioButton>(Resource.Id.StrRadio);
                    RadioButton nnum = dVP.FindViewById<RadioButton>(Resource.Id.NumRadio);
                    //TODO: 빈칸 검증
                    //layout(ll)에 추가할 것
                    var dVF = LayoutInflater.Inflate(Resource.Layout.defVarFrag, null);
                    dVF.Id = nv.id = View.GenerateViewId();
                    TextView vvarName = dVF.FindViewById<TextView>(Resource.Id.varNameFrag);
                    TextView vvarValue = dVF.FindViewById<TextView>(Resource.Id.varValueFrag);
                    TextView vvarType = dVF.FindViewById<TextView>(Resource.Id.varTypeFrag);
                    Button vdelBtn = dVF.FindViewById<Button>(Resource.Id.varDelFragBtn);
                    vvarName.Text = nv.name = newvarname.Text;
                    vvarValue.Text = (string)(nv.value = newvarvalue.Text);
                    if (nstr.Checked == true)
                    {
                        vvarType.Text = "문자";
                        nv.type = type.letter;
                    }
                    else
                    {
                        vvarType.Text = "숫자";
                        nv.type = type.number;
                    }
                    vdelBtn.Click += delegate {
                        //EVENT who is the the del 
                    };
                    ll.AddView(dVF);
                    newVal.Add(nv);
                });

                AlertDialog dlgr = dlg.Create();
                dlgr.Show();
            };
  
            dialog.SetView(layout);
            dialog.SetPositiveButton(Android.Resource.String.Ok, (sender, e) => {
                List<variable> oldvariable = varList;
                varList = new List<variable>();
                foreach(variable ov in oldvariable)
                {
                    variable sv = new variable();
                    sv.name = ll.FindViewById<LinearLayout>(ov.id).FindViewById<TextView>(Resource.Id.varNameFrag).Text;
                    sv.value = ll.FindViewById<LinearLayout>(ov.id).FindViewById<TextView>(Resource.Id.varValueFrag).Text;
                    sv.type = ll.FindViewById<LinearLayout>(ov.id).FindViewById<TextView>(Resource.Id.varTypeFrag).Text=="숫자" ? type.number : type.letter;
                    sv.id  = ov.id;
                    bool isExist = false;
                    varList.ForEach((variable v) => {
                        if (v.name == sv.name)
                            isExist = true;
                    });
                    if (sv.name != "" && (string)sv.value != "" && !isExist)
                    {
                        varList.Add(sv);
                    }
                    else
                    {
                        // Toast 문제있소!
                    }
                }
                foreach(variable nv in newVal)
                {
                    variable sv = nv;
                    //ToDo:error fix
                    //sv.name = ll.FindViewById<LinearLayout>(nv.id).FindViewById<EditText>(Resource.Id.varName).Text;
                    //sv.value = ll.FindViewById<LinearLayout>(nv.id).FindViewById<EditText>(Resource.Id.varValue).Text;
                    //sv.type = ll.FindViewById<LinearLayout>(nv.id).FindViewById<RadioButton>(Resource.Id.StrRadio).Checked ? type.letter : type.number;
                    //sv.id = nv.id;
                    bool isExist = false;
                    varList.ForEach((variable v) => {
                        if (v.name == sv.name)
                            isExist = true;
                    });
                    if (sv.name != "" && (string)sv.value!="" && !isExist)
                    {
                        varList.Add(sv);
                    }
                    else
                    {
                        // Toast 문제있소!
                    }
                }
            });
            #region temp
            //EditText varValue = (EditText)layout.FindViewById(Resource.Id.varValue),
            //varName = (EditText)layout.FindViewById(Resource.Id.varName);
            //RadioButton varNum = (RadioButton)layout.FindViewById(Resource.Id.varNumber),
            //varStr = (RadioButton)layout.FindViewById(Resource.Id.varString);
            //dialog.SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
            //{
            //    try
            //    {
            //        string value = varValue.Text;
            //        string name = varName.Text;
            //        DefType d = new DefType();
            //        if (varNum.Checked)
            //        {
            //            d = DefType.Number;
            //        }
            //        else if (varStr.Checked)
            //        {
            //            d = DefType.String;
            //        }
            //        checkVar(d, name, ref value);
            //        FlowPart fp = new definePart(d, name, value)
            //        {
            //            compoId = View.GenerateViewId(),
            //            index = codeBlock.Count
            //        };
            //        codeBlock.Add(fp);
            //        varList.Add(name);
            //        Button defflow = new Button(this)
            //        {
            //            Text = "선언",
            //            Tag = codeBlock.Count - 1,
            //            Id = fp.compoId
            //        };
            //        defflow.SetTextColor(Color.Rgb(0, 0, 0));
            //        defflow.SetBackgroundColor(Color.Rgb(128, 0, 128));
            //        defflow.SetMinHeight(width / 5);
            //        defflow.SetMinWidth(width / 5);
            //        defflow.SetTextSize(ComplexUnitType.Dip, 25);
            //        defflow.Click += (sednder, Dialo) =>
            //        {
            //            int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
            //            View la = LayoutInflater.Inflate(Resource.Layout.defSetting, null);
            //            dialog.SetView(la);
            //            EditText vV = (EditText)la.FindViewById(Resource.Id.varValue),
            //            vN = (EditText)la.FindViewById(Resource.Id.varName);
            //            RadioButton vNu = (RadioButton)la.FindViewById(Resource.Id.varNumber),
            //            vSt = (RadioButton)la.FindViewById(Resource.Id.varString);
            //            vV.Text = ((definePart)codeBlock[index]).defValue;
            //            vN.Text = ((definePart)codeBlock[index]).defName;
            //            LinearLayout ll = (LinearLayout)la.RootView;
            //            Button delb = new Button(this) { Text = "삭제" };
            //            delb.SetBackgroundColor(Color.Rgb(255, 0, 0));
            //            delb.SetTextColor(Color.Rgb(0, 0, 0));
            //            delb.Click += delegate
            //            {
            //                codeBlock.RemoveAt(index);
            //                gridflow.RemoveViewAt(index);
            //                dialogger.Cancel();
            //            };
            //            ll.AddView(delb);

            //            if (((definePart)codeBlock[index]).defType == DefType.Number)
            //            {
            //                vNu.Checked = true;
            //            }
            //            else if (vSt.Checked)
            //            {
            //                vSt.Checked = true;
            //            }

            //            dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
            //            {
            //                string v = vV.Text;
            //                string n = vN.Text;
            //                DefType dd = new DefType();
            //                if (vNu.Checked)
            //                {
            //                    dd = DefType.Number;
            //                }
            //                else if (vSt.Checked)
            //                {
            //                    dd = DefType.String;
            //                }
            //                try
            //                {
            //                    checkVar(dd, n, ref v);
            //                    codeBlock[index] = new definePart(dd, n, v);
            //                }
            //                catch (Exception ee)
            //                {
            //                    Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
            //                }
            //            });
            //            dialogger = dialog.Create();
            //            dialogger.Show();
            //        };
            //        gridflow.AddView(defflow);
            //    }
            //    catch (Exception ee)
            //    {
            //        Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
            //    }
            //});
            #endregion
            //완료 이벤트 개발 우선순위 1 변수초기화후 새로 다시추가 2 사용된 변수인지 체크
            dialogger = dialog.Create();
            dialogger.Show();
            
        }

        private void Slider_DrawerOpen(object sender, EventArgs e)
        {
            if (codeShow)
            {
                foreach (FlowPart i in codeBlock)
                {
                    m_Adapter.Add(i.Digest());
                }
            }
            else
            {
                XmlDocument code = new XmlDocument();
                XmlElement cd = code.CreateElement("code");
                List<FlowPart> varCode = new List<FlowPart>();
                foreach (variable i in varList)
                {
                    varCode.Add(new definePart(i.type == type.letter ? DefType.String : DefType.Number, i.name, i.value.ToString()));
                }
                varCode.AddRange(codeBlock);
                codeBlock = varCode;
                foreach (FlowPart i in codeBlock)
                {
                    cd.AppendChild(i.XmlDigest(code));
                }
                code.AppendChild(cd);
                vm = new BMachine(code.OuterXml, (string o) => { RunOnUiThread(delegate { m_Adapter.Add(o); }); });
                vm.Run();
                /*
                Vaquita4android.Parser parser = new Vaquita4android.Parser();
                VTvm.vm = new Vaquita4android.vm.Machine();
                VTvm.vm.load(parser.compile(code));
                VTvm.vm.onPrint += (object o) => { m_Adapter.Add(o.ToString()); };
                VTvm.vm.run();*/
            }
        }

        private void Slider_DrawerClose(object sender, EventArgs e)
        {
            m_Adapter.Clear();
            vm.Dispose();
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
                    string formula = formularTb.Text;
                    //checkFormular(varList, formularTb.Text); TODO:넣기
                    FlowPart fp = new optPart(formula);
                    fp.compoId = View.GenerateViewId();
                    fp.index = codeBlock.Count;
                    codeBlock.Add(fp);

                    Button optflow = new Button(this)
                    {
                        Text = "출력",
                        Tag = codeBlock.Count - 1,
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
                            codeBlock.RemoveAt(index);
                            gridflow.RemoveViewAt(index);
                            dialogger.Cancel();
                        };
                        ll.AddView(delb);
                        dialog.SetView(la);
                        EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                        formTb.Text = ((optPart)(codeBlock[index])).formula;
                        dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                        {
                            try
                            {
                                string form = formTb.Text;
                                //checkFormular(varList, formularTb.Text);
                                codeBlock[index] = new optPart(formula);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        });
                        dialogger = dialog.Create();
                        dialogger.Show();
                    };
                    gridflow.AddView(optflow);
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
                    string formula = formularTb.Text;
                    checkFormular(varList, formularTb.Text);
                    FlowPart fp = new calculationPart(formula);
                    fp.compoId = View.GenerateViewId();
                    fp.index = codeBlock.Count;
                    codeBlock.Add(fp);

                    Button calcflow = new Button(this)
                    {
                        Text = "연산",
                        Tag = codeBlock.Count - 1,
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
                            codeBlock.RemoveAt(index);
                            gridflow.RemoveViewAt(index);
                            dialogger.Cancel();
                        };
                        ll.AddView(delb);
                        dialog.SetView(la);
                        EditText formTb = (EditText)la.FindViewById(Resource.Id.formular);
                        formTb.Text = ((calculationPart)(codeBlock[index])).formula;
                        dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                        {
                            try
                            {
                                string form = formTb.Text;
                                checkFormular(varList, formularTb.Text);
                                codeBlock[index] = new calculationPart(formula);
                            }
                            catch (Exception ee)
                            {
                                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                            }
                        });
                        dialogger = dialog.Create();
                        dialogger.Show();
                    };
                    gridflow.AddView(calcflow);
                }
                catch (Exception ee)
                {
                    Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                }
            });
            dialogger = dialog.Create();
            dialogger.Show();
        }

        private void ConIpt_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
            {
                //VTvm.vm.io.read(((EditText)sender).Text);
                m_Adapter.Add(((EditText)sender).Text);
                conIpt.Text = "";
            }
        }
        #endregion

        #region CHECK_FUNCTION
        bool checkFormular(List<variable> vars, string s)
        {
            #region unused code
            /*
            string st = s;
            List<string> var = new List<string>();
            List<String> v = new List<String>();
            List<String> o = new List<String>();
            List<String> op = new List<String>();
            foreach(variable i in vars)
            {
                var.Add(i.name);
            }
            op.Add("+");
            op.Add("-");
            op.Add("*");
            op.Add("/");
            op.Add("^");
            List<String> f = new List<String>();
            //int x = 0;
            int length = st.Length;
            String tem = "";
            int x = 0;
            for (int y = 0; y < length; y++)
            {
                // String tem="";
                String temp = st.Substring(x, 1);
                if (temp == "+" || temp == "-" || temp == "*" || temp == "/")
                {
                    if (!(tem.Length == 0))
                    {
                        v.Add(tem);
                    }
                    o.Add(st.Substring(x, 1));
                    st = st.Remove(0, x + 1);
                    x = 0;
                    tem = "";
                }
                else if (temp == "(" || temp == ")")
                {
                    st = st.Remove(0, x + 1);
                    f.Add(temp + " " + y);
                    if (tem != "")
                    {
                        v.Add(tem);
                    }
                    tem = "";
                    x = 0;
                }
                else if (temp == "=")
                {
                    if (tem != "")
                    { v.Add(tem); }
                    o.Add(st.Substring(x, 1));
                    st = st.Remove(0, x + 1);
                    x = 0;
                    tem = "";
                    if (st.Contains("+") || st.Contains("-") || st.Contains("*") || st.Contains("/"))
                    {
                        throw new Exception("= 뒤는 한개의 항만 올 수 있습니다.");
                    }
                    int dump;
                    if (int.TryParse(st, out dump))
                    {
                        throw new Exception("= 뒤에 변수가 아닌 상수 " + dump + "가 사용되었습니다.");
                    }
                    v.Add(st);
                }
                else
                {
                    tem = tem + temp;
                    x++;
                }
            }

            v.Sort();
            
            var.Sort();
            if (!o.Contains("="))
            {
                throw new Exception("= 연산자가 인식되지 않았습니다.");
            }
            foreach (string t in v)
            {
                Decimal temp;
                bool tryparse = Decimal.TryParse(t, out temp);
                if (tryparse)
                {
                    continue;
                }
                bool pass = false;
                foreach (string te in var)
                {
                    if (t == te)
                    {
                        pass = true;
                        break;
                    }
                }
                if (!pass)
                {
                    throw new Exception("명시되지 않은 변수: '" + t + "'(을)를 사용했습니다.");
                }

            }
            if (f.Count != 0)
            {
                if (f[0].Contains("("))
                {
                    for (int te = 0; te < f.Count; te++)
                    {
                        String ss = f[te];
                        f.Remove(ss);
                        if (!(te == 0))
                        {
                            te--;
                        }
                        string[] temp = ss.Split(' ');
                        if (temp[0] == "(")
                        {
                            bool p = false;
                            for (int tee = te; tee < f.Count; tee++)
                            {
                                String sss = f[tee];
                                string[] tttt = sss.Split(' ');
                                if (tttt[0] == ")")
                                {
                                    if (int.Parse(tttt[1]) > int.Parse(temp[1]))
                                    {
                                        p = true;
                                        f.Remove(sss);
                                        if (te != 0)
                                            tee--;
                                        te--;
                                        break;
                                    }
                                }
                            }
                            if (!p)
                            {
                                throw new Exception("괄호의 짝이 맞지 않습니다.");
                            }
                        }
                    }
                }
            }
            if (f.Count != 0)
            {
                throw new Exception("괄호의 짝이 맞지 않습니다.");
            }
            foreach (string t in o)
            {
                bool pass = false;

                foreach (string te in op)
                {
                    if (t.Equals("="))
                    {
                        pass = true;
                        break;
                    }
                    if (t == te)
                    {
                        pass = true;
                        break;
                    }
                }
                if (!pass)
                {
                    throw new Exception("명시되지 않은 연산자: '" + t + "'(을)를 사용했습니다.");
                }
            }
            return true;
            */
#endregion
            /*
            Interpreter i = new Interpreter();
            foreach(variable ji in vars)
            {
                i.SetVariable(ji.name, ji.value, ji.type == type.letter ? Type.GetType("string") : Type.GetType("double"));
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