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
        public static List<Kierowca> kierowcy = new List<Kierowca>();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Zarzadzanie pojazdami");
                Console.WriteLine("2. Zarzadanie kierowcami");
                Console.WriteLine("3. Wyjdz");
                Console.Write("Twoj wybor: ");
                
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        MenuPojazdow();
                        break;
                    case "2":
                        MenuKierowcow();
                        break;
                    case "3":
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
                Console.WriteLine("1. Rejestracja nowego samochodu");
                Console.WriteLine("2. Lista zarejestrowanych aut");
                Console.WriteLine("3. Zarzadzaj konkretnym pojazdem");
                Console.WriteLine("4. Wroc do menu glownego");
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
                        Pojazd wybrany = WybierzPojazd();
                        if (wybrany != null)
                        {
                            MenuAkcjiPojazdu(wybrany);
                        }
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Wybrales zla opcje!");
                        break;
                }
                
            }
        }

        static void DodajNowyPojazd()
        {
            Console.Clear();
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
            Console.Write("Podaj date waznosci ubezpieczenia (RRRR-MM-DD):  ");
            DateTime.TryParse(Console.ReadLine(), out DateTime oc);

            Pojazd noweAuto = new Pojazd(
                marka,
                model,
                vin,
                rok,
                przebieg,
                przeglad,
                oc);
 
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
                    bool gotowy = auto.czyZdatnyDoJazdy(out string powod);
                    if (gotowy)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    string statusGotowosci = gotowy ? "[GOTOWY]" : "[NIEZDATNY]";
                    Console.WriteLine($"{statusGotowosci} [{auto.Status}] {auto.Marka} {auto.Model} - Przebieg: {auto.AktualnyPrzebieg}km");
                    Console.ResetColor();
                    Console.WriteLine($"   -> Przegląd do: {auto.WaznoscBadaniaTechnicznego.ToShortDateString()}");
                    Console.WriteLine($"   -> OC do: {auto.waznoscPolisyOC.ToShortDateString()}");
                }
            }
            Console.WriteLine("\nNacisnij dowolny klawisz...");
            Console.ReadKey();
        }

        static Pojazd WybierzPojazd()
        {
            Console.Clear();
            Console.WriteLine("<---Wybierz pojazd z listy ponizej.--->");

            if (flota.Count == 0)
            {
                Console.WriteLine("Brak aut na liscie.");
                Console.ReadKey();
                return null;
            }

            for (int i=0; i<flota.Count; i++)
            {
                Console.WriteLine($"{i+1}. {flota[i].Marka} {flota[i].Model} ({flota[i]. Vin})");
            }
            Console.Write("\nPodaj numer pojazdu (0 jesli chcesz anulowac): ");
            if(int.TryParse(Console.ReadLine(), out int numer) && numer > 0 && numer <= flota.Count)
            {
                return flota[numer - 1];
            }
            return null;
        }

        static void MenuAkcjiPojazdu(Pojazd auto)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Zarzadamy {auto.Marka} {auto.Model}.");
                Console.WriteLine($"Status: {auto.Status}.");
                Console.WriteLine($"Aktualny przebieg: {auto.AktualnyPrzebieg} km.");
                
                bool czySprawny = auto.czyZdatnyDoJazdy(out string powod);
                if (!czySprawny) Console.WriteLine($"UWAGA: {powod}!");

                Console.WriteLine("1. Zaktualizuj przebieg");
                Console.WriteLine("2. Zaktaulizuj OC");
                Console.WriteLine("3. Zaktualizuj badanie techniczne");
                Console.WriteLine("4. Zglos usterke");
                Console.WriteLine("5. Wykonaj naprawe");
                Console.WriteLine("6. Historia serwisowa");
                Console.WriteLine("7. Wroc");

                Console.Write("Wybor: ");
                string wybory = Console.ReadLine();

                switch (wybory)
                {
                    case "1":
                        Console.Write("Podaj nowy przebieg: ");
                        if (int.TryParse(Console.ReadLine(), out int nowyPrzebieg))
                        {
                            if (nowyPrzebieg >= auto.AktualnyPrzebieg)
                            {
                                auto.UstawPrzebieg(nowyPrzebieg);
                                Console.Write("Zapisano.");
                            }
                            else
                            {
                                Console.Write("Nie mozna cofnac licznika.");
                            }
                        }
                        break;
                    case "2":
                        Console.Write("Podaj date konca nowego OC(RRRR-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime noweOC))
                        {
                            if (noweOC >= auto.waznoscPolisyOC)
                            {
                                auto.UstawWaznoscOc(noweOC);
                                Console.Write("Zapisano.");
                            }
                            else
                            {
                                Console.Write("Nie mozna zaktutalizowac oc do tylu.");
                            }
                        }
                        break;
                    case "3":
                        Console.Write("Podaj date konca nowego badania technicznego(RRRR-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime noweBadania))
                        {
                            if (noweBadania >= auto.WaznoscBadaniaTechnicznego)
                            {
                                auto.UstawWaznoscBadaniaTechnicznego(noweBadania);
                                Console.Write("Zapisano.");
                            }
                            else
                            {
                                Console.Write("Nie mozna zaktutalizowac oc do tylu.");
                            }
                        }
                        break;
                    case "4":
                        Console.Write("Podaj usterke/zdarzenie: ");
                        string opis = Console.ReadLine();
                        
                        Console.Write("Czy usterka jest krytyczna(t/n): ");
                        bool krytyczna = Console.ReadLine().ToLower() == "t";
                        
                        auto.ZglosUsterke(opis, krytyczna);
                        Console.WriteLine("Zgloszenie przyjete!");
                        break;
                    case "5":
                        auto.WykonajNaprawe();
                        Console.WriteLine("Wszystkie usterki oznaczone jako rozwiazane. Status auta dostepny.");
                        break;
                    case "6":
                        if (auto.HistoriaSerwisow.Count==0)
                        {
                            Console.WriteLine("Historia serwisow jest pusta.");
                        }
                        else
                        {
                            foreach (var wpis in auto.HistoriaSerwisow)
                            {
                                string stan = wpis.czyRozwiazana ? "[NAPRAWIONE]" : "[AKTYWNE]";
                                string waga = wpis.czyKrytyczna ? "KRYTYCZNA" : "Info";
                                Console.WriteLine($"* {wpis.dataZgloszenia.ToShortDateString()} - {stan} {waga}: {wpis.opis}");
                            }
                        }
                        Console.WriteLine("\nNacisnij dowolny klawisz aby wrocic...");
                        Console.ReadKey();
                        break;
                    case "7":
                        return;
                }
                if (wybory != "1" && wybory != "2" && wybory != "3" && wybory != "4" && wybory != "5") 
                {
                    Thread.Sleep(3000);
                }
            }
        }

        static void MenuKierowcow()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Rejestracja nowego kierowcy");
                Console.WriteLine("2. Zarzadzaj konkretnym kierowcą");
                Console.WriteLine("3. Wroc do menu glownego");
                Console.Write("Twoj wybor: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        DodajNowegoKierowce();
                        break;
                    case "2":
                        Kierowca wybrany = WybierzKierowce();
                        if (wybrany != null)
                        {
                            MenuAkcjiKierowcy(wybrany);
                        }
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Wybrales zla opcje!");
                        break;
                }
            }
        }

        static void DodajNowegoKierowce()
        {
            Console.Clear();
            Console.Write("Podaj imie kierowcy: ");
            string imie = Console.ReadLine();
           
            Console.Write("Podaj nazwisko kierowcy: ");
            string nazwisko = Console.ReadLine();
            
            Kierowca nowySzofer = new Kierowca(imie, nazwisko);
            
            Console.WriteLine("Podaj kategorie prawa jazdy oddzielone po przecinku: ");
            string input =  Console.ReadLine();
            string[] listaKategorii = input.Split(',');
            
            foreach (string kategoria in listaKategorii)
            {
                string rodzajKategorii = kategoria.Trim();
                if (string.IsNullOrEmpty(rodzajKategorii)) continue;
                Console.Write($"Podaj datę ważności dla kategorii {rodzajKategorii} (RRRR-MM-DD): ");
                DateTime.TryParse(Console.ReadLine(), out DateTime dataWaznosci);
                nowySzofer.DodajUprawnienie(rodzajKategorii, dataWaznosci);
            }
            kierowcy.Add(nowySzofer);
    
            Console.WriteLine("Dodano kierowcę z uprawnieniami!");
            Console.ReadKey();
            // Console.Write("Podaj date waznosci prawa jazdy: ");
            // DateTime.TryParse(Console.ReadLine(), out DateTime dataWaznosci);
        }

        static Kierowca WybierzKierowce()
        {
            Console.Clear();
            Console.WriteLine("<---Wybierz kierowce z listy ponizej.--->");

            if (kierowcy.Count == 0)
            {
                Console.WriteLine("Brak kierowcow na liscie.");
                Console.ReadKey();
                return null;
            }

            for (int i=0; i<kierowcy.Count; i++)
            {
                string kategorieNapis = string.Join(",", kierowcy[i].KategoriaPrawaJazdy.Select(u => u.Kategoria));
                Console.WriteLine($"{i+1}. {kierowcy[i].Imie} {kierowcy[i].Nazwisko} Kategorie:({kategorieNapis})");
            }
            Console.Write("\nPodaj numer kierowcy (0 jesli chcesz anulowac): ");
            if(int.TryParse(Console.ReadLine(), out int numer) && numer > 0 && numer <= kierowcy.Count)
            {
                return kierowcy[numer - 1];
            }
            return null;
        }

        static void MenuAkcjiKierowcy(Kierowca szofer)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"----ZARZADZANIE: {szofer.Imie} {szofer.Nazwisko} ----");
                Console.WriteLine("\nAktualne uprawnienia:");
                foreach (var upr in szofer.KategoriaPrawaJazdy) 
                {
                    if (upr.dataWaznosci < DateTime.Now) 
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[WYGASŁE] ");
                    }
                    else 
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[WAŻNE]   ");
                    }
                    Console.WriteLine($"Kat. {upr.Kategoria} - do: {upr.dataWaznosci.ToShortDateString()}");
                    Console.ResetColor();
                }
                Console.WriteLine("----------------------------------");

                Console.WriteLine("1. Zaktualizuj date waznosci kategorii");
                Console.WriteLine("2. Dodaj nowa kategorie");
                // Console.WriteLine("3. Przypisz/usun auto");
                // Console.WriteLine("4. Przypisz/usun trase");
                // Console.WriteLine("5. Usun kierowce");
                Console.WriteLine("6. Wroc");

                Console.Write("Wybor: ");
                string wybory = Console.ReadLine();

                switch (wybory)
                {
                    case "1":
                        Console.Write("Podaj symbol kategorii do edycji (np.B): ");
                        string szukanaKat =  Console.ReadLine().Trim().ToUpper();
                        
                        var uprawnienieDoZmiany = szofer.KategoriaPrawaJazdy.FirstOrDefault(u => u.Kategoria == szukanaKat);

                        if (uprawnienieDoZmiany != null)
                        {
                            Console.Write($"Podaj nowa date waznosci dla {szukanaKat} (RRRR-MM-DD): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime nowaData))
                            {
                                uprawnienieDoZmiany.ZmienDateWaznosci(nowaData);
                                Console.WriteLine("Zaktualizowano date.");
                            }
                            else Console.WriteLine("Błędna data.");
                        }
                        else
                        {
                            Console.WriteLine("Ten kierowca nie posiada takiej kategorii.");
                        }
                        break;
                    case "2":
                        Console.WriteLine("Jaka chcialbys dodac kategorie: ");
                        
                        break;
                    case "6":
                        return;
                }
                if (wybory == "1" || wybory == "2")
                {
                    Thread.Sleep(1500); 
                }
            }
        }
    }
}