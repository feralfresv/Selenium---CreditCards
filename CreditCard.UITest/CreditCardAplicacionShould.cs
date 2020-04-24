using System;
using CreditCard.UITest.PageObjectModels;
using CreditCards.UITests;
using CreditCards.UITests.PageObjectModels;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace CreditCard.UITest
{

    [Trait("Category", "Aplication")]
    public class CreditCardAplicacionShould : IClassFixture<ChromeDriverFixture>
    {
        private const string HomeUrl = "http://localhost:5258";
        private const string ApplyUrl = "http://localhost:5258/Apply";
        private readonly ChromeDriverFixture _chromeDriverFixture;


        public CreditCardAplicacionShould(ChromeDriverFixture chromeDriverFixture)
        {
            _chromeDriverFixture = chromeDriverFixture;
            _chromeDriverFixture.Driver.Manage().Cookies.DeleteAllCookies();
            _chromeDriverFixture.Driver.Navigate().GoToUrl("about:blank");

        }

        [Fact(DisplayName = "BeInitiatedFromHomePage_NewLowRate")]
        public void BeInitiatedFromHomePage_NewLowRate()
        {
                var homePage = new HomePage(_chromeDriverFixture.Driver);
                homePage.NavigateTo();

                ApplicationPage applicationPage = homePage.ClickApplyLowRateLink();

                applicationPage.EnsurePageLoaded();
        }

        [Fact(DisplayName = "BeInitiatedFromHomePage_EasyApplication")]
        public void BeInitiatedFromHomePage_EasyApplication()
        {
            var homePage = new HomePage(_chromeDriverFixture.Driver);
            homePage.NavigateTo();

            homePage.WaitForEasyApplicationCarouselPage();

            ApplicationPage applicationPage = homePage.ClickApplyEasyApplicationLink();

            applicationPage.EnsurePageLoaded();

        }

        [Fact(DisplayName = "BeInitiatedFromHomePage_EasyApplication_Prebuilt_Conditions")]
        public void BeInitiatedFromHomePage_EasyApplication_Prebuilt_Conditions()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.WaitForEasyApplicationCarouselPage();

                ApplicationPage applicationPage = homePage.ClickApplyEasyApplicationLink();

                applicationPage.EnsurePageLoaded();
            }
        }        

        [Fact(DisplayName = "BeInitiatedFromHomePage_CustomerService")]
        public void BeInitiatedFromHomePage_CustomerService()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(_chromeDriverFixture.Driver);
                homePage.NavigateTo();

                WebDriverWait wait = new WebDriverWait(_chromeDriverFixture.Driver, TimeSpan.FromSeconds(35));

                IWebElement applyLink =
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("customer-service-apply-now")));

                applyLink.Click();

                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", _chromeDriverFixture.Driver.Title);
                Assert.Equal(ApplyUrl, _chromeDriverFixture.Driver.Url);
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
            const string FirstName = "Sarah";
            const string LastName = "Smith";
            const string Number = "123456-A";
            const string Age = "18";
            const string Income = "50000";

            using (IWebDriver driver = new ChromeDriver())
            {
                var applicationPage = new ApplicationPage(driver);
                applicationPage.NavigateTo();

                applicationPage.EnterFirstName(FirstName);
                applicationPage.EnterLastName(LastName);
                applicationPage.EnterFrequentFlyerNumber(Number);
                applicationPage.EnterAge(Age);
                applicationPage.EnterGrossAnnualIncome(Income);
                applicationPage.ChooseMaritalStatusSingle();
                applicationPage.ChooseBusinessSourceTV();
                applicationPage.AcceptTerms();
                ApplicationCompletePage applicationCompletePage =
                    applicationPage.SubmitApplication();

                applicationCompletePage.EnsurePageLoaded();

                Assert.Equal("ReferredToHuman", applicationCompletePage.Decision);
                Assert.NotEmpty(applicationCompletePage.ReferenceNumber);
                Assert.Equal($"{FirstName} {LastName}", applicationCompletePage.FullName);
                Assert.Equal(Age, applicationCompletePage.Age);
                Assert.Equal(Income, applicationCompletePage.Income);
                Assert.Equal("Single", applicationCompletePage.RelationshipStatus);
                Assert.Equal("TV", applicationCompletePage.BusinessSource);
            }
        }

        [Fact(DisplayName = "BeSubmittedWhenValidationErrorsCorrected")]
        public void BeSubmittedWhenValidationErrorsCorrected()
        {
            const string firstName = "Sarah";
            const string invalidAge = "17";
            const string validAge = "18";

            using (IWebDriver driver = new ChromeDriver())
            {
                var applicationPage = new ApplicationPage(driver);
                applicationPage.NavigateTo();

                applicationPage.EnterFirstName(firstName);
                // Don't enter lastname
                applicationPage.EnterFrequentFlyerNumber("123456-A");
                applicationPage.EnterAge(invalidAge);
                applicationPage.EnterGrossAnnualIncome("50000");
                applicationPage.ChooseMaritalStatusSingle();
                applicationPage.ChooseBusinessSourceTV();
                applicationPage.AcceptTerms();
                applicationPage.SubmitApplication();

                // Assert that validation failed                                
                Assert.Equal(2, applicationPage.ValidationErrorMessages.Count);
                Assert.Contains("Please provide a last name", applicationPage.ValidationErrorMessages);
                Assert.Contains("You must be at least 18 years old", applicationPage.ValidationErrorMessages);

                // Fix errors
                applicationPage.EnterLastName("Smith");
                applicationPage.ClearAge();
                applicationPage.EnterAge(validAge);

                // Resubmit form
                ApplicationCompletePage applicationCompletePage = applicationPage.SubmitApplication();

                // Check form submitted
                applicationCompletePage.EnsurePageLoaded();
            }
        }
    }
}
