using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace BreadMachine.Android
{
    public enum Status
    {
        Running, Stop, Pause, WaitForInput, Break
    }
    enum Inputtype { number, str };
    public class BMachine : IDisposable
    {
        /// <summary>
        /// 현재 진행중인 블럭의 위치를 알려줍니다.
        /// </summary>
        public int blockPoint;

        private XDocument currentCode;
        private List<XElement> codeList;

        /// <summary>
        /// 출력 호출시 사용될 함수입니다.
        /// </summary>
        public Action<string> onPrint;

        /// <summary>
        /// 가상머신의 상태입니다
        /// </summary>
        public Status status;


        /// <summary>
        /// 코드가 subMachine에있는지 알려줍니다
        /// </summary>
        bool isSub;

        private string input = "";
        private string inputreadyvarname = "";
        private Inputtype inputt;
        private Interpreter evaler;
        private BMachine subBm;

        public BMachine(string codeBlock, Action<string> onPrint = null)
        {
            blockPoint = 0;
            isSub = false;
            currentCode = XDocument.Parse(codeBlock);
            codeList = new List<XElement>(currentCode.Root.Elements());
            //varList = new List<Variable>();
            status = Status.Stop;
            this.onPrint = onPrint;
            evaler = new Interpreter();
        }

        public BMachine(XDocument codeBlock, Action<string> onPrint = null)
        {
            blockPoint = 0;
            isSub = false;
            currentCode = codeBlock;
            codeList = new List<XElement>(codeBlock.Root.Elements());
            //varList = new List<Variable>();
            status = Status.Stop;
            this.onPrint = onPrint;
            evaler = new Interpreter();
        }

        /// <summary>
        /// 코드를 한 블럭 실행시킵니다.
        /// </summary>
        public void Step()
        {
            XElement curline = codeList[blockPoint++];
            switch (curline.Name.LocalName)
            {
                case "def":
                    if (curline.Attribute("type").Value == "0")
                    {
                        evaler.SetVariable(curline.Value, long.Parse(curline.Attribute("value").Value));
                    }
                    else
                    {
                        evaler.SetVariable(curline.Value, curline.Attribute("value").Value);
                    }
                    break;
                case "calc":
                    evaler.SetVariable(curline.Value.Split('=')[0], evaler.Eval(curline.Value.Split('=')[1]));
                    break;
                case "sel":
                    if (curline.Attribute("else").Value == "false")
                    {
                        if (evaler.Eval<bool>(curline.Attribute("con").Value))
                        {
                            subBm = new BMachine(new XDocument(curline.Elements().First()), onPrint);
                            isSub = true;
                            subBm.evaler = this.evaler;
                            subBm.Runner();
                            evaler = subBm.evaler;
                        }
                    }
                    else
                    {
                        if (evaler.Eval<bool>(curline.Attribute("con").Value))
                        {
                            subBm = new BMachine(new XDocument(curline.Elements().First()), onPrint);
                            isSub = true;
                            subBm.evaler = evaler;
                            subBm.Runner();
                            evaler = subBm.evaler;
                        }
                        else
                        {
                            subBm = new BMachine(new XDocument(curline.Elements().Skip(1).First()), onPrint);
                            isSub = true;
                            subBm.evaler = evaler;
                            subBm.Runner();
                            evaler = subBm.evaler;
                        }
                    }
                    isSub = false;
                    break;
                case "loop":
                    while (evaler.Eval<bool>(curline.Attribute("con").Value))
                    {
                        subBm = new BMachine(new XDocument(curline.Elements().First()), onPrint);
                        isSub = true;
                        subBm.evaler = this.evaler;
                        subBm.Runner();
                        evaler = subBm.evaler;
                        if (subBm.status == Status.Break)
                            break;
                    }
                    isSub = false;
                    break;
                case "ivk":
                    Ivk(curline);
                    break;
                case "break":
                    status = Status.Break;
                    break;
            }
        }

        /// <summary>
        /// 입력 대기중 실행되면 값을 입력합니다.
        /// </summary>
        /// <param name="a">입력될 값입니다</param>
        /// <returns>true면 입력이 처리된겁니다</returns>
        public bool Input(string a)
        {
            if(isSub)
            {
                return subBm.Input(a);
            }
            else
            {
                input = status == Status.WaitForInput ? a : input;
                return status == Status.WaitForInput;
            }
        }

        private void WaitForInput()
        {
            while (input == "") { }
            status = Status.Running;
            gotInput();
        }

        private void gotInput()
        {
            if (inputt == Inputtype.number)
            {
                evaler.SetVariable(inputreadyvarname, long.Parse(input));
            }
            else
            {
                evaler.SetVariable(inputreadyvarname, input);
            }
        }

        private void Ivk(XElement ivkCmd)
        {
            switch (ivkCmd.Attribute("type").Value)
            {
                case "0":
                    onPrint?.Invoke(evaler.Eval<object>(ivkCmd.Value).ToString());
                    break;
                case "1":
                    status = Status.WaitForInput;
                    inputreadyvarname = ivkCmd.Value;
                    inputt = ivkCmd.Attribute("vtype").Value == "0" ? Inputtype.number : Inputtype.str;
                    WaitForInput();
                    break;
            }
        }
        public void Run()
        {
            ThreadStart s = new ThreadStart(() => Runner());
            mainThread = new Thread(s);
            mainThread.Start();
        }

        private void Runner()
        {
            object lockObject = new object();
            status = Status.Running;
            foreach (var meaningless_val in codeList)
            {
                if (status == Status.Break)
                    break;
                lock (lockObject)
                {
                    Step();
                }
            }

            status = Status.Stop;
        }

        #region IDisposable implementation with finalizer
        private bool isDisposed = false;
        private Thread mainThread;

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    mainThread.Abort();
                    //if (_wplayer != null) _wplayer.Dispose();
                }
            }
            isDisposed = true;
        }
        #endregion
    }
}
