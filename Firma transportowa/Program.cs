using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using FirmaTransportowa.Application.Mappings;

namespace FirmaTransportowa
{
    class Program
    {
        static List<Pojazd> flota = new List<Pojazd>();
        public static List<Kierowca> kierowcy = new List<Kierowca>();
        static List<Trasa> trasy = new List<Trasa>();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Zarzadzanie pojazdami");
                Console.WriteLine("2. Zarzadanie kierowcami");
                Console.WriteLine("3. Trasy");
                Console.WriteLine("4. Wyjdz");
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
                        MenuTras();
                        break;
                    case "4":
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
                Console.WriteLine("3. Lista zarejestrowanych aut (DTO)");
                Console.WriteLine("4. Zarzadzaj konkretnym pojazdem");
                Console.WriteLine("5. Wroc do menu glownego");
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
                        WyswietlPojazdyDto();
                        break;
                    case "4":
                        Pojazd wybrany = WybierzPojazd();
                        if (wybrany != null)
                        {
                            MenuAkcjiPojazdu(wybrany);
                        }
                        break;
                    case "5":
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
            Console.Write("Podaj przebieg podczas ostatniego przeglądu/serwisu (km): ");
            int.TryParse(Console.ReadLine(), out int przebiegPrzegladu);
            Console.Write("Podaj date waznosci przegladu (RRRR-MM-DD): ");
            DateTime.TryParse(Console.ReadLine(), out DateTime przeglad);
            Console.Write("Podaj date waznosci ubezpieczenia (RRRR-MM-DD):  ");
            DateTime.TryParse(Console.ReadLine(), out DateTime oc);

            Pojazd noweAuto;
            try
            {
                noweAuto = new Pojazd(
                    marka,
                    model,
                    vin,
                    rok,
                    przebieg,
                    przebiegPrzegladu,
                    przeglad,
                    oc);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Błąd danych: {ex.Message}");
                Console.WriteLine("Naciśnij dowolny klawisz...");
                Console.ReadKey();
                return;
            }
 
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
                    int dniDoPrzegladu = (auto.WaznoscBadaniaTechnicznego.Date - DateTime.Now.Date).Days;
                    int dniDoOc = (auto.waznoscPolisyOC.Date - DateTime.Now.Date).Days;

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

                    string przegladInfo = dniDoPrzegladu >= 0
                        ? $"{dniDoPrzegladu} dni"
                        : $"Wygasło {-dniDoPrzegladu} dni temu";

                    string ocInfo = dniDoOc >= 0
                        ? $"{dniDoOc} dni"
                        : $"Wygasło {-dniDoOc} dni temu";

                    Console.ResetColor();
                    Console.WriteLine($"   -> Przegląd: {auto.WaznoscBadaniaTechnicznego.ToShortDateString()} ({przegladInfo})");
                    Console.WriteLine($"   -> OC: {auto.waznoscPolisyOC.ToShortDateString()} ({ocInfo})");
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
                Console.WriteLine($"Przypisany kierowca: {(auto.PrzypisanyKierowca == null ? "-" : $"{auto.PrzypisanyKierowca.Imie} {auto.PrzypisanyKierowca.Nazwisko}")}.");
                
                bool czySprawny = auto.czyZdatnyDoJazdy(out string powod);
                if (!czySprawny) Console.WriteLine($"UWAGA: {powod}!");

