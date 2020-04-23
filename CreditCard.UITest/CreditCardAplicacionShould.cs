using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace CreditCard.UITest
{

    [Trait("Category", "Aplication")]
    public class CreditCardAplicacionShould
    {
        private const string HomeUrl = "http://localhost:5258";
        private const string ApplyUrl = "http://localhost:5258/Apply";
        private readonly ITestOutputHelper _output;

        public CreditCardAplicacionShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "BeInitiatedFromHomePage_NewLowRate")]
        public void BeInitiatedFromHomePage_NewLowRate()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                IWebElement appliLink = driver.FindElement(By.Name("ApplyLowRate"));
                appliLink.Click();
                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);

            }
        }

        [Fact(DisplayName = "BeInitiatedFromHomePage_EasyApplication")]
        public void BeInitiatedFromHomePage_EasyApplication()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                IWebElement carouseNext = driver.FindElement(By.CssSelector("#myCarousel > a.right.carousel-control > span.glyphicon.glyphicon-chevron-right"));
                carouseNext.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
                IWebElement applilink = wait.Until(d => d.FindElement(By.LinkText("Easy: Apply Now!")));
                applilink.Click();
                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }

        [Fact(DisplayName = "BeInitiatedFromHomePage_EasyApplication_Prebuilt_Conditions")]
        public void BeInitiatedFromHomePage_EasyApplication_Prebuilt_Conditions()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(11));
                IWebElement applyLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Easy: Apply Now!")));
                applyLink.Click();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }


        [Fact(DisplayName = "BeInitiatedFromHomePage_CustomerService")]
        public void BeInitiatedFromHomePage_CustomerService()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                _output.WriteLine($"{DateTime.Now.ToLongTimeString()} Navigating to '{HomeUrl}'");
                driver.Navigate().GoToUrl(HomeUrl);

                _output.WriteLine($"{DateTime.Now.ToLongTimeString()} Finding element using explicit wait");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(35));

                IWebElement applyLink =
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("customer-service-apply-now")));

                _output.WriteLine($"{DateTime.Now.ToLongTimeString()} Found element Displayed={applyLink.Displayed} Enabled={applyLink.Enabled}");
                _output.WriteLine($"{DateTime.Now.ToLongTimeString()} Clicking element");
                applyLink.Click();

                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }

        [Fact(DisplayName = "BeInitiatedFromHomePage_RandomGreeting")]
        public void BeInitiatedFromHomePage_RandomGreeting()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                IWebElement randomGreetingApplyLink = driver.FindElement(By.PartialLinkText("- Apply Now!"));
                randomGreetingApplyLink.Click();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);

            }
        }


        [Fact(DisplayName = "BeInitiatedFromHomePage_RandomGreeting_Using_XPATH")]
        public void BeInitiatedFromHomePage_RandomGreeting_Using_XPATH()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                IWebElement randomGreetingApplyLink =
                    driver.FindElement(By.XPath("/html/body/div/div[4]/div/p/a"));
                randomGreetingApplyLink.Click();

                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }

        [Fact(DisplayName = "BeSubmittedWhenValid")]
        public void BeSubmittedWhenValid()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(ApplyUrl);
                driver.FindElement(By.Id("FirstName")).SendKeys("Sarah");
                driver.FindElement(By.Id("LastName")).SendKeys("Smith");
                driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys("123456-A");
                driver.FindElement(By.Id("Age")).SendKeys("18");
                driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys("50000");
                driver.FindElement(By.Id("Single")).Click();

                IWebElement businessSourceSelectElement = driver.FindElement(By.Id("BusinessSource"));
                SelectElement bussinesSource = new SelectElement(businessSourceSelectElement);
                //Chechk default selected option is correct
                Assert.Equal("I'd Rather Not Say", bussinesSource.SelectedOption.Text);
                //Get all the available options
                foreach (IWebElement item in bussinesSource.Options)
                {
                    _output.WriteLine($"Value: {item.GetAttribute("value")} Text: {item.Text}");
                }
                Assert.Equal(5, bussinesSource.Options.Count);

                //Select an option
                bussinesSource.SelectByValue("Email");
                bussinesSource.SelectByText("Internet Search");
                bussinesSource.SelectByIndex(4);


                driver.FindElement(By.Id("TermsAccepted")).Click();
                driver.FindElement(By.Id("SubmitApplication")).Click();

                // Check form submitted
                Assert.StartsWith("Application Complete", driver.Title);
                Assert.Equal("ReferredToHuman", driver.FindElement(By.Id("Decision")).Text);
                Assert.NotEmpty(driver.FindElement(By.Id("ReferenceNumber")).Text);
                Assert.Equal("Sarah Smith", driver.FindElement(By.Id("FullName")).Text);
                Assert.Equal("18", driver.FindElement(By.Id("Age")).Text);
                Assert.Equal("50000", driver.FindElement(By.Id("Income")).Text);
                Assert.Equal("Single", driver.FindElement(By.Id("RelationshipStatus")).Text);
                Assert.Equal("TV", driver.FindElement(By.Id("BusinessSource")).Text);

                DemoHelper.Pause();
            }
        }

        [Fact(DisplayName = "")]
        public void BeSubmittedWhenValidationErrorsCorrected()
        {
            const string firstName = "Sarah";
            const string invalidAge = "17";
            const string validAge = "18";

            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(ApplyUrl);

                driver.FindElement(By.Id("FirstName")).SendKeys(firstName);
                // Don't enter lastname
                driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys("123456-A");
                driver.FindElement(By.Id("Age")).SendKeys(invalidAge);
                driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys("50000");
                driver.FindElement(By.Id("Single")).Click();
                IWebElement businessSourceSelectElement =
                        driver.FindElement(By.Id("BusinessSource"));
                SelectElement businessSource = new SelectElement(businessSourceSelectElement);
                businessSource.SelectByValue("Email");
                driver.FindElement(By.Id("TermsAccepted")).Click();
                driver.FindElement(By.Id("SubmitApplication")).Click();

                // Assert that validation failed                
                var validationErrors =
                    driver.FindElements(By.CssSelector(".validation-summary-errors > ul > li"));
                Assert.Equal(2, validationErrors.Count);
                Assert.Equal("Please provide a last name", validationErrors[0].Text);
                Assert.Equal("You must be at least 18 years old", validationErrors[1].Text);

                // Fix errors
                driver.FindElement(By.Id("LastName")).SendKeys("Smith");
                driver.FindElement(By.Id("Age")).Clear();
                driver.FindElement(By.Id("Age")).SendKeys(validAge);

                // Resubmit form
                driver.FindElement(By.Id("SubmitApplication")).Click();

                // Check form submitted
                Assert.StartsWith("Application Complete", driver.Title);
                Assert.Equal("ReferredToHuman", driver.FindElement(By.Id("Decision")).Text);
                Assert.NotEmpty(driver.FindElement(By.Id("ReferenceNumber")).Text);
                Assert.Equal("Sarah Smith", driver.FindElement(By.Id("FullName")).Text);
                Assert.Equal("18", driver.FindElement(By.Id("Age")).Text);
                Assert.Equal("50000", driver.FindElement(By.Id("Income")).Text);
                Assert.Equal("Single", driver.FindElement(By.Id("RelationshipStatus")).Text);
                Assert.Equal("Email", driver.FindElement(By.Id("BusinessSource")).Text);
            }
        }
    }
}
