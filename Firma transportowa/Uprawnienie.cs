namespace FirmaTransportowa;

public class Uprawnienie
{
    public string Kategoria { get; set; }
    public DateTime dataWaznosci { get; set; }

    public Uprawnienie(string kat, DateTime data)
    {
        Kategoria = kat.ToUpper();
        dataWaznosci = data;
    }
}