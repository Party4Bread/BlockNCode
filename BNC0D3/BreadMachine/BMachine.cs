using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BreadMachine.Android
{
    public enum Status
    {
        Running, Stop, Pause, WaitForInput, Break
    }
    /*
    struct Variable
    {
        string name;
        string value;
        bool isString;
        public Variable(string name, string value, bool isString)
        {
            this.name = name;
            this.value = value;
            this.isString = isString;
        }
    }*/
    public class BMachine : IDisposable
    {
        int blockPoint;
        XDocument currentCode;
        List<XElement> codeList;
        Action<string> onPrint;
        Status status;
        //List<Variable> varList;
        Interpreter evaler;
        public BMachine(string codeBlock, Action<string> onPrint = null)
        {
            blockPoint = 0;
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
            currentCode = codeBlock;
            codeList = new List<XElement>(codeBlock.Root.Elements());
            //varList = new List<Variable>();
            status = Status.Stop;
            this.onPrint = onPrint;
            evaler = new Interpreter();
        }

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
                            BMachine ifBm = new BMachine(new XDocument(curline.Elements().First()), onPrint);
                            ifBm.evaler = this.evaler;
                            ifBm.Run();
                            evaler = ifBm.evaler;
                        }
                    }
                    else
                    {
                        if (evaler.Eval<bool>(curline.Attribute("con").Value))
                        {
                            BMachine ifBm = new BMachine(new XDocument(curline.Elements().First()), onPrint);
                            ifBm.evaler = evaler;
                            ifBm.Run();
                            evaler = ifBm.evaler;
                        }
                        else
                        {
                            BMachine ifBm = new BMachine(new XDocument(curline.Elements().Skip(1).First()), onPrint);
                            ifBm.evaler = evaler;
                            ifBm.Run();
                            evaler = ifBm.evaler;
                        }
                    }
                    break;
                case "loop":
                    while(evaler.Eval<bool>(curline.Attribute("con").Value))
                    {
                        BMachine loopBm = new BMachine(new XDocument(curline.Elements().First()), onPrint);
                        loopBm.evaler = this.evaler;
                        loopBm.Run();
                        evaler = loopBm.evaler;
                        if (loopBm.status == Status.Break)
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
        }

        private void Ivk(XElement ivkCmd)
        {
            switch (ivkCmd.Attribute("type").Value)
            {
                case "0":
                    onPrint?.Invoke(evaler.Eval<object>(ivkCmd.Value).ToString());
                    break;
                case "1":

                    break;
            }
        }

        public void Run()
        {
            foreach (var meaningless_val in codeList)
            {
                if (status == Status.Break)
                    break;
                    Step();
            }
        }

        #region IDisposable implementation with finalizer
        private bool isDisposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    //if (_wplayer != null) _wplayer.Dispose();
                }
            }
            isDisposed = true;
        }
        #endregion
    }
}
