using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Extension.ControlSimpleMenu;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;

namespace ControlMenuTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ControlMenuTest
    {
        public ControlMenuTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            
            List<string> items = new List<string>();
            items.Add("prvi");
            items.Add("drugi");
            items.Add("Treci");
            items.Add("4");
            items.Add("5");
            items.Add("6");
            items.Add("7");
            items.Add("8");
            items.Add("9");
            items.Add("10");
            items.Add("11");
            items.Add("12");
            items.Add("13");

            ControlMenuItems controlMenuItems = new ControlMenuItems(items);
            controlMenuItems[2].HaveChildren = true;

            
            ControlMenu cm = new ControlMenu();
            cm.Execute("Caption is here:", controlMenuItems, 1, 2, ControlMenu_OnExecute);
            
            //cm.Execute("Caption is here:", controlMenuItems, 5, 7);
        }

        private void ControlMenu_OnExecute(object sender, ControlMenuSelectedEventArgs e)
        {
            e.postAction = ControlMenuSelectedPostAction.None;
        }
    }
}
