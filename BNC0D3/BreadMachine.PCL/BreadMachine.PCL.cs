using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BreadMachine.PCL
{
    public enum Status
    {
        Running, Stop, Pause, WaitForInput
    }
    public class BMachine : IDisposable
    {
        int blockPoint;
        XDocument currentCode;
        List<XElement> codeList;
        Action<string> onPrint;
        Status status;
        Interpreter evaler;
        public BMachine(string codeBlock, Action<string> onPrint = null)
        {
            blockPoint = 0;
            currentCode = XDocument.Parse(codeBlock);
            codeList = currentCode.Root.Descendants() as List<XElement>;
            //varList = new List<Variable>();
            status = Status.Stop;
            this.onPrint = onPrint;
            evaler = new Interpreter();
        }
        public BMachine(XDocument codeBlock, Action<string> onPrint = null)
        {
            blockPoint = 0;
            currentCode = codeBlock;
            codeList = codeBlock.Root.Descendants() as List<XElement>;
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
                case "ivk":
                    Ivk(curline);
                    break;
            }
        }

        private void Ivk(XElement ivkCmd)
        {
            switch (ivkCmd.Attribute("type").Value)
            {
                case "0":
                    onPrint?.Invoke(evaler.Eval<string>(ivkCmd.Value));
                    break;
                case "1":

                    break;
            }
        }

        public void Run()
        {

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
