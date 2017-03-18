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
        private Button optbtn;
        private SlidingDrawer slider;
        List<FlowPart> codeBlock;
        AlertDialog.Builder dialog;
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
            optbtn = (Button)FindViewById(Resource.Id.opt_button);
            gridflow = (GridLayout)FindViewById(Resource.Id.gridView1);
            slider = (SlidingDrawer)FindViewById(Resource.Id.slidingDrawer1);
            DisplayMetrics displayMetrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            int height = displayMetrics.HeightPixels;
            int width = displayMetrics.WidthPixels;
            dialog = new AlertDialog.Builder(this);

            //preset
            m_Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            conOpt.Adapter = m_Adapter;
            conIpt.ImeOptions = Android.Views.InputMethods.ImeAction.Done;
            //value4code
            codeBlock = new List<FlowPart>();
            List<string> varList = new List<string>();
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
            consumit.Click += delegate
            {
                //VTvm.vm.io.read(conIpt.Text);
                m_Adapter.Add(conIpt.Text);
                conIpt.Text = "";
            };
            defbtn.Click += delegate
            {
                View layout = LayoutInflater.Inflate(Resource.Layout.defSetting, null);
                dialog.SetView(layout);
                EditText varValue = (EditText)layout.FindViewById(Resource.Id.varValue),
                varName = (EditText)layout.FindViewById(Resource.Id.varName);
                RadioButton varNum = (RadioButton)layout.FindViewById(Resource.Id.varNumber),
                varStr = (RadioButton)layout.FindViewById(Resource.Id.varString);
                dialog.SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
                {
                    try
                    {
                        string value = varValue.Text;
                        string name = varName.Text;
                        DefType d = new DefType();
                        if (varNum.Checked)
                        {
                            d = DefType.Number;
                        }
                        else if (varStr.Checked)
                        {
                            d = DefType.String;
                        }
                        checkVar(d, name, ref value);
                        FlowPart fp = new definePart(d, name, value)
                        {
                            compoId = View.GenerateViewId(),
                            index = codeBlock.Count
                        };
                        codeBlock.Add(fp);
                        varList.Add(name);
                        Button defflow = new Button(this)
                        {
                            Text = "선언",
                            Tag = codeBlock.Count - 1,
                            Id = fp.compoId
                        };
                        defflow.SetTextColor(Color.Rgb(0, 0, 0));
                        defflow.SetBackgroundColor(Color.Rgb(128, 0, 128));
                        defflow.SetMinHeight(width / 5);
                        defflow.SetMinWidth(width / 5);
                        defflow.SetTextSize(ComplexUnitType.Dip, 25);
                        defflow.Click += (sednder, Dialo) =>
                        {
                            int index = Convert.ToInt32(((Button)sednder).Tag.ToString());
                            View la = LayoutInflater.Inflate(Resource.Layout.defSetting, null);
                            dialog.SetView(la);
                            EditText vV = (EditText)la.FindViewById(Resource.Id.varValue),
                            vN = (EditText)la.FindViewById(Resource.Id.varName);
                            RadioButton vNu = (RadioButton)la.FindViewById(Resource.Id.varNumber),
                            vSt = (RadioButton)la.FindViewById(Resource.Id.varString);
                            vV.Text = ((definePart)codeBlock[index]).defValue;
                            vN.Text = ((definePart)codeBlock[index]).defName;

                            if (((definePart)codeBlock[index]).defType == DefType.Number)
                            {
                                vNu.Checked = true;
                            }
                            else if (vSt.Checked)
                            {
                                vSt.Checked = true;
                            }

                            dialog.SetPositiveButton(Android.Resource.String.Ok, delegate
                            {
                                string v = vV.Text;
                                string n = vN.Text;
                                DefType dd = new DefType();
                                if (vNu.Checked)
                                {
                                    dd = DefType.Number;
                                }
                                else if (vSt.Checked)
                                {
                                    dd = DefType.String;
                                }
                                try
                                {
                                    checkVar(dd, n, ref v);
                                    codeBlock[index] = new definePart(dd, n, v);
                                }
                                catch (Exception ee)
                                {
                                    Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                                }
                            });
                            dialog.Show();
                        };
                        gridflow.AddView(defflow);
                    }
                    catch (Exception ee)
                    {
                        Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                    }
                });
                dialog.Show();
            };
            calcbtn.Click += delegate
            {

                View layout = LayoutInflater.Inflate(Resource.Layout.calcSetting, null);
                dialog.SetView(layout);
                TextView menuName = (TextView)layout.FindViewById(Resource.Id.MenuName);
                menuName.Text = "계산식";
                EditText formularTb = (EditText)layout.FindViewById(Resource.Id.formular);
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
                            dialog.SetView(layout);
                            EditText formTb = (EditText)FindViewById(Resource.Id.formular);
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
                            dialog.Show();
                        };
                        gridflow.AddView(calcflow);
                    }
                    catch (Exception ee)
                    {
                        Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                    }
                });
                dialog.Show();
            };
            optbtn.Click += delegate
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
                            dialog.SetView(layout);
                            EditText formTb = (EditText)FindViewById(Resource.Id.formular);
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
                            dialog.Show();
                        };
                        gridflow.AddView(calcflow);
                    }
                    catch (Exception ee)
                    {
                        Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
                    }
                });
                dialog.Show();
            };
        }
        bool checkFormular(List<string> var, string s)
        {
            string st = s;
            List<String> v = new List<String>();
            List<String> o = new List<String>();
            List<String> additionalop = new List<String>();
            additionalop.Add("+");
            additionalop.Add("-");
            additionalop.Add("*");
            additionalop.Add("/");
            additionalop.Add("^");
            //int x = 0;
            int length = st.Length;
            string tem = "";
            int x = 0;
            for (int y = 0; y < length; y++)
            {
                // String tem="";
                string temp = st.Substring(x, 1);
                if (additionalop.Contains(temp))
                {
                    if (temp == "=")
                    {
                        v.Add(tem);
                        o.Add(st.Substring(x, 1));
                        st = st.Remove(0, x + 1);
                        x = 0;
                        tem = "";
                        v.Add(st);
                    }
                    else
                    {
                        v.Add(tem);
                        o.Add(st.Substring(x, 1));
                        st = st.Remove(0, x + 1);
                        x = 0;
                        tem = "";
                    }
                }
                else
                {
                    tem = tem + temp;
                    x++;
                }
            }

            v.Sort();
            var.Sort();
            if (o.Contains("="))
            {
                throw new Exception("= 연산자가 없습니다.");
            }
            foreach (string t in v)
            {
                int temp;
                bool tryparse = int.TryParse(t, out temp);
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

            foreach (string t in o)
            {
                bool pass = false;
                foreach (string te in additionalop)
                {
                    if (t == te || t == "=")
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
                if (decimal.TryParse(value, out temp))
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