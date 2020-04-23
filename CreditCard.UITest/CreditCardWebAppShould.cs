
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace CreditCard.UITest
{

    [Trait("Category", "Smoke")]
    public class CreditCardWebAppShould
    {
        private const string HomeTitle = "Home Page - Credit Cards";
        private const string HomeUrl = "http://localhost:5258/";
        private const string AboutUrl = "http://localhost:5258/Home/About";

        [Fact(DisplayName = "LoadAplicationPage")]
        public void LoadHomePage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                driver.Manage().Window.Maximize();
                driver.Manage().Window.Minimize();
                driver.Manage().Window.Size = new System.Drawing.Size(300, 200);
                DemoHelper.Pause();
                driver.Manage().Window.Position = new System.Drawing.Point(1, 1);
                DemoHelper.Pause();
                driver.Manage().Window.Position = new System.Drawing.Point(50, 50);
                DemoHelper.Pause();
                driver.Manage().Window.Position = new System.Drawing.Point(100, 100);
                DemoHelper.Pause();
                driver.Manage().Window.FullScreen();
                DemoHelper.Pause();
                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);
            }
        }

        [Fact(DisplayName = "Refresh")]
        public void Refresh()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                driver.Navigate().Refresh();

                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);
            }
        }

        [Fact(DisplayName = "ReloadHomePage")]
        public void ReloadHomePage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                IWebElement generationTokenElement = driver.FindElement(By.Id("GenerationToken"));
                string initialToken = generationTokenElement.Text;
                DemoHelper.Pause();
                driver.Navigate().GoToUrl(AboutUrl);
                driver.Navigate().Back();


                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);

                string reloadedToken = driver.FindElement(By.Id("GenerationToken")).Text;
                Assert.NotEqual(initialToken, reloadedToken);
            }
        }

        [Fact(DisplayName = "ReloadHomePageOnForward")]
        public void ReloadHomePageOnForward()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(AboutUrl);
                DemoHelper.Pause();
                driver.Navigate().GoToUrl(HomeUrl);
                IWebElement element = driver.FindElement(By.Id("GenerationToken"));
                string initialtoke = element.Text;
                driver.Navigate().Back();
                driver.Navigate().Forward();


                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);

                string reload = driver.FindElement(By.Id("GenerationToken")).Text;
                Assert.NotEqual(initialtoke, reload);

            }
        }


        [Fact(DisplayName = "DisplayProductAndRates")]
        public void DisplayProductAndRates()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                ReadOnlyCollection<IWebElement> tableCells = driver.FindElements(By.TagName("td"));

                Assert.Equal("Easy Credit Card", tableCells[0].Text);
                Assert.Equal("20% APR", tableCells[1].Text);

                Assert.Equal("Silver Credit Card", tableCells[2].Text);
                Assert.Equal("18% APR", tableCells[3].Text);

                Assert.Equal("Gold Credit Card", tableCells[4].Text);
                Assert.Equal("17% APR", tableCells[5].Text);

            }
        }
    }
}
