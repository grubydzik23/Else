using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace FirmaTransportowa
{
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
            Console.Write("Podaj numer vin auta:");
            string vin = Console.ReadLine();
            
            Console.Write("Podaj rok auta: ");
            int.TryParse(Console.ReadLine(), out int rok);
            Console.Write("Podaj aktualny przebieg w (km): ");
            int.TryParse(Console.ReadLine(), out int przebieg);
            Console.Write("Podaj date waznosci przegladu (RRRR-MM-DD): ");
            DateTime.TryParse(Console.ReadLine(), out DateTime przeglad);
            Console.Write("Podaj date waznosci ubezpieczenia (RRRR-DD-MM):  ");
            DateTime.TryParse(Console.ReadLine(), out DateTime oc);

            Pojazd noweAuto = new Pojazd();
            noweAuto.Marka = marka;
            noweAuto.Model = model;
            noweAuto.RokProdukcji = rok;
            noweAuto.Vin = vin;
            noweAuto.Przebieg = przebieg;
            noweAuto.WaznoscBadaniaTechnicznego = przeglad;
            noweAuto.waznoscPolisyOC = oc;

            noweAuto.Status = StatusPojazdu.Dostepny;
            
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
                    bool gotowy = auto.czyZdatnyDoJazdy();
                    if (gotowy)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    string statusGotowosci = gotowy ? "[GOTOWY]" : "[NIEZDATNY]";
                    Console.WriteLine($"{statusGotowosci} [{auto.Status}] {auto.Marka} {auto.Model} - Przebieg: {auto.Przebieg}km");
                    Console.ResetColor();
                    Console.WriteLine($"   -> Przegląd do: {auto.WaznoscBadaniaTechnicznego.ToShortDateString()}");
                    Console.WriteLine($"   -> OC do: {auto.waznoscPolisyOC.ToShortDateString()}");
                }
            }
            Console.WriteLine("\nNacisnij dowolny klawisz...");
            Console.ReadKey();
        }
    }
}