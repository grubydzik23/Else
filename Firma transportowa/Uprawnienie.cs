namespace FirmaTransportowa;

public class Uprawnienie
{
    public string Kategoria { get; private set; }
    public DateTime dataWaznosci { get; private set; }

    public Uprawnienie(string kat, DateTime data)
    {
        Kategoria = kat.ToUpper();
        dataWaznosci = data;
    }

    public void ZmienDateWaznosci(DateTime nowaData)
    {
        dataWaznosci = nowaData;
    }
}