namespace FirmaTransportowa;

public class Kierowca
{
    public string Imie { get; set; }
    public string Nazwisko { get; set; }

    public List<Uprawnienie> KategoriaPrawaJazdy { get; set; } = new List<Uprawnienie>();

    // public string trasa { get; set; }
    public bool czyMaWaznaKategorie(string wymaganaKategoria)
    {
        return KategoriaPrawaJazdy.Any(u =>
            u.Kategoria == wymaganaKategoria.ToUpper() && u.dataWaznosci > DateTime.Now);
    }
}