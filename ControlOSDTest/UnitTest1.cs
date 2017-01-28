using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace OSDTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
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
          //  OSD.OSD.ShowMessage("hello");
            List<string> strings1 = new List<string>();
            strings1.Add("ajedan");
            strings1.Add("adva2222");
            strings1.Add("atri");
            strings1.Add("acetiri");
            OSD.Menu.MenuItems menuItems1 = new OSD.Menu.MenuItems(strings1);
            OSD.Menu.Menu submenu1 = new OSD.Menu.Menu("Submenu1", menuItems1);


            List<string> strings = new List<string>();
            strings.Add("jedan");
            strings.Add("dva2222");
            strings.Add("tri");
            strings.Add("cetiri");
            strings.Add("5");
            strings.Add("6");
            strings.Add("7");
            strings.Add("8");
            strings.Add("9");
            strings.Add("10");
            strings.Add("11");

            OSD.Menu.MenuItems menuItems = new OSD.Menu.MenuItems(strings);
            menuItems.Add(new OSD.Menu.MenuItem("aaa",true, submenu1));
            OSD.Menu.Menu firstMenu = new OSD.Menu.Menu("First Menu", menuItems);

            submenu1.parentMenu = firstMenu;

            OSD.Menu.Menu.ShowMenu(firstMenu, new OSD.Menu.MenuItemChosenDelegate(MenuItemClicked));
        }

        public void MenuItemClicked(object o, OSD.Menu.MenuItemChosenEventArgs e)
        {
            if (e.selectedMenuItem.text == "jedan")
            {                
                OSD.Menu.Menu.Close();
            }
            else
            {

                //List<string> strings = new List<string>();
                //strings.Add("ajedan");
                //strings.Add("adva2222");
                //strings.Add("atri");
                //strings.Add("acetiri");


                //OSD.Menu.MenuItems menuItems = new OSD.Menu.MenuItems(strings);



                //OSD.Menu.Menu.ChangePage("tvw menu", menuItems);
            }
        }
    }
}
