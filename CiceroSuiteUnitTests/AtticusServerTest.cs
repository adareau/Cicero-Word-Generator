﻿using AtticusServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CiceroSuiteUnitTests
{
    
    
    /// <summary>
    ///This is a test class for AtticusServerTest and is intended
    ///to contain all AtticusServerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AtticusServerTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for loadServerSettings
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AtticusServer.exe")]
        public void loadServerSettingsTest()
        {
            string filename = "AtticusServerSettings-1.set";
            ServerSettings result = AtticusServer_Accessor.loadServerSettings(filename);
            Assert.AreEqual(result.ServerName, "FermiServer", "Deserialized server settings object had incorrect name.");
            Assert.IsTrue(result.myDevicesSettings.ContainsKey("VdqjfUGLEm"), "Deserialized server settings object was missing a device.");

            result = AtticusServer_Accessor.loadServerSettings("AtticusServerSettings-2.set");
            Assert.AreEqual(result.ServerName, "Samwise", "Deserialized server settings object had incorrect name.");
            Assert.IsTrue(result.myDevicesSettings.ContainsKey("GPIB0/19,0"), "Deserialized server was missing a GPIB device settings object.");
            Assert.AreEqual(result.myDevicesSettings["GPIB0/19,0"].DeviceDescription, "Agilent Technologies,33250A,0,1.05-1.01-1.00-03-1\n",
                "Deserialized server had wrong GPIB device description.");

        }
    }
}
