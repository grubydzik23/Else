using FirmaTransportowa.Domain.Enums;

namespace FirmaTransportowa.Domain.Entities;

public class Pojazd
{
    public string Marka { get; private set; }
    public string Model { get; private set; }
    public int RokProdukcji { get; private set; }
    public StatusPojazdu Status { get; private set; }
    public string Vin { get; private set; }

    public int AktualnyPrzebieg { get; private set; }
    public int PrzebiegOstatniegoPrzegladu { get; private set; }
    public int CoIlePrzeglad { get; } = 15000;

    public string WymaganaKategoriaPrawaJazdy { get; private set; }

    public DateTime waznoscPolisyOC { get; private set; }
    public DateTime WaznoscBadaniaTechnicznego { get; private set; }

    public List<SerwisPojazdu> HistoriaSerwisow { get; } = new List<SerwisPojazdu>();

    public Kierowca? PrzypisanyKierowca { get; private set; }

    public Pojazd(
        string marka,
        string model,
        string vin,
        int rokProdukcji,
        int aktualnyPrzebieg,
        int przebiegOstatniegoPrzegladu,
        string wymaganaKategoriaPrawaJazdy,
        DateTime waznoscBadaniaTechnicznego,
        DateTime waznoscPolisyOc)
    {
        Marka = marka;
        Model = model;
        Vin = vin;
        RokProdukcji = rokProdukcji;
        AktualnyPrzebieg = aktualnyPrzebieg;
        if (przebiegOstatniegoPrzegladu > aktualnyPrzebieg)
        {
            throw new ArgumentException("Przebieg ostatniego przeglądu nie może być większy niż aktualny przebieg.");
        }
        PrzebiegOstatniegoPrzegladu = przebiegOstatniegoPrzegladu;
        if (string.IsNullOrWhiteSpace(wymaganaKategoriaPrawaJazdy))
        {
            throw new ArgumentException("Kategoria prawa jazdy jest wymagana.");
        }
        WymaganaKategoriaPrawaJazdy = wymaganaKategoriaPrawaJazdy.Trim().ToUpper();
        WaznoscBadaniaTechnicznego = waznoscBadaniaTechnicznego;
        waznoscPolisyOC = waznoscPolisyOc;
        Status = StatusPojazdu.Dostepny;
    }

    public void UstawPrzebieg(int nowyPrzebieg)
    {
        AktualnyPrzebieg = nowyPrzebieg;
    }

    public void UstawWaznoscOc(DateTime nowaData)
    {
        waznoscPolisyOC = nowaData;
    }

    public void UstawWaznoscBadaniaTechnicznego(DateTime nowaData)
    {
        WaznoscBadaniaTechnicznego = nowaData;
    }

    public void UstawStatus(StatusPojazdu nowyStatus)
    {
        Status = nowyStatus;
    }

    public void PrzypiszKierowce(Kierowca kierowca)
    {
        PrzypisanyKierowca = kierowca;
    }

    public bool CzyKierowcaMozeProwadzic(Kierowca kierowca, out string powod)
    {
        if (!kierowca.czyMaWaznaKategorie(WymaganaKategoriaPrawaJazdy))
        {
            powod = $"Kierowca nie ma ważnej kategorii {WymaganaKategoriaPrawaJazdy}.";
            return false;
        }

        powod = "OK";
        return true;
    }

    public void UsunPrzypisanieKierowcy()
    {
        PrzypisanyKierowca = null;
    }

    public void ZglosUsterke(string opis, bool krytyczna)
    {
        HistoriaSerwisow.Add(new SerwisPojazdu(opis, krytyczna, TypWpisuSerwisowego.Usterka));
        if (krytyczna)
        {
            Status = StatusPojazdu.Serwis;
        }
    }

    public void DodajPrzeglad(string opis, int przebiegPrzegladu)
    {
        if (przebiegPrzegladu > AktualnyPrzebieg)
        {
            throw new ArgumentException("Przebieg przeglądu nie może być większy niż aktualny przebieg.");
        }

        HistoriaSerwisow.Add(new SerwisPojazdu(opis, false, TypWpisuSerwisowego.Przeglad));
        PrzebiegOstatniegoPrzegladu = przebiegPrzegladu;
    }

    public void DodajWymianeCzesci(string opis)
    {
        HistoriaSerwisow.Add(new SerwisPojazdu(opis, false, TypWpisuSerwisowego.WymianaCzesci));
    }

    public void WykonajNaprawe()
    {
        foreach (var usterka in HistoriaSerwisow.Where(u => !u.czyRozwiazana))
        {
            usterka.napraw();
        }

        HistoriaSerwisow.Add(new SerwisPojazdu("Wykonano naprawę pojazdu.", false, TypWpisuSerwisowego.Naprawa));
        PrzebiegOstatniegoPrzegladu = AktualnyPrzebieg;
        Status = StatusPojazdu.Dostepny;
    }

    public bool czyZdatnyDoJazdy(out string powod)
    {
        powod = "";
        if (Status != StatusPojazdu.Dostepny)
        {
            powod = $"Status pojazdu to: {Status}";
            return false;
        }
        if (waznoscPolisyOC < DateTime.Now)
        {
            powod = "Polisa OC jest niewazna.";
            return false;
        }
        if (WaznoscBadaniaTechnicznego < DateTime.Now)
        {
            powod = "Przeglad techniczny wygasl.";
            return false;
        }

        if (AktualnyPrzebieg > PrzebiegOstatniegoPrzegladu + CoIlePrzeglad)
        {
            powod = $"Przekroczony limit kilometrow od serwisu! (Limit: {CoIlePrzeglad} km)";
            return false;
        }
        bool maKrytyczneUsterki = HistoriaSerwisow.Any(u => u.czyKrytyczna && !u.czyRozwiazana);
        if (maKrytyczneUsterki)
        {
            powod = "Pojazd ma nienaprawione usterki krytyczne!";
            return false;
        }

        powod = "Wszystko OK.";
        return true;
    }
}
