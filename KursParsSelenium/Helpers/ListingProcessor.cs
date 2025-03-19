using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KursParsSelenium
{
    public static class ListingProcessor
    {
        public static void ProcessListings(List<ListingInfo> listings)
        {
            Console.WriteLine("Выберите метод сортировки(цифра) на выбор:\n1.По цене\n2.По рейтингу\n3.По кол-ву отзывов\n4.Цена/качество(больше - лучше)");
            int sortChoice = Convert.ToInt32(Console.ReadLine());

            listings.RemoveAll(item => item.Rating == "Рейтинг отсутствует" || item.ReviewsCount < 15);

            switch (sortChoice)
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
                case 4:
                    listings.Sort((a, b) => b.PriceQualityRatio.CompareTo(a.PriceQualityRatio));
                    break;
            }
        }

        public static void ShowAndOpenListing(List<ListingInfo> listings)
        {
            int num = 0;
            foreach (var item in listings)
            {
                num++;
                Console.WriteLine($"{num}.Название отеля: {item.Title}, " +
                    $"Оценка: {item.Rating}, " +
                    $"Кол-во отзывов: {item.ReviewsCount}, " +
                    $"Цена: {item.Price}, " +
                    $"Коэф. цена/качества: {item.PriceQualityRatio}, " +
                    //$"Ссылка: {item.Link}");
            }

            Console.WriteLine("Выбрав номер объекта, вы можете перейти на официальную страницу для бронирования");
            while (true)
            {
                int NumOfObject = Convert.ToInt32(Console.ReadLine());
                if (NumOfObject >= 1 && NumOfObject <= listings.Count)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = listings[NumOfObject - 1].Link,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Неверный ввод");
                }
            }
        }
    }
}