using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static OpenQA.Selenium.BiDi.Modules.Session.ProxyConfiguration;
using System.Diagnostics;
using System.Security.Policy; // ќткрываем ссылки в браузере по умолчанию
using System.Globalization;

namespace KursParsSelenium
{
    public List<ListingInfo> Run(UserInfo user)
    {
        IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl("https://sutochno.ru/");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        IWebElement CityInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("suggest")));
        CityInput.Clear();
        CityInput.SendKeys(user.UserCity);
        IWebElement CityInputChoice = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//span[normalize-space(text())='{user.UserCity}']")));
        System.Threading.Thread.Sleep(5000);



        IWebElement ArrivalDate = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("search-widget-field--occupied__in")));
        ArrivalDate.Click();
        System.Threading.Thread.Sleep(5000);


        if (user.FindDate(driver, user.UserArrivalDate))
        {
            Console.WriteLine("ƒата заезда была найдена!");
        }
        else { Console.WriteLine("ƒата заезда не найдена, введите другую дату"); }

        if (user.FindDate(driver, user.UserDepartureDate))
        {
            Console.WriteLine("ƒата заезда была найдена!");
        }
        else { Console.WriteLine("ƒата заезда не найдена, введите другую дату"); }



        System.Threading.Thread.Sleep(5000);

        IWebElement SearchButton = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"root\"]/div[1]/div[2]/div[1]/div[2]/div/div[3]/button")));
        SearchButton.Click();

        System.Threading.Thread.Sleep(5000);

        IWebElement Quality = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"searchParams\"]/div[1]/div[3]/button[1]/span")));
        Quality.Click();

        System.Threading.Thread.Sleep(5000);

        IWebElement PriceOfAllTime = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"searchParams\"]/div[2]/div/div/div[1]/div/div[1]/div/div/ul/li[2]")));
        PriceOfAllTime.Click();

        System.Threading.Thread.Sleep(5000);

        IWebElement PriceMinInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"searchParams\"]/div[2]/div/div/div[1]/div/div[2]/div/div[1]/div/span[1]/input")));
        PriceMinInput.Click();
        PriceMinInput.Clear();
        PriceMinInput.SendKeys(Convert.ToString(user.UserMinCost));

        System.Threading.Thread.Sleep(5000);

        IWebElement PriceMaxInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"searchParams\"]/div[2]/div/div/div[1]/div/div[2]/div/div[1]/div/span[3]/input")));
        PriceMaxInput.Clear();
        PriceMaxInput.SendKeys(Convert.ToString(user.UserMaxCost));
        System.Threading.Thread.Sleep(5000);
        IWebElement ApplyPriceInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"searchParams\"]/div[2]/div/div/div[2]/button[2]")));
        ApplyPriceInput.Click();

        System.Threading.Thread.Sleep(5000);
        IWebElement ChoosePriceQuality = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"root\"]/div[1]/div[2]/div[4]/div[1]/div[2]/div/div[2]/button[2]")));
        ChoosePriceQuality.Click();



        System.Threading.Thread.Sleep(5000);

        ScrollDown(driver);


        System.Threading.Thread.Sleep(5000);




        var listings = CollectListings(driver, wait, user)


        System.Threading.Thread.Sleep(5000);
        var cards = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(".card-list__item")));
        var nextbuttons = driver.FindElements(By.CssSelector(".navigation"));
        while (true)
        {
            //int CountOfNextButtonsEqual1 = 0;
            if (nextbuttons.Count() == 0)
            {
                break;
            }

            System.Threading.Thread.Sleep(5000);

            listings.AddRange(CollectListings(driver, wait, user));

            System.Threading.Thread.Sleep(5000);
            nextbuttons = driver.FindElements(By.CssSelector(".navigation"));
            if (nextbuttons.Count() == 1 || nextbuttons.Count() == 0)
            {
                break;
            }
            nextbuttons[1].Click();

            System.Threading.Thread.Sleep(5000);


            ScrollDown(driver); // двигаем вниз

        }

    }


    private void ScrollDown(IWebElement driver)
    {
        lastOffset = 0;
        while (true)
        {
            // ѕрокручиваем вниз на 500 пикселей
            js.ExecuteScript("window.scrollBy(0, 500);");
            Thread.Sleep(1000); // небольша€ пауза, чтобы страница успела отрисоватьс€

            // ѕолучаем текущее положение прокрутки (сколько пикселей от верха)
            long newOffset = (long)js.ExecuteScript("return window.pageYOffset;");
            // “акже узнаЄм полную высоту документа
            long docHeight = (long)js.ExecuteScript("return document.body.scrollHeight;");

            // ≈сли новое положение не изменилось или мы близки к нижней границе страницы Ч выходим
            if (newOffset == lastOffset || newOffset + 500 >= docHeight)
            {
                break;
            }

            // «апоминаем новое положение, чтобы сравнить на следующем шаге
            lastOffset = newOffset;
        }
    }


    private List<ListingInfo> CollectListings(IWebDriver driver, WebDriverWait wait, UserInfo user)
    {
        var listings = new List<ListingInfo>();

        System.Threading.Thread.Sleep(5000);
        var cards = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(".card-list__item")));


        foreach (var card in cards)
        {
            var title = card.FindElement(By.CssSelector(".card-content__object-type")).Text;
            var PriceElements = card.FindElements(By.CssSelector(".price-order__main-text"));
            int price;

            price = user.GetPrice(PriceElements[0].Text);


            var ratingElements = card.FindElements(By.CssSelector(".rating-list__rating"));
            string ratingText;
            if (ratingElements.Count > 0)
            {
                ratingText = ratingElements[0].Text;
            }
            else
            {
                ratingText = "–ейтинг отсутствует";
            }
            var ReviewElements = card.FindElements(By.CssSelector(".rating-list__count"));
            int CountOfReviews;
            if (ReviewElements.Count > 0)
            {
                CountOfReviews = user.GetCountOfReviews(ReviewElements[0].Text);
            }
            else
            {
                CountOfReviews = 0;
            }



            IWebElement linkElement = card.FindElement(By.CssSelector("a.card-content"));
            string partialHref = linkElement.GetAttribute("href");

            listings.Add(new ListingInfo
            {
                Title = title,
                Rating = ratingText,
                Price = price,
                ReviewsCount = CountOfReviews,
                Link = partialHref
            });


        }
        return listings;
    }

    public bool FindDate(IWebDriver driver, string date)
    {
        int MaxAttempts = 10;
        bool found = false;
        int attempt = 0;
        while (!found && attempt < MaxAttempts)
        {
            var ArrivalDateChoice = driver.FindElements(By.CssSelector($"td[data-cy-date='{date}']"));
            if (ArrivalDateChoice.Count() > 0)
            {
                ArrivalDateChoice[0].Click();
                found = true;
            }
            else
            {
                var NextButtonsCalendar = driver.FindElements(By.CssSelector(".sc-datepickerext-wrapper-next"));
                if (NextButtonsCalendar.Count() == 0)
                {
                    break;
                }
                NextButtonsCalendar[0].Click();
                Thread.Sleep(1000);
            }
            attempt++;
            //IWebElement ArrivalDateChoice = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector($"td[data-cy-date='{user.UserArrivalDate}']")));
            //ArrivalDateChoice.Click();
        }
        return found;
    }
    public int GetCountOfReviews(string ReviewsText)
    {
        int Reviews;

        var parts = ReviewsText.Split(' ');
        Reviews = int.Parse(parts[0]);
        return Reviews;
    }
    public int GetPrice(string PriceTextWithSpace)
    {
        int Price;
        string PriceText = "";
        var parts = PriceTextWithSpace.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Count(); i++)
        {
            if (parts[i] == "?") { break; }
            else
            {
                PriceText += (parts[i]);
            }
        }
        Price = int.Parse(PriceText);
        return Price;
    }

}