namespace FirmaTransportowa;

public class Pojazd
{
    public string Marka { get; set; }
    public string Model { get; set; }
    public int RokProdukcji { get; set; }
    public StatusPojazdu Status { get; set; }
    public string Vin { get; set; }
    
    public int AktualnyPrzebieg {get; set;}
    public int PrzebiegOstatniegoPrzegladu { get; set; }
    public int CoIlePrzeglad { get; set; } = 15000;
    
    public DateTime waznoscPolisyOC  { get; set; }
    public DateTime WaznoscBadaniaTechnicznego { get; set; } 
    
    public List<SerwisPojazdu> HistoriaSerwisow { get; set; } = new List<SerwisPojazdu>();

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
            powod = $"Przekroczony limin kilometrow od serwisu! (Limit: {CoIlePrzeglad} km)";
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