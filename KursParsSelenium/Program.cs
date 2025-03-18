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
using System.Globalization;





namespace KursParsSelenium
{
    internal class Program
    {

        static void Main(string[] args)
        {
            // 1. Ввод данных пользователя
            UserInfo user = new UserInfo();
            user.GetInfo();  // Запрос города, дат, цены

            // 2. Создаём сервис Selenium
            var seleniumService = new SeleniumService();

            // 3. Запускаем парсинг
            var listings = seleniumService.Run(user);

            // 4. Сортируем и фильтруем
            ListingProcessor.ProcessListings(listings);

            // 5. Выводим и даём возможность открыть ссылку
            ListingProcessor.ShowAndOpenListing(listings);
        }
    }
}