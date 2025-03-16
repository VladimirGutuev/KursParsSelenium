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
using System.Security.Policy; // Открываем ссылки в браузере по умолчанию





namespace KursParsSelenium
{
    internal class Program
    {

        static void Main(string[] args)
        {
            UserInfo user = new UserInfo();
            user.GetInfo();


            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://sutochno.ru/");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement CityInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("suggest")));
            CityInput.Clear();
            CityInput.SendKeys(user.UserCity);
            IWebElement CityInputChoice = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[normalize-space(text())='Калининград,']")));
            System.Threading.Thread.Sleep(5000);

            IWebElement ArrivalDate = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("search-widget-field--occupied__in")));
            ArrivalDate.Click();
            System.Threading.Thread.Sleep(5000);
            IWebElement ArrivalDateChoice = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector($"td[data-cy-date='{user.UserArrivalDate}']")));
            ArrivalDateChoice.Click();

            System.Threading.Thread.Sleep(5000);

            IWebElement DepartureDateChoice = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector($"td[data-cy-date='{user.UserDepartureDate}']")));
            DepartureDateChoice.Click();

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

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            long lastOffset = 0;

            while (true)
            {
                // Прокручиваем вниз на 500 пикселей
                js.ExecuteScript("window.scrollBy(0, 500);");
                Thread.Sleep(1000); // небольшая пауза, чтобы страница успела отрисоваться

                // Получаем текущее положение прокрутки (сколько пикселей от верха)
                long newOffset = (long)js.ExecuteScript("return window.pageYOffset;");
                // Также узнаём полную высоту документа
                long docHeight = (long)js.ExecuteScript("return document.body.scrollHeight;");

                // Если новое положение не изменилось или мы близки к нижней границе страницы — выходим
                if (newOffset == lastOffset || newOffset + 500 >= docHeight)
                {
                    break;
                }

                // Запоминаем новое положение, чтобы сравнить на следующем шаге
                lastOffset = newOffset;
            }


            System.Threading.Thread.Sleep(5000);




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
                    ratingText = "Рейтинг отсутствует";
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

                //CountOfReviews = user.GetCountOfReviews(ReviewElements[0].Text);

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
            System.Threading.Thread.Sleep(5000);
            var nextbuttons = driver.FindElements(By.CssSelector(".navigation"));

            if (nextbuttons.Count() > 0)
            {
                nextbuttons[0].Click();
                System.Threading.Thread.Sleep(5000);

                lastOffset = 0;
                while (true)
                {
                    // Прокручиваем вниз на 500 пикселей
                    js.ExecuteScript("window.scrollBy(0, 500);");
                    Thread.Sleep(1000); // небольшая пауза, чтобы страница успела отрисоваться

                    // Получаем текущее положение прокрутки (сколько пикселей от верха)
                    long newOffset = (long)js.ExecuteScript("return window.pageYOffset;");
                    // Также узнаём полную высоту документа
                    long docHeight = (long)js.ExecuteScript("return document.body.scrollHeight;");

                    // Если новое положение не изменилось или мы близки к нижней границе страницы — выходим
                    if (newOffset == lastOffset || newOffset + 500 >= docHeight)
                    {
                        break;
                    }

                    // Запоминаем новое положение, чтобы сравнить на следующем шаге
                    lastOffset = newOffset;
                }
            }

            System.Threading.Thread.Sleep(5000);

            





            while (true)
            {
                //int CountOfNextButtonsEqual1 = 0;
                if (nextbuttons.Count() == 0)
                {
                    break;
                }

                System.Threading.Thread.Sleep(5000);
                cards = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(".card-list__item")));


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
                        ratingText = "Рейтинг отсутствует";
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
                System.Threading.Thread.Sleep(5000);
                nextbuttons = driver.FindElements(By.CssSelector(".navigation"));
                if (nextbuttons.Count() == 1 || nextbuttons.Count() == 0)
                {
                    break;
                }
                nextbuttons[1].Click();

                System.Threading.Thread.Sleep(5000);

                lastOffset = 0;
                while (true)
                {
                    // Прокручиваем вниз на 500 пикселей
                    js.ExecuteScript("window.scrollBy(0, 500);");
                    Thread.Sleep(1000); // небольшая пауза, чтобы страница успела отрисоваться

                    // Получаем текущее положение прокрутки (сколько пикселей от верха)
                    long newOffset = (long)js.ExecuteScript("return window.pageYOffset;");
                    // Также узнаём полную высоту документа
                    long docHeight = (long)js.ExecuteScript("return document.body.scrollHeight;");

                    // Если новое положение не изменилось или мы близки к нижней границе страницы — выходим
                    if (newOffset == lastOffset || newOffset + 500 >= docHeight)
                    {
                        break;
                    }

                    // Запоминаем новое положение, чтобы сравнить на следующем шаге
                    lastOffset = newOffset;
                }

            }
            //foreach (var item in listings)
            //{
            //    Console.WriteLine($"Название отеля: {item.Title} Оценка отеля: {item.Rating}, Количество оценок: {item.ReviewsCount} Цена за всё время проживания: {item.Price}");
            //}
            Console.WriteLine($"Недвижимость была найдена! Всего {listings.Count()} объектов");
            Console.WriteLine($"Выберите метод сортировки(цифра) на выбор:\n1.По цене\n2.По рейтингу\n3.По кол-ву отзывов\n4.Цена/качество");
            int SortChoice = Convert.ToInt32(Console.ReadLine());

            listings.RemoveAll(item => item.Rating == "Рейтинг отсутствует" || item.ReviewsCount < 15);

            switch (SortChoice)
            {
                case 1:
                    listings.Sort((a, b) => a.Price.CompareTo(b.Price));
                    break;
                case 2:
                    listings.Sort((a, b) => a.Rating.CompareTo(b.Rating));
                    break;
                case 3:
                    listings.Sort((a, b) => a.ReviewsCount.CompareTo(b.ReviewsCount));
                    break;
                //case 4:
                    // listings.Sort((a, b) => a.Price.CompareTo(b.Price)); ДОДЕЛАТЬ !!!!
            }


            int num = 0;
            foreach (var item in listings)
            {
                num += 1;
                Console.WriteLine($"{num}.Название отеля: {item.Title} Оценка отеля: {item.Rating}, Количество оценок: {item.ReviewsCount} Цена за всё время проживания: {item.Price}, Ссылка: {item.Link}");
            }
            Console.WriteLine("Выбрав номер объекта, вы можете перейти на официальную страницу для бронирования");
            while (true)
            {
                int NumOfObject = Convert.ToInt32(Console.ReadLine());
                if (num >= NumOfObject && NumOfObject > 0)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = listings[num - 1].Link,
                        UseShellExecute = true
                    });
                }
                else { Console.WriteLine("Неверный ввод"); }
            }
        }
    }
}
