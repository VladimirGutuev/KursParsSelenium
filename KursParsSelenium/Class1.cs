using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using OpenQA.Selenium; // Открываем ссылки в браузере по умолчанию
using System.Threading;
using System.Globalization; // Правильно ли введена дата пользователем


namespace KursParsSelenium
{
    internal class Class1
    { }
    class UserInfo
    {

        public string UserCity;
        public string UserArrivalDate;
        public string UserDepartureDate;
        public int UserMinCost;
        public int UserMaxCost;

        public void GetInfo()
        {
            Console.WriteLine("Здравствуйте! Для наилучшего выбора стоит задать вам несколько вопросов: ");
            Console.Write("Введите ваш город: ");
            UserCity = Console.ReadLine();
            Console.Write("\nВведите дату заезда в формате день.месяц.год: ");
            while (true)
            {
                UserArrivalDate = Console.ReadLine();
                if (TryGetValidDate(UserArrivalDate, out string arrivalDate) == true)
                {
                    UserArrivalDate = arrivalDate;
                    break;
                }
                else
                {
                    Console.WriteLine("Неверная дата заезда. Попробуйте снова: ");
                }
            }
            Console.Write("\nВведите дату выезда в формате день.месяц.год: ");
            while (true)
            {
                UserDepartureDate = Console.ReadLine();
                if (TryGetValidDate(UserDepartureDate, out string departureDate) == true)
                {
                    UserDepartureDate = departureDate;
                    break;
                }
                else
                {
                    Console.WriteLine("Неверная дата выезда. Попробуйте снова: ");
                }
            }
            Console.Write("\nВведите минимальную стоимость жилья за проживание: ");
            UserMinCost = Convert.ToInt32(Console.ReadLine());
            Console.Write("\nВведите максимальную стоимость жилья за проживание: ");
            UserMaxCost = Convert.ToInt32(Console.ReadLine());
        }

        public bool TryGetValidDate(string input, out string validDate)
        {
            string[] formats = { "dd.MM.yyyy", "dd/MM/yyyy", "dd,MM,yyyy", "dd-MM-yyyy" };
            if (DateTime.TryParseExact(input, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
            {
                validDate = dt.ToString("dd.MM.yyyy");
                return true;
            }
            else
            {
                validDate = null;
                return false;
            }
        }

        public void SortListings()
        {
            
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
            for(int i = 0; i < parts.Count(); i++)
            {
                if (parts[i] == "₽") { break; }
                else
                {
                    PriceText += (parts[i]);
                }
            }
            Price = int.Parse(PriceText);
            return Price;
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
            if (found == true)
            {
                return true;
            }
            else { return false; }
        }
    }
    class ListingInfo
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Rating { get; set; }
        public int ReviewsCount { get; set; }
        public string Link { get; set; }
        public decimal PriceQualityRatio
        => Convert.ToDecimal(Rating) / Price * 1000;
    }
}
