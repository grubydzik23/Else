namespace FirmaTransportowa;

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
    
    public DateTime waznoscPolisyOC  { get; private set; }
    public DateTime WaznoscBadaniaTechnicznego { get; private set; } 
    
    public List<SerwisPojazdu> HistoriaSerwisow { get; } = new List<SerwisPojazdu>();

    public Pojazd(
        string marka,
        string model,
        string vin,
        int rokProdukcji,
        int aktualnyPrzebieg,
        DateTime waznoscBadaniaTechnicznego,
        DateTime waznoscPolisyOc)
    {
        Marka = marka;
        Model = model;
        Vin = vin;
        RokProdukcji = rokProdukcji;
        AktualnyPrzebieg = aktualnyPrzebieg;
        PrzebiegOstatniegoPrzegladu = aktualnyPrzebieg;
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

    public void ZglosUsterke(string opis, bool krytyczna)
    {
        HistoriaSerwisow.Add(new SerwisPojazdu(opis, krytyczna));
        if (krytyczna)
        {
            Status = StatusPojazdu.Serwis;
        }
    }
    public void WykonajNaprawe()
    {
        foreach (var usterka in HistoriaSerwisow.Where(u => !u.czyRozwiazana))
        {
            usterka.napraw();
        }

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