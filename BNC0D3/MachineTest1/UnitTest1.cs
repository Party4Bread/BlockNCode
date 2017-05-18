using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicExpresso;
using BreadMachine.Android;

namespace MachineTest1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DYPNT1()
        {
            var interpreter = new Interpreter();

            var parameters = new[] {
                new Parameter("x", 23),
                new Parameter("y", 7)
                };

            Assert.AreEqual(30, interpreter.Eval("x + y", parameters));
        }
        [TestMethod]
        public void DYPNT2()
        {
            var interpreter = new Interpreter();

            var parameters = new[] {
                new Parameter("y", 7) 
                };
            interpreter.SetVariable("x", 12);
            Assert.AreEqual(19, interpreter.Eval("x + y", parameters));
        }
        [TestMethod]
        public void DYPNT3()
        {
            var interpreter = new Interpreter();
            interpreter.SetVariable("x", 122);
            interpreter.SetVariable("y", 7);
            interpreter.SetVariable("y", interpreter.Eval("x + y"));
            Assert.AreEqual(129, interpreter.Eval("y"));
        }
        [TestMethod]
        public void splittest1()
        {
            string exp = "a=213+51";
           
            Assert.AreEqual(2,exp.Split('=').Length);
        }


        [TestMethod]
        public void Machine1()
        {

            string ans = ";";
            BMachine bm = new BMachine(@"<code>
                <def type='0' value='23'>a</def>
                <calc>a=a+12</calc>
                <ivk type='0'>a</ivk>
                </code>", (string sd) => { ans = sd; });
            bm.Step();
            bm.Step();
            bm.Step();
            Assert.AreEqual("35", ans);
        }
    }
}
