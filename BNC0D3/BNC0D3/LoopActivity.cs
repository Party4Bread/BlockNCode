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

namespace BNC0D3
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Label = "BNC0D3", WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | SoftInput.StateAlwaysHidden, Theme = "@android:style/Theme.NoTitleBar")]
    public class LoopActivity : Activity
    {
        #region CREATE_VAR
        private ArrayAdapter<string> m_Adapter;
        private Button defbtn;
        private Button calcbtn;
        private Button optbtn;
        private Button exitbtn;
        bool codeShow;
        private SlidingDrawer slider;
        codePart codeBlock;
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
            defbtn = FindViewById<Button>(Resource.Id.def_button);
            calcbtn = FindViewById<Button>(Resource.Id.calc_button);
            optbtn = FindViewById<Button>(Resource.Id.opt_button);
            loadbtn = FindViewById<Button>(Resource.Id.load_button);
            gridflow = (GridLayout)FindViewById(Resource.Id.gridView1);
            slider = (SlidingDrawer)FindViewById(Resource.Id.slidingDrawer1);
            exitbtn = FindViewById<Button>(Resource.Id.exitbtn);
            DisplayMetrics displayMetrics = new DisplayMetrics();
            dialog = new AlertDialog.Builder(this);
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            width = displayMetrics.WidthPixels;
            mgr = (InputMethodManager)GetSystemService(InputMethodService);
            //preset
            m_Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            //value4code
            codeBlock = new codePart();
            varList = new List<variable>();
            #endregion

            #region COMPONENT_EVENT
            defbtn.Click += Defbtn_Click;
            calcbtn.Click += Calcbtn_Click;
            optbtn.Click += Optbtn_Click;
            exitbtn.Click += Exitbtn_Click;
            #endregion
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
            Intent intent = new Intent();
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(codeBlock.XmlDigest(doc));
            intent.PutExtra("code",doc.OuterXml);
            //Upper code might be useless.
            TempStorage.tempFP = codeBlock;
            SetResult(Result.Ok, intent);
            Finish();
        }
        #region component_Function

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
                TextView varType = vl.FindViewById<TextView>(Resource.Id.varValueFrag);
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
                foreach (variable ov in oldvariable)
                {
                    variable sv = new variable();
                    sv.name = ll.FindViewById<LinearLayout>(ov.id).FindViewById<EditText>(Resource.Id.varName).Text;
                    sv.value = ll.FindViewById<LinearLayout>(ov.id).FindViewById<EditText>(Resource.Id.varValue).Text;
                    sv.type = ll.FindViewById<LinearLayout>(ov.id).FindViewById<RadioButton>(Resource.Id.StrRadio).Checked ? type.letter : type.number;
                    sv.id = ov.id;
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
                foreach (variable nv in newVal)
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
                    if (sv.name != "" && (string)sv.value != "" && !isExist)
                    {
                        varList.Add(sv);
                    }
                    else
                    {
                        // Toast 문제있소!
                    }
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
        #endregion

        #region CHECK_FUNCTION
        bool checkFormular(List<variable> vars, string s)
        {
            Interpreter i = new Interpreter();
            foreach (variable ji in vars)
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