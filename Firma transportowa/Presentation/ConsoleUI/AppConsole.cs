using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FirmaTransportowa.Application.Mappings;
using FirmaTransportowa.Application.Services;
using FirmaTransportowa.Domain.Entities;
using FirmaTransportowa.Domain.Enums;

namespace FirmaTransportowa.Presentation.ConsoleUI;

/// <summary>
/// Warstwa prezentacji: menu konsolowe i wejście/wyjście. Logika biznesowa jest w Application + Domain.
/// </summary>
public sealed class AppConsole
{
    private readonly PojazdAppService _pojazdAppService;
    private readonly KierowcaAppService _kierowcaAppService;
    private readonly TrasaAppService _trasaAppService;
    private readonly ZlecenieTransportoweAppService _zlecenieAppService;

    public AppConsole(
        PojazdAppService pojazdAppService,
        KierowcaAppService kierowcaAppService,
        TrasaAppService trasaAppService,
        ZlecenieTransportoweAppService zlecenieAppService)
    {
        _pojazdAppService = pojazdAppService;
        _kierowcaAppService = kierowcaAppService;
        _trasaAppService = trasaAppService;
        _zlecenieAppService = zlecenieAppService;
    }

    public void Run()
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

    private void MenuPojazdow()
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
                    var wybrany = WybierzPojazd();
                    if (wybrany != null)
                        MenuAkcjiPojazdu(wybrany);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Wybrales zla opcje!");
                    break;
            }
        }
    }
    
    private void DodajNowyPojazd()
    {
        Console.Clear();
        Console.Write("Podaj marke auta: ");
        string marka = Console.ReadLine() ?? "";
        Console.Write("Podaj model auta: ");
        string model = Console.ReadLine() ?? "";
        Console.Write("Podaj numer vin auta:");
        string vin = Console.ReadLine() ?? "";

        Console.Write("Podaj rok auta: ");
        int.TryParse(Console.ReadLine(), out int rok);
        Console.Write("Podaj aktualny przebieg w (km): ");
        int.TryParse(Console.ReadLine(), out int przebieg);
        Console.Write("Podaj przebieg podczas ostatniego przeglądu/serwisu (km): ");
        int.TryParse(Console.ReadLine(), out int przebiegPrzegladu);
        Console.Write("Podaj wymaganą kategorię prawa jazdy dla tego pojazdu (np. B/C/C+E): ");
        string wymaganaKategoria = Console.ReadLine() ?? "";
        Console.Write("Podaj date waznosci przegladu (RRRR-MM-DD): ");
        DateTime.TryParse(Console.ReadLine(), out DateTime przeglad);
        Console.Write("Podaj date waznosci ubezpieczenia (RRRR-MM-DD):  ");
        DateTime.TryParse(Console.ReadLine(), out DateTime oc);

        var result = _pojazdAppService.RegisterVehicle(
            marka, model, vin, rok, przebieg, przebiegPrzegladu, wymaganaKategoria, przeglad, oc);

        if (!result.Success)
        {
            Console.WriteLine($"Błąd danych: {result.Message}");
            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Pojazd dodany pomyślnie! Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    private void WyswietlPojazdy()
    {
        var flota = _pojazdAppService.GetAll().ToList();
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
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                string statusGotowosci = gotowy ? "[GOTOWY]" : "[NIEZDATNY]";
                string kierowcaInfo = auto.PrzypisanyKierowca != null
                    ? $"{auto.PrzypisanyKierowca.Imie} {auto.PrzypisanyKierowca.Nazwisko}"
                    : "Brak kierowcy";

                Console.WriteLine(
                    $"{statusGotowosci} [{auto.Status}] {auto.Marka} {auto.Model} - Przebieg: {auto.AktualnyPrzebieg}km | Szofer: {kierowcaInfo}");

                string przegladInfo = dniDoPrzegladu >= 0
                    ? $"{dniDoPrzegladu} dni"
                    : $"Wygasło {-dniDoPrzegladu} dni temu";

                string ocInfo = dniDoOc >= 0
                    ? $"{dniDoOc} dni"
                    : $"Wygasło {-dniDoOc} dni temu";

                Console.ResetColor();
                Console.WriteLine(
                    $"   -> Przegląd: {auto.WaznoscBadaniaTechnicznego.ToShortDateString()} ({przegladInfo})");
                Console.WriteLine($"   -> OC: {auto.waznoscPolisyOC.ToShortDateString()} ({ocInfo})");
                if (!gotowy)
                    Console.WriteLine($"   -> Powód: {powod}");
            }
        }

        Console.WriteLine("\nNacisnij dowolny klawisz...");
        Console.ReadKey();
    }

    private Pojazd? WybierzPojazd()
    {
        Console.Clear();
        Console.WriteLine("<---Wybierz pojazd z listy ponizej.--->");

        var flota = _pojazdAppService.GetAll().ToList();
        if (flota.Count == 0)
        {
            Console.WriteLine("Brak aut na liscie.");
            Console.ReadKey();
            return null;
        }

        for (int i = 0; i < flota.Count; i++)
            Console.WriteLine($"{i + 1}. {flota[i].Marka} {flota[i].Model} ({flota[i].Vin})");

        Console.Write("\nPodaj numer pojazdu (0 jesli chcesz anulowac): ");
        if (int.TryParse(Console.ReadLine(), out int numer) && numer > 0 && numer <= flota.Count)
            return flota[numer - 1];
        return null;
        
    }

    private void MenuAkcjiPojazdu(Pojazd auto)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Zarzadamy {auto.Marka} {auto.Model}.");
            Console.WriteLine($"Status: {auto.Status}.");
            Console.WriteLine($"Aktualny przebieg: {auto.AktualnyPrzebieg} km.");
            Console.WriteLine(
                $"Przypisany kierowca: {(auto.PrzypisanyKierowca == null ? "-" : $"{auto.PrzypisanyKierowca.Imie} {auto.PrzypisanyKierowca.Nazwisko}")}.");

            bool czySprawny = auto.czyZdatnyDoJazdy(out string powod);
            if (!czySprawny)
                Console.WriteLine($"UWAGA: {powod}!");

            Console.WriteLine("1. Zaktualizuj przebieg");
            Console.WriteLine("2. Zaktaulizuj OC");
            Console.WriteLine("3. Zaktualizuj badanie techniczne");
            Console.WriteLine("4. Zglos usterke");
            Console.WriteLine("5. Wykonaj naprawe");
            Console.WriteLine("6. Dodaj wpis przeglądu");
            Console.WriteLine("7. Dodaj wpis wymiany części");
            Console.WriteLine("8. Historia serwisowa");
            Console.WriteLine("9. Zmień status pojazdu");
            Console.WriteLine("10. Przypisz kierowcę");
            Console.WriteLine("11. Usuń przypisanie kierowcy");
            Console.WriteLine("12. Wroc");

            Console.Write("Wybor: ");
            string wybory = Console.ReadLine() ?? "";

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
                            Console.Write("Nie mozna cofnac licznika.");
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
                            Console.Write("Nie mozna zaktutalizowac oc do tylu.");
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
                            Console.Write("Nie mozna zaktutalizowac oc do tylu.");
                    }
                    break;
                case "4":
                    Console.Write("Podaj usterke/zdarzenie: ");
                    string opis = Console.ReadLine() ?? "";
                    Console.Write("Czy usterka jest krytyczna(t/n): ");
                    bool krytyczna = Console.ReadLine()?.ToLower() == "t";
                    auto.ZglosUsterke(opis, krytyczna);
                    Console.WriteLine("Zgloszenie przyjete!");
                    break;
                case "5":
                    auto.WykonajNaprawe();
                    Console.WriteLine("Wszystkie usterki oznaczone jako rozwiazane. Status auta dostepny.");
                    break;
                case "6":
                    Console.Write("Opis przeglądu: ");
                    string opisPrzegladu = Console.ReadLine() ?? "";
                    Console.Write("Podaj przebieg podczas tego przeglądu: ");
                    if (int.TryParse(Console.ReadLine(), out int przebiegPrzegladuNowy))
                    {
                        try
                        {
                            auto.DodajPrzeglad(opisPrzegladu, przebiegPrzegladuNowy);
                            Console.WriteLine("Dodano wpis przeglądu.");
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Błąd: {ex.Message}");
                        }
                    }
                    break;
                case "7":
                    Console.Write("Opis wymienionej części / czynności: ");
                    string opisWymiany = Console.ReadLine() ?? "";
                    auto.DodajWymianeCzesci(opisWymiany);
                    Console.WriteLine("Dodano wpis wymiany części.");
                    break;
                case "8":
                    if (auto.HistoriaSerwisow.Count == 0)
                        Console.WriteLine("Historia serwisow jest pusta.");
                    else
                    {
                        foreach (var wpis in auto.HistoriaSerwisow)
                        {
                            string stan = wpis.czyRozwiazana ? "[NAPRAWIONE]" : "[AKTYWNE]";
                            string waga = wpis.czyKrytyczna ? "KRYTYCZNA" : "Info";
                            Console.WriteLine(
                                $"* {wpis.dataZgloszenia.ToShortDateString()} - [{wpis.TypWpisu}] {stan} {waga}: {wpis.opis}");
                        }
                    }
                    Console.WriteLine("\nNacisnij dowolny klawisz aby wrocic...");
                    Console.ReadKey();
                    break;
                case "9":
                    Console.WriteLine("Wybierz nowy status:");
                    Console.WriteLine("1. Dostepny");
                    Console.WriteLine("2. WTrasie");
                    Console.WriteLine("3. Serwis");
                    string wyborStatusu = Console.ReadLine() ?? "";
                    switch (wyborStatusu)
                    {
                        case "1":
                            auto.UstawStatus(StatusPojazdu.Dostepny);
                            Console.WriteLine("Status zmieniono na Dostepny.");
                            break;
                        case "2":
                            auto.UstawStatus(StatusPojazdu.WTrasie);
                            Console.WriteLine("Status zmieniono na WTrasie.");
                            break;
                        case "3":
                            auto.UstawStatus(StatusPojazdu.Serwis);
                            Console.WriteLine("Status zmieniono na Serwis.");
                            break;
                        default:
                            Console.WriteLine("Niepoprawna opcja.");
                            break;
                    }
                    break;
                case "10":
                    var wybranyKierowca = WybierzKierowce();
                    if (wybranyKierowca != null)
                    {
                        var assign = _pojazdAppService.TryAssignDriver(auto, wybranyKierowca);
                        if (!assign.Success)
                        {
                            Console.WriteLine($"Nie można przypisać kierowcy: {assign.Message}");
                            Console.ReadKey();
                            break;
                        }
                        Console.WriteLine("Przypisano kierowcę.");
                        Thread.Sleep(1200);
                    }
                    break;
                case "11":
                    if (auto.PrzypisanyKierowca != null &&
                        _trasaAppService.DriverHasActiveRouteWithVehicle(auto.PrzypisanyKierowca, auto))
                    {
                        Console.WriteLine("Nie można usunąć przypisania: kierowca jest aktualnie w trasie tym pojazdem.");
                        Console.WriteLine("Najpierw zakończ trasę.");
                        Console.ReadKey();
                        break;
                    }

                    auto.UsunPrzypisanieKierowcy();
                    Console.WriteLine("Usunięto przypisanie kierowcy.");
                    Thread.Sleep(1200);
                    break;
                case "12":
                    return;
            }

            if (wybory != "1" && wybory != "2" && wybory != "3" && wybory != "4" && wybory != "5")
                Thread.Sleep(3000);
        }
    }

    private void WyswietlPojazdyDto()
    {
        var flota = _pojazdAppService.GetAll().ToList();
        if (flota.Count == 0)
        {
            Console.WriteLine("Brak aut na liscie.");
            Console.WriteLine("\nNacisnij dowolny klawisz...");
            Console.ReadKey();
            return;
        }

        foreach (var dto in flota.Select(p => p.ToDto()))
        {
            Console.WriteLine(
                $"[{dto.Status}] {dto.Marka} {dto.Model} | VIN: {dto.Vin} | Przebieg: {dto.AktualnyPrzebieg} km");
            Console.WriteLine(
                $"   -> Wymagana kategoria: {dto.WymaganaKategoriaPrawaJazdy} | Kierowca: {dto.PrzypisanyKierowca ?? "Brak"}");
            Console.WriteLine(
                $"   -> Przegląd do: {dto.WaznoscBadaniaTechnicznego.ToShortDateString()} | OC do: {dto.WaznoscPolisyOC.ToShortDateString()}");
            Console.WriteLine(
                $"   -> Serwis: ostatni przegląd przy {dto.PrzebiegOstatniegoPrzegladu} km, co {dto.CoIlePrzeglad} km");
        }

        Console.WriteLine("\nNacisnij dowolny klawisz...");
        Console.ReadKey();
    }

    private void MenuTras()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Zlecenia transportowe");
            Console.WriteLine("2. Rozpocznij trasę (ze zlecenia)");
            Console.WriteLine("3. Zakończ trasę");
            Console.WriteLine("4. Historia tras kierowcy");
            Console.WriteLine("5. Wróć");
            Console.Write("Twój wybór: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    MenuZlecen();
                    break;
                case "2":
                    RozpocznijTraseZeZlecenia();
                    break;
                case "3":
                    ZakonczTrase();
                    break;
                case "4":
                    HistoriaTrasKierowcy();
                    break;
                case "5":
                    return;
            }
        }
    }

    private void MenuZlecen()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Utwórz zlecenie");
            Console.WriteLine("2. Lista zleceń");
            Console.WriteLine("3. Wróć");
            Console.Write("Twój wybór: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    UtworzZlecenie();
                    break;
                case "2":
                    ListaZlecen();
                    break;
                case "3":
                    return;
            }
        }
    }

    private void UtworzZlecenie()
    {
        Console.Clear();
        Console.Write("Start (miasto): ");
        var start = Console.ReadLine() ?? "";
        Console.Write("Cel (miasto): ");
        var cel = Console.ReadLine() ?? "";
        Console.Write("Towar: ");
        var towar = Console.ReadLine() ?? "";

        var result = _zlecenieAppService.TryCreate(start, cel, towar, out var created);
        if (!result.Success)
        {
            Console.WriteLine($"Błąd: {result.Message}");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Utworzono zlecenie. Id: {created!.Id}");
        Console.ReadKey();
    }

    private void ListaZlecen()
    {
        Console.Clear();
        var zlecenia = _zlecenieAppService.GetAll().OrderByDescending(z => z.DataUtworzenia).ToList();
        if (zlecenia.Count == 0)
        {
            Console.WriteLine("Brak zleceń.");
            Console.ReadKey();
            return;
        }

        foreach (var z in zlecenia)
        {
            Console.WriteLine($"[{z.Status}] {z.StartMiejsce} -> {z.CelMiejsce} | Towar: {z.Towar} | Id: {z.Id}");
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    private void RozpocznijTraseZeZlecenia()
    {
        Console.Clear();
        var zlecenia = _zlecenieAppService.GetDoRealizacji().ToList();
        if (zlecenia.Count == 0)
        {
            Console.WriteLine("Brak zleceń do realizacji (Nowe).");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Wybierz zlecenie do rozpoczęcia:");
        for (int i = 0; i < zlecenia.Count; i++)
        {
            var z = zlecenia[i];
            Console.WriteLine($"{i + 1}. {z.StartMiejsce} -> {z.CelMiejsce} | Towar: {z.Towar}");
        }
        Console.Write("Numer zlecenia (0 anuluj): ");
        if (!int.TryParse(Console.ReadLine(), out int nrZl) || nrZl < 1 || nrZl > zlecenia.Count)
            return;
        var zlecenie = zlecenia[nrZl - 1];

        var kierowca = WybierzKierowce();
        if (kierowca == null)
            return;

        var pojazd = WybierzPojazd();
        if (pojazd == null)
            return;

        if (pojazd.PrzypisanyKierowca == null || pojazd.PrzypisanyKierowca != kierowca)
            Console.WriteLine("UWAGA: ten pojazd nie ma przypisanego tego kierowcy. Przypisuję teraz.");

        var start = _trasaAppService.TryStartRoute(kierowca, pojazd, zlecenie);
        if (!start.Success)
        {
            Console.WriteLine($"Nie można rozpocząć trasy: {start.Message}");
            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Rozpoczęto trasę. Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    private void ZakonczTrase()
    {
        Console.Clear();
        var aktywne = _trasaAppService.GetActiveAll().ToList();
        if (aktywne.Count == 0)
        {
            Console.WriteLine("Brak aktywnych tras.");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < aktywne.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {aktywne[i].Kierowca} | VIN: {aktywne[i].VinPojazdu} | Start: {aktywne[i].Start} | {aktywne[i].Opis} | Towar: {aktywne[i].Towar}");
        }
        Console.Write("Wybierz trasę do zakończenia (0 anuluj): ");
        if (!int.TryParse(Console.ReadLine(), out int numer) || numer < 1 || numer > aktywne.Count)
            return;

        var trasa = aktywne[numer - 1];
        Console.Write("Ile km przejechano na tej trasie?: ");
        int.TryParse(Console.ReadLine(), out int km);
        Console.Write("Ile litrow paliwa bylo zatankowane na koncu trasy?: ");
        decimal.TryParse(Console.ReadLine(), out decimal litry);
        Console.Write("Jaka byla srednia cena paliwa za litr?: ");
        decimal.TryParse(Console.ReadLine(), out decimal cena);
        Console.Write("Jakie byly koszty dodatkowe(parking,autostrada itp.)?: ");
        decimal.TryParse(Console.ReadLine(), out decimal dodatkowe);
        _trasaAppService.EndRoute(trasa, km, litry, cena, dodatkowe);

        Console.WriteLine("Zakończono trasę i podliczono koszty. Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    private void HistoriaTrasKierowcy()
    {
        Console.Clear();
        var kierowca = WybierzKierowce();
        if (kierowca == null)
            return;

        var imieNazw = $"{kierowca.Imie} {kierowca.Nazwisko}";
        var historia = _trasaAppService.GetHistoryForDriver(imieNazw).ToList();
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
            Console.WriteLine($"* {t.Start} -> {koniec} | km: {km} | VIN: {t.VinPojazdu} | {t.Opis} | Towar: {t.Towar} | Koszty: {t.KosztyCalkowite}zł");
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    private void MenuKierowcow()
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
                    var wybrany = WybierzKierowce();
                    if (wybrany != null)
                        MenuAkcjiKierowcy(wybrany);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Wybrales zla opcje!");
                    break;
            }
        }
    }

    private void DodajNowegoKierowce()
    {
        Console.Clear();
        Console.Write("Podaj imie kierowcy: ");
        string imie = Console.ReadLine() ?? "";
        Console.Write("Podaj nazwisko kierowcy: ");
        string nazwisko = Console.ReadLine() ?? "";

        var nowySzofer = new Kierowca(imie, nazwisko);

        Console.WriteLine("Podaj kategorie prawa jazdy oddzielone po przecinku: ");
        string input = Console.ReadLine() ?? "";
        string[] listaKategorii = input.Split(',');

        foreach (string kategoria in listaKategorii)
        {
            string rodzajKategorii = kategoria.Trim();
            if (string.IsNullOrEmpty(rodzajKategorii))
                continue;
            Console.Write($"Podaj datę ważności dla kategorii {rodzajKategorii} (RRRR-MM-DD): ");
            DateTime.TryParse(Console.ReadLine(), out DateTime dataWaznosci);
            nowySzofer.DodajUprawnienie(rodzajKategorii, dataWaznosci);
        }

        _kierowcaAppService.RegisterDriver(nowySzofer);

        Console.WriteLine("Dodano kierowcę z uprawnieniami!");
        Console.ReadKey();
        Console.WriteLine("Nacisnij dowolny klawisz aby wyjsc.....");
    }

    private Kierowca? WybierzKierowce()
    {
        Console.Clear();
        Console.WriteLine("<---Wybierz kierowce z listy ponizej.--->");

        var kierowcy = _kierowcaAppService.GetAll().ToList();
        var flota = _pojazdAppService.GetAll().ToList();

        if (kierowcy.Count == 0)
        {
            Console.WriteLine("Brak kierowcow na liscie.");
            Console.ReadKey();
            return null;
        }

        for (int i = 0; i < kierowcy.Count; i++)
        {
            var k = kierowcy[i];
            string kategorieNapis = string.Join(",", k.KategoriaPrawaJazdy.Select(u => u.Kategoria));
            var auto = flota.FirstOrDefault(p => p.PrzypisanyKierowca == k);
            string autoInfo = auto != null ? $"[Auto: {auto.Marka}]" : "[Brak auta]";
            string pelneImie = $"{k.Imie} {k.Nazwisko}";
            bool wTrasie = _trasaAppService.DriverHasActiveRoute(k);
            string statusTrasy = wTrasie ? "(W TRASIE)" : "";
            Console.WriteLine($"{i + 1}. {k.Imie} {k.Nazwisko} {autoInfo} {statusTrasy} - Kat: ({kategorieNapis})");
        }

        Console.Write("\nPodaj numer kierowcy (0 jesli chcesz anulowac): ");
        if (int.TryParse(Console.ReadLine(), out int numer) && numer > 0 && numer <= kierowcy.Count)
            return kierowcy[numer - 1];
        return null;
    }

    private void MenuAkcjiKierowcy(Kierowca szofer)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"----ZARZADZANIE: {szofer.Imie} {szofer.Nazwisko} ----");

            var flota = _pojazdAppService.GetAll().ToList();
            var przypisaneAuto1 = flota.FirstOrDefault(p => p.PrzypisanyKierowca == szofer);
            string autoInfo = przypisaneAuto1 != null
                ? $"{przypisaneAuto1.Marka} {przypisaneAuto1.Model} ({przypisaneAuto1.Vin})"
                : "Brak";

            string imieNazw = $"{szofer.Imie} {szofer.Nazwisko}";
            var aktywneTrasy = _trasaAppService.GetActiveAll().Where(t => t.Kierowca == imieNazw).ToList();
            var aktywnaTrasa = aktywneTrasy.FirstOrDefault();
            string trasaInfo = aktywnaTrasa != null ? aktywnaTrasa.Opis : "Brak";

            Console.WriteLine($"Przypisany pojazd: {autoInfo}");
            Console.WriteLine($"Aktywna trasa:     {trasaInfo}");
            Console.WriteLine("----------------------------------");
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
            Console.WriteLine("3. Przypisz/usun auto");
            Console.WriteLine("4. Usun kierowce");
            Console.WriteLine("5. Wroc");

            Console.Write("Wybor: ");
            string wybory = Console.ReadLine() ?? "";

            switch (wybory)
            {
                case "1":
                    Console.Write("Podaj symbol kategorii do edycji (np.B): ");
                    string szukanaKat = Console.ReadLine()?.Trim().ToUpper() ?? "";
                    var uprawnienieDoZmiany = szofer.KategoriaPrawaJazdy.FirstOrDefault(u => u.Kategoria == szukanaKat);
                    if (uprawnienieDoZmiany != null)
                    {
                        Console.Write($"Podaj nowa date waznosci dla {szukanaKat} (RRRR-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime nowaData))
                        {
                            uprawnienieDoZmiany.ZmienDateWaznosci(nowaData);
                            Console.WriteLine("Zaktualizowano date.");
                        }
                        else
                            Console.WriteLine("Błędna data.");
                    }
                    else
                        Console.WriteLine("Ten kierowca nie posiada takiej kategorii.");
                    break;
                case "2":
                    Console.Clear();
                    Console.WriteLine("Jaka chcialbys dodac kategorie: ");
                    string dodajKategoria = Console.ReadLine()?.Trim().ToUpper() ?? "";
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
                        Console.WriteLine("Podano zly format daty.");
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine($"--- AUTO DLA: {szofer.Imie} {szofer.Nazwisko} ---");
                    var przypisaneAuto = flota.FirstOrDefault(p => p.PrzypisanyKierowca == szofer);
                    if (przypisaneAuto != null)
                    {
                        Console.WriteLine("Kierowca ma juz przypisane auto: ");
                        Console.WriteLine($"> {przypisaneAuto.Marka} {przypisaneAuto.Model} (VIN: {przypisaneAuto.Vin}) <");
                        Console.WriteLine("Czy chcesz zwolnic to auto? (T/N): ");
                        if (Console.ReadLine()?.ToUpper() == "T")
                        {
                            if (_trasaAppService.DriverHasActiveRouteWithVehicle(szofer, przypisaneAuto))
                            {
                                Console.WriteLine("Nie można zwolnić auta: kierowca jest aktualnie w trasie tym pojazdem.");
                                Console.WriteLine("Najpierw zakończ trasę.");
                                Console.ReadKey();
                                break;
                            }

                            przypisaneAuto.UsunPrzypisanieKierowcy();
                            Console.WriteLine("Pomyslnie usunieto przypisanie.");
                        }
                        else
                            Console.WriteLine("Anulowano.");
                    }
                    else
                    {
                        Console.WriteLine("Ten kierowca nie jest przypisany do zadnego auta.");
                        Console.WriteLine("Dostepne pojazdy we flocie (bez kierowcy).");
                        var wolneAuta = flota.Where(p => p.PrzypisanyKierowca == null).ToList();
                        if (wolneAuta.Count == 0)
                            Console.WriteLine("Nie ma zadnych wolnych aut.");
                        else
                        {
                            for (int i = 0; i < wolneAuta.Count; i++)
                                Console.WriteLine($"{i + 1}. {wolneAuta[i].Marka} {wolneAuta[i].Model} (VIN: {wolneAuta[i].Vin}) <");
                            Console.Write("Wybierz numer auta do przypisania(0 jesli chcesz anulowac).");
                            if (int.TryParse(Console.ReadLine(), out int nr) && nr > 0 && nr <= wolneAuta.Count)
                            {
                                var wybraneAuto = wolneAuta[nr - 1];
                                var assign = _kierowcaAppService.TryAssignVehicleToDriver(szofer, wybraneAuto);
                                if (!assign.Success)
                                {
                                    Console.WriteLine($"Nie można przypisać auta: {assign.Message}");
                                    Console.ReadKey();
                                    break;
                                }
                                Console.WriteLine($"Sukces! Przypisano {wybraneAuto.Marka} do kierowcy {szofer.Nazwisko}.");
                            }
                            else
                                Console.WriteLine("Anulowano.");
                        }
                    }
                    Console.WriteLine("Nacisnij dowolny klawisz aby wyjsc...");
                    Console.ReadKey();
                    break;
                case "4":
                    Console.Clear();
                    Console.WriteLine($"Czy na pewno chcesz usunac kierowce: {szofer.Imie} {szofer.Nazwisko}?");
                    Console.Write("Jesli chcesz potwierdzic napisz 'TAK'...");
                    if (Console.ReadLine()?.ToUpper() == "TAK")
                    {
                        var remove = _kierowcaAppService.TryRemoveDriver(szofer);
                        if (!remove.Success)
                        {
                            Console.WriteLine(remove.Message);
                            Console.ReadKey();
                            break;
                        }
                        Console.WriteLine("\nSUKCES: Kierowca został usunięty z systemu.");
                        Console.WriteLine("Nacisnij dowolny klawisz aby wyjsc...");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Anulowane usuwanie.");
                        Thread.Sleep(1000);
                    }
                    break;
                case "5":
                    return;
            }
            if (wybory == "1" || wybory == "2")
                Thread.Sleep(1500);
        }
    }
}
