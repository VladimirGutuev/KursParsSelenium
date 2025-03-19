using System;


namespace KursParsSelenium
{
    public class UserInfo
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
    }
}