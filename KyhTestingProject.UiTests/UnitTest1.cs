using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace KyhTestingProject.UiTests
{
    [TestClass]
    public class UnitTest1
    {
        private static IWebDriver _driver;
        public UnitTest1()
        {
            //Varje g�ng en Constructor skapas 
            //Kan k�ras flera g�nger 
        }

        [ClassInitialize]
        public void Initialize()
        {
            //Medans denna kan bara k�ra en g�ng
            _driver = new ChromeDriver();
        }

        [ClassCleanup]
        public void CleanUp()
        {
            _driver.Close();
            _driver.Dispose();
        }

        [TestMethod]
        public void Can_Edit_Employee()
        {
            _driver.Navigate().GoToUrl("https://localhost:7122/");
            
        }
    }
}