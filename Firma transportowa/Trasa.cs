namespace FirmaTransportowa;

public class Trasa
{
    public string VinPojazdu { get; }
    public string Kierowca { get; }
    public string Opis { get; }
    public DateTime Start { get; }
    public DateTime? Koniec { get; private set; }
    public int? PrzejechaneKm { get; private set; }

    public Trasa(string vinPojazdu, string kierowca, string opis)
    {
        VinPojazdu = vinPojazdu;
        Kierowca = kierowca;
        Opis = opis;
        Start = DateTime.Now;
    }

    public void Zakoncz(int przejechaneKm)
    {
        PrzejechaneKm = przejechaneKm;
        Koniec = DateTime.Now;
    }
}

