using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Remoting.Metadata.W3cXsd2001; // Открываем ссылки в браузере по умолчанию

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
            UserArrivalDate = Console.ReadLine();
            Console.Write("\nВведите дату выезда в формате день.месяц.год: ");
            UserDepartureDate = Console.ReadLine();
            Console.Write("\nВведите минимальную стоимость жилья за проживание: ");
            UserMinCost = Convert.ToInt32(Console.ReadLine());
            Console.Write("\nВведите максимальную стоимость жилья за проживание: ");
            UserMaxCost = Convert.ToInt32(Console.ReadLine());
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


    }
    class ListingInfo
    {
        public string Title { get; set; }
        public decimal Price {  get; set; }
        public string Rating {  get; set; }
        public int ReviewsCount {  get; set; }
        public string Link { get; set; }
    }
}
