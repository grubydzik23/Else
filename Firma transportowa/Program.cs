using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace FirmaTransportowa
{
    enum StatusPojazdu
    {
        Dostepny,
        WTrasie,
        Serwis
    }
    class Pojazd
    {
        public string Marka { get; set; }
        public string Model { get; set; }
        public int RokProdukcji { get; set; }
        public DateTime WaznoscBadaniaTechnicznego { get; set; } 
        public StatusPojazdu Status { get; set; }
        public string Vin { get; set; }
    }
    class Program
    {
        static List<Pojazd> flota = new List<Pojazd>();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1.Zarzadzanie pojazdami");
                Console.WriteLine("2.Wyjdz");
                Console.Write("Twoj wybor: ");
                
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        MenuPojazdow();
                        break;
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Wybrales zla opcje!");
                        break;
                }
            }
        }
        static void MenuPojazdow()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1.Rejestracja nowego samochodu");
                Console.WriteLine("2.Lista zarejestrowanych aut");
                Console.WriteLine("3.Wroc do menu glownego");
                Console.Write("Twoj wybor: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        DodajNowyPojazd();
                        break;
                    case "2":
                        WyswietlPojazdy();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Wybrales zla opcje!");
                        break;
                }
                
            }
        }

        static void DodajNowyPojazd()
        {
            Console.Write("Podaj marke auta: ");
            string marka = Console.ReadLine();
            Console.Write("Podaj model auta: ");
            string model = Console.ReadLine();
            Console.Write("Podaj rok auta: ");
            int rok = int.Parse(Console.ReadLine());
            Console.Write("Podaj numer vin auta:");
            string vin = Console.ReadLine();

            Pojazd noweAuto = new Pojazd();
            noweAuto.Marka = marka;
            noweAuto.Model = model;
            noweAuto.RokProdukcji = rok;
            noweAuto.Vin = vin;

            noweAuto.Status = StatusPojazdu.Dostepny;
            noweAuto.WaznoscBadaniaTechnicznego = DateTime.Now.AddYears(1);
            
            flota.Add(noweAuto);
            Console.WriteLine("Pojazd dodany pomyślnie! Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        static void WyswietlPojazdy()
        {
            if (flota.Count == 0)
            {
                Console.WriteLine("Brak aut na liscie.");
            }
            else
            {
                foreach (var auto in flota)
                {
                    Console.WriteLine($"[{auto.Status}] {auto.Marka} {auto.Model} ({auto.RokProdukcji}) - Przegląd do: {auto.WaznoscBadaniaTechnicznego.ToShortDateString()}");
                }
            }
            Console.WriteLine("\nNacisnij dowolny klawisz...");
            Console.ReadKey();
        }
    }
}