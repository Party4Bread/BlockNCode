using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BreadMachine
{
    public class BreadMachine : IDisposable
    {
        int blockPoint;
        XmlDocument currentCode;
        XmlNodeList codeList;
        public BreadMachine(XmlDocument codeBlock)
        {
            blockPoint = 0;
            currentCode = codeBlock;
            codeList = codeBlock.ChildNodes;
        }

        public void Step()
        {
            XmlNode curline = codeList[blockPoint++];
            switch(curline)
            {
                
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