                Console.WriteLine("1. Zaktualizuj przebieg");
                Console.WriteLine("2. Zaktaulizuj OC");
                Console.WriteLine("3. Zaktualizuj badanie techniczne");
                Console.WriteLine("4. Zglos usterke");
                Console.WriteLine("5. Wykonaj naprawe");
                Console.WriteLine("6. Historia serwisowa");
                Console.WriteLine("7. Przypisz kierowcę");
                Console.WriteLine("8. Usuń przypisanie kierowcy");
                Console.WriteLine("9. Wroc");

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
                        var wybranyKierowca = WybierzKierowce();
                        if (wybranyKierowca != null)
                        {
                            auto.PrzypiszKierowce(wybranyKierowca);
                            Console.WriteLine("Przypisano kierowcę.");
                            Thread.Sleep(1200);
                        }
                        break;
                    case "8":
                        auto.UsunPrzypisanieKierowcy();
                        Console.WriteLine("Usunięto przypisanie kierowcy.");
                        Thread.Sleep(1200);
                        break;
                    case "9":
                        return;
                }
                if (wybory != "1" && wybory != "2" && wybory != "3" && wybory != "4" && wybory != "5") 
                {
                    Thread.Sleep(3000);
                }
            }
        }

        static void WyswietlPojazdyDto()
        {
            if (flota.Count == 0)
            {
                Console.WriteLine("Brak aut na liscie.");
                Console.WriteLine("\nNacisnij dowolny klawisz...");
                Console.ReadKey();
                return;
            }

            foreach (var dto in flota.Select(p => p.ToDto()))
            {
                Console.WriteLine($"[{dto.Status}] {dto.Marka} {dto.Model} | VIN: {dto.Vin} | Przebieg: {dto.AktualnyPrzebieg} km");
                Console.WriteLine($"   -> Przegląd do: {dto.WaznoscBadaniaTechnicznego.ToShortDateString()} | OC do: {dto.WaznoscPolisyOC.ToShortDateString()}");
                Console.WriteLine($"   -> Serwis: ostatni przegląd przy {dto.PrzebiegOstatniegoPrzegladu} km, co {dto.CoIlePrzeglad} km");
            }

            Console.WriteLine("\nNacisnij dowolny klawisz...");
            Console.ReadKey();
        }

        static void MenuTras()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Rozpocznij trasę");
                Console.WriteLine("2. Zakończ trasę");
                Console.WriteLine("3. Historia tras kierowcy");
                Console.WriteLine("4. Wróć");
                Console.Write("Twój wybór: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        RozpocznijTrase();
                        break;
                    case "2":
                        ZakonczTrase();
                        break;
                    case "3":
                        HistoriaTrasKierowcy();
                        break;
                    case "4":
                        return;
                }
            }
        }

        static void RozpocznijTrase()
        {
            Console.Clear();
            var kierowca = WybierzKierowce();
            if (kierowca == null) return;

            var pojazd = WybierzPojazd();
            if (pojazd == null) return;

            if (pojazd.PrzypisanyKierowca == null || pojazd.PrzypisanyKierowca != kierowca)
            {
                Console.WriteLine("UWAGA: ten pojazd nie ma przypisanego tego kierowcy. Przypisuję teraz.");
                pojazd.PrzypiszKierowce(kierowca);
            }

            if (!pojazd.czyZdatnyDoJazdy(out string powod))
            {
                Console.WriteLine($"Nie można rozpocząć trasy: {powod}");
                Console.WriteLine("Naciśnij dowolny klawisz...");
                Console.ReadKey();
                return;
            }

            Console.Write("Opis trasy (np. Kraków -> Warszawa): ");
            var opis = Console.ReadLine() ?? "";

            var t = new Trasa(pojazd.Vin, $"{kierowca.Imie} {kierowca.Nazwisko}", opis);
            trasy.Add(t);
            Console.WriteLine("Rozpoczęto trasę. Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        static void ZakonczTrase()
        {
            Console.Clear();
            var aktywne = trasy.Where(t => t.Koniec == null).ToList();
            if (aktywne.Count == 0)
            {
                Console.WriteLine("Brak aktywnych tras.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < aktywne.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {aktywne[i].Kierowca} | VIN: {aktywne[i].VinPojazdu} | Start: {aktywne[i].Start} | {aktywne[i].Opis}");
            }
            Console.Write("Wybierz trasę do zakończenia (0 anuluj): ");
            if (!int.TryParse(Console.ReadLine(), out int numer) || numer < 1 || numer > aktywne.Count) return;

            var trasa = aktywne[numer - 1];
            Console.Write("Ile km przejechano na tej trasie?: ");
            int.TryParse(Console.ReadLine(), out int km);
            trasa.Zakoncz(km);

            var pojazd = flota.FirstOrDefault(p => p.Vin == trasa.VinPojazdu);
            if (pojazd != null && km > 0)
            {
                pojazd.UstawPrzebieg(pojazd.AktualnyPrzebieg + km);
            }

            Console.WriteLine("Zakończono trasę. Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        static void HistoriaTrasKierowcy()
        {
            Console.Clear();
            var kierowca = WybierzKierowce();
            if (kierowca == null) return;

            var imieNazw = $"{kierowca.Imie} {kierowca.Nazwisko}";
            var historia = trasy.Where(t => t.Kierowca == imieNazw).OrderByDescending(t => t.Start).ToList();
            if (historia.Count == 0)
            {
                Console.WriteLine("Brak tras dla tego kierowcy.");
                Console.ReadKey();
                return;
            }

            foreach (var t in historia)
            {
                var koniec = t.Koniec == null ? "AKTYWNA" : t.Koniec.Value.ToString();
                var km = t.PrzejechaneKm == null ? "-" : t.PrzejechaneKm.Value.ToString();
                Console.WriteLine($"* {t.Start} -> {koniec} | km: {km} | VIN: {t.VinPojazdu} | {t.Opis}");
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz...");
            Console.ReadKey();
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
                        Console.Clear();
                        Console.WriteLine("Jaka chcialbys dodac kategorie: ");
                        string dodajKategoria = Console.ReadLine().Trim().ToUpper();
                        
                        if (string.IsNullOrEmpty(dodajKategoria))
                        {
                            Console.WriteLine("Nie podano nazwy kategorii.");
                            break; 
                        }

                        Console.WriteLine($"Podaj date waznosci dla {dodajKategoria} (RRRR-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime pierwszaData))
                        {
                            szofer.DodajUprawnienie(dodajKategoria, pierwszaData);
                            Console.WriteLine($"\nSukces! Dodano kategorię {dodajKategoria}.");
                        }
                        else
                        {
                            Console.WriteLine("Podano zly format daty.");
                        }
                        break;
                    // case "3":
                    //     Console.Clear();
                    //     
                    //     break;
                    // case"4":
                    //     break;
                    // case"5":
                    //     
                    //     break;
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