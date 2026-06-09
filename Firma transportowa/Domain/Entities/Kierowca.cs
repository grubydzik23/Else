using FirmaTransportowa.Domain.ValueObjects;

namespace FirmaTransportowa.Domain.Entities;

public class Kierowca
{
    public string Imie { get; private set; }
    public string Nazwisko { get; private set; }

    public List<Uprawnienie> KategoriaPrawaJazdy { get; } = new List<Uprawnienie>();

    public Kierowca(string imie, string nazwisko)
    {
        Imie = imie;
        Nazwisko = nazwisko;
    }

    public void DodajUprawnienie(string kategoria, DateTime dataWaznosci)
    {
        var upr = new Uprawnienie(kategoria, dataWaznosci);
        KategoriaPrawaJazdy.Add(upr);
    }

    public bool czyMaWaznaKategorie(string wymaganaKategoria)
    {
        return KategoriaPrawaJazdy.Any(u =>
            u.Kategoria == wymaganaKategoria.ToUpper() && u.dataWaznosci > DateTime.Now);
    }
}
