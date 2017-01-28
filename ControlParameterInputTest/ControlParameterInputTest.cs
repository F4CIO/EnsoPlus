using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Extension;

namespace ControlParameterInputTest
{
    /// <summary>
    /// Summary description for ControlParameterInput
    /// </summary>
    [TestClass]
    public class ControlParameterInputTest
    {
        public ControlParameterInputTest()
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
        public void TestDisplay()
        {
            List<string> suggestions = new List<string>();
            suggestions.Add("home second and third");
            suggestions.Add("home second");
            suggestions.Add("house");
            suggestions.Add("home");
            suggestions.Add("dido diiwer asdf asdff");
            suggestions.Add("dido diiwer asdf asdff dido diiwer asdf asdff dido diiwer asdf asdff");
            suggestions.Add("figaro figud frojds");
            suggestions.Add("demn");
            suggestions.Add("its ok");
            suggestions.Add("get me out of here");
            suggestions.Add("give me a break");

            string r = ParameterInput.Display("open",suggestions, true, false, "its ok", false, false,null, null);
            r = r;
        }
    }
}
