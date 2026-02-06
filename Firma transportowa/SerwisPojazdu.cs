namespace FirmaTransportowa;

public class SerwisPojazdu
{
    public string opis { get; private set; }
    public DateTime dataZgloszenia { get; private set; }
    public bool czyKrytyczna { get; private set; }
    public bool czyRozwiazana { get; private set; }

    public SerwisPojazdu(string Opis, bool CzyKrytyczna)
    {
        opis = Opis;
        czyKrytyczna = CzyKrytyczna;
        dataZgloszenia = DateTime.Now;
        czyRozwiazana = false;
    }
    
    public void napraw()
    {
        czyRozwiazana = true;
    }
}