using System;

namespace KursParsSelenium
{
    internal class Program
    {

        static void Main(string[] args)
        {
            while (true)
            {
                try
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
                    break;
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Произошла ошибка {ex.Message}");
                    Console.Write("Перезапустить программу? (Y/N):");
                    string input = Console.ReadLine();
                    if (input.Trim().ToUpper() == "N") { break; }
                }
            }
        }
    }
}