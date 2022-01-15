/*
Используя REST Api НБУ (https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json)
написать приложение, которое выводит курсы валют по отношению к гривне на текущий день.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace CurrencyRates_Regex
{
    class Currency : IComparable<Currency>
    {
        public string CurrencyName { get; set; }
        public decimal Rate { get; set; }
        public Currency(string currencyName, decimal rate)
        {
            CurrencyName = currencyName;
            Rate = rate;
        }
        public override string ToString()
        {
            return $"Currency: {CurrencyName} rate: {Rate}";
        }

        int IComparable<Currency>.CompareTo(Currency other)
        {
            return this.CurrencyName.CompareTo(other.CurrencyName);
        }
        public bool IsCurrencyNameFragmentStart(string fragment)
        {
            return CurrencyName.StartsWith(fragment, StringComparison.CurrentCultureIgnoreCase);
        }
        //public bool IsCurrencyNameFragment(string fragment)
        //{
        //    if (CurrencyName.IndexOf(fragment)<0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
    }
    class Rates : IEnumerable<Currency> 
    {
        public List<Currency> Currencylist { get; set; } = new List<Currency>();
        public void Add(Currency currency)
        {
            Currencylist.Add(currency);
        }

        public IEnumerator<Currency> GetEnumerator()
        {
            return Currencylist.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Currencylist).GetEnumerator();
        }
        public bool isCurrencyInCurrencyList(string fragment)
        {
            foreach (var item in Currencylist)
            {
                if (item.IsCurrencyNameFragmentStart(fragment))
                {
                    return true;
                }
            }
            return false;
        }
        public void Sort()
        {
            Currencylist.Sort();
        }
    }
    class Program
    {
        public static DateTime EnterDate()
        {
            Console.WriteLine("день:");
            int day = int.Parse(Console.ReadLine());
            Console.WriteLine("месяц:");
            int manth = int.Parse(Console.ReadLine());
            Console.WriteLine("год:");
            int year = int.Parse(Console.ReadLine());
            
            return new DateTime(year, manth, day);
        }
        public static Rates GetRates()
        {
            return GetRates(DateTime.Now);
        }
        public static Rates GetRates(DateTime date)
        {
            string dateRate = date.ToString("yyyyMMdd");
            string test = "123654";
            test.Split("23");
            var client = new WebClient();
            var html = client.DownloadString($"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={dateRate}&json");

            MatchCollection collection = Regex.Matches(html, "txt\":\"(.*?)\".*?\":(\\d*.?\\d*)");
            Rates currencies = new Rates();
            foreach (Match item in collection)
            {
                try
                {
                    string txt = (string)item.Groups[1].Value;
                    string rate = item.Groups[2].Value;
                    decimal dec = Convert.ToDecimal(rate, CultureInfo.InvariantCulture);
                    Currency currency = new Currency(txt, dec);
                    currencies.Add(currency);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
            return currencies;

        }
        static void Main(string[] args)
        {           
            char ch='0';
            do
            {
                try
                {
                    Console.WriteLine("\tМЕНЮ\n\n1-Показать все курсы валют на текущую дату\n2-Показать все курсы валют на указанную дату" +
                    "\n3-Показать конкретный курс на указанную дату\n" +
                    "4-Отсортировать список валют\nESC-Выход");


                    ch = Console.ReadKey().KeyChar;
                    Console.WriteLine();

                    switch (ch)
                    {
                        case '1':
                            {
                                Console.Clear();
                                Console.WriteLine("\tКурсы валют\n\n");

                                foreach (var item in GetRates())
                                {
                                    Console.WriteLine(item);
                                }
                                Console.WriteLine("Нажмите любую клавишу...");

                                Console.ReadKey();
                                break;
                            }

                        case '2':
                            {
                                Console.Clear();
                                Console.WriteLine("\tКурсы валют на указанную дату\n\n");
                                Console.WriteLine("Введите дату");
                                foreach (var item in GetRates(EnterDate()))
                                {
                                    Console.WriteLine(item);
                                }
                                Console.WriteLine("Нажмите любую клавишу...");

                                Console.ReadKey();
                                break;
                            }
                        case '3':
                            {
                                Console.Clear();
                                Console.WriteLine("\tКурс конкретной валюты на указанную дату\n\n");
                                Console.WriteLine("Введите валюту");
                                string fragmentCurrency = Console.ReadLine();
                                Rates currencies = GetRates(EnterDate());
                                if (currencies.isCurrencyInCurrencyList(fragmentCurrency))
                                {
                                    foreach (var item in currencies)
                                    {
                                        if (item.IsCurrencyNameFragmentStart(fragmentCurrency))
                                        {
                                            Console.WriteLine(item);
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Нет такой валюты");
                                }
                                Console.WriteLine("Нажмите любую клавишу...");

                                Console.ReadKey();
                                break;
                            }
                        case '4':
                            {
                                Console.Clear();
                                Console.WriteLine("\tСортировка по названию валюты\n\n");
                                Rates currencies = GetRates(EnterDate());
                                currencies.Sort();
                                foreach (var item in currencies)
                                {
                                    Console.WriteLine(item);
                                }
                                Console.WriteLine("Нажмите любую клавишу...");
                                Console.ReadKey();
                                break;
                            }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    Console.ReadKey();
                }                
                Console.Clear();
            } while (ch != 27);            
        }
    }
}
