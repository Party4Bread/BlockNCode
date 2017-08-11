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
    struct CodeStack
    {
        public List<XElement> codeList;
        public int currentCode;
        public Status currentStatus;
        public bool isloop;
    }
    public class BMachine : IDisposable
    {
        /// <summary>
        /// 현재 진행중인 블럭의 위치를 알려줍니다.
        /// </summary>
        public int blockPoint;

        private XDocument currentCode;

        /// <summary>
        /// 출력 호출시 사용될 함수입니다.
        /// </summary>
        public Action<string> onPrint;
        public Action addsubBM;

        /// <summary>
        /// 가상머신의 상태입니다
        /// </summary>
        public Status status;

        Stack<CodeStack> codestack = new Stack<CodeStack>();

        private string input = "";
        private string inputreadyvarname = "";
        private Inputtype inputt;
        private Interpreter evaler;

        public BMachine(string codeBlock, Action<string> onPrint = null,Action addsub=null)
        {
            blockPoint = 0;
            currentCode = XDocument.Parse(codeBlock);
            addNewCodeStack(new List<XElement>(currentCode.Root.Elements()));
            //varList = new List<Variable>();
            status = Status.Stop;
            this.onPrint = onPrint;
            evaler = new Interpreter();
            addsubBM = addsub;
        }

        public BMachine(XDocument codeBlock, Action<string> onPrint = null,Action addsub=null)
        {
            blockPoint = 0;
            currentCode = codeBlock;
            addNewCodeStack(new List<XElement>(currentCode.Root.Elements()));
            //varList = new List<Variable>();
            status = Status.Stop;
            this.onPrint = onPrint;
            evaler = new Interpreter();
            addsubBM = addsub;
        }
        bool endCurrentCodeStack()
        {
            codestack.Pop();
            if (codestack.Count == 0)
            {
                return false;
            }
            return true;
        }
        void addNewCodeStack(List<XElement> code,bool isloop=false)
        {
            codestack.Push(new CodeStack() {codeList=code,currentCode=0,currentStatus=Status.Running,isloop=isloop});
        }
        
        XElement getCurrentBlock()
        {
            if (codestack.Count == 0)
                return null;
            var ccstck = codestack.Pop();
            if (ccstck.currentCode >= ccstck.codeList.Count)
                if (codestack.Count == 0)
                    return null;
                else if(ccstck.isloop)
                {
                    codestack.Push(ccstck);
                    return null;
                }
                else
                    ccstck = codestack.Pop();
            var ccblck = ccstck.codeList[ccstck.currentCode++];
            codestack.Push(ccstck);
            return ccblck;
        }
        /// <summary>
        /// 코드를 한 블럭 실행시킵니다.
        /// </summary>
        public bool Step()
        {
            XElement curline=getCurrentBlock();
            if (curline == null)
                return false;
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
                            addNewCodeStack(new List<XElement>(curline.Elements().First().Elements()));
                        }
                    } 
                    else
                    {
                        if (evaler.Eval<bool>(curline.Attribute("con").Value))
                        {
                            addNewCodeStack(new List<XElement>(curline.Elements().First().Elements()));
                        }
                        else
                        {
                            addNewCodeStack(new List<XElement>(curline.Elements().Skip(1).First().Elements()));
                        }
                    }
                    break;
                case "loop":
                    while (evaler.Eval<bool>(curline.Attribute("con").Value))
                    {
                        addNewCodeStack(new List<XElement>(curline.Elements().First().Elements()),true);
                        Runner();                        
                        if (codestack.Pop().currentStatus == Status.Break)
                            break;
                    }
                    break;
                case "ivk":
                    Ivk(curline);
                    break;
                case "break":
                    status = Status.Break;
                    break;
            }
            return true;
        }
        /*
        public bool Step()
        {
            XElement curline;
            try
            {
                curline=codeList[blockPoint++];
            }
            catch (Exception)
            {
                return false;
            }
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
            return true;
        }
        */
        /// <summary>
        /// 입력 대기중 실행되면 값을 입력합니다.
        /// </summary>
        /// <param name="a">입력될 값입니다</param>
        /// <returns>true면 입력이 처리된겁니다</returns>
        public bool Input(string a)
        {
            input = status == Status.WaitForInput ? a : input;
            return status == Status.WaitForInput;
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
            bool SuccessToExecute = true;
            while (SuccessToExecute)
            {
                if (status == Status.Break)
                    break;
                lock (lockObject)
                {
                    SuccessToExecute=Step();
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
