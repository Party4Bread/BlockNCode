using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BreadMachine.Android
{
    public enum Status
    {
        Running, Stop, Pause, WaitForInput
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
        XmlDocument currentCode;
        XmlNodeList codeList;
        Action<string> onPrint;
        Status status;
        //List<Variable> varList;
        Interpreter evaler;
        public BMachine(XmlDocument codeBlock, Action<string> onPrint=null)
        {
            blockPoint = 0;
            currentCode = codeBlock;
            codeList = codeBlock.ChildNodes;
            //varList = new List<Variable>();
            status = Status.Stop;
            this.onPrint = onPrint;
            evaler = new Interpreter();
        }

        public void Step()
        {
            XmlNode curline = codeList[blockPoint++];
            switch(curline.Name)
            {
                case "def":
                    if(curline.Attributes["type"].Value == "0")
                    {
                        evaler.SetVariable(curline.InnerText, long.Parse(curline.Attributes["value"].Value));
                    }
                    else
                    {
                        evaler.SetVariable(curline.InnerText, curline.Attributes["value"].Value);
                    }
                    break;
                case "calc":
                    evaler.SetVariable(curline.InnerText.Split('=')[0],curline.InnerText.Split('=')[1]);
                    break;
                case "ivk":
                    Ivk(curline);
                    break;
            }
        }

        private void Ivk(XmlNode ivkCmd)
        {
            switch(ivkCmd.Attributes["type"].Value)
            {
                case "0":
                    onPrint?.Invoke(evaler.Eval<string>(ivkCmd.InnerText));
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
