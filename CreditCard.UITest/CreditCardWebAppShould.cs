using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;
using System.IO;
using ApprovalTests.Reporters;
using ApprovalTests;
using CreditCard.UITest.PageObjectModels;
//using ApprovalTests.Reporters.Windows;

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
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
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
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                string initialToken = homePage.GenerationToken;
                driver.Navigate().GoToUrl(AboutUrl);
                driver.Navigate().Back();

                homePage.EnsurePageLoaded();

                string reloadedToken = homePage.GenerationToken;
                Assert.NotEqual(initialToken, reloadedToken);
            }
        }


        [Fact(DisplayName = "DisplayProductAndRates")]
        public void DisplayProductAndRates()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
                DemoHelper.Pause();

                

                Assert.Equal("Easy Credit Card", homePage.Products[0].name);
                Assert.Equal("20% APR", homePage.Products[0].interestRate);

                Assert.Equal("Silver Credit Card", homePage.Products[1].name);
                Assert.Equal("18% APR", homePage.Products[1].interestRate);

                Assert.Equal("Gold Credit Card", homePage.Products[2].name);
                Assert.Equal("17% APR", homePage.Products[2].interestRate);

            }
        }

        [Fact(DisplayName = "OpenContactFotterLinkInNewTab")]
        public void OpenContactFotterLinkInNewTab()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
                homePage.ClickContactFooterLink();

                ReadOnlyCollection<string> allTabs = driver.WindowHandles;
                string homePageTab = allTabs[0];
                string contactTab = allTabs[1];
                driver.SwitchTo().Window(contactTab);

                Assert.EndsWith("/Home/Contact", driver.Url);
            }
        }

        [Fact(DisplayName = "AlertIfLiveChatClosed")]
        public void AlertIfLiveChatClosed()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
                homePage.ClickLiveChatFooterLink();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));                
                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
                Assert.Equal("Live chat is currently closed.", alert.Text);

                DemoHelper.Pause();
                alert.Accept();
                DemoHelper.Pause();
            }
        }

        [Fact(DisplayName = "NavigateToAboutUsWhenOkClicked")]
        public void NavigateToAboutUsWhenOkClicked()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
                homePage.ClickLearnAboutUsLink();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
                alert.Accept();

                Assert.EndsWith("/Home/About", driver.Url);
            }
        }

        [Fact(DisplayName = "NotNavigateToaboutWhenCancelClicked")]
        public void NotNavigateToaboutWhenCancelClicked()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLearnAboutUsLink();

                WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(5));
                IAlert alertBox = wait.Until(ExpectedConditions.AlertIsPresent());
                alertBox.Dismiss();

                homePage.EnsurePageLoaded();
            }
        }

        [Fact(DisplayName = "NotDisplayCookiesUseMessage")]
        public void NotDisplayCookiesUseMessage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
                driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
                driver.Navigate().Refresh();

                Assert.False(homePage.IsCookieMessagePresent);

                Cookie cookieValue = driver.Manage().Cookies.GetCookieNamed("acceptedCookies");
                Assert.Equal("true", cookieValue.Value);

                driver.Manage().Cookies.DeleteCookieNamed("acceptedCookies");
                driver.Navigate().Refresh();

                Assert.True(homePage.IsCookieMessagePresent);                
            }
        }

        [Fact(DisplayName = "RenderAboutPage")]
        //[UseReporter(typeof(BeyondCompare4Reporter))]
        public void RenderAboutPage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(AboutUrl);

                ITakesScreenshot screenShotDriver = (ITakesScreenshot)driver;

                Screenshot screenshot = screenShotDriver.GetScreenshot();

                screenshot.SaveAsFile("aboutpage.bmp", ScreenshotImageFormat.Bmp);

                FileInfo file = new FileInfo("aboutpage.bmp");

                //Approvals.Verify(file);
            }
        }
    }
}
