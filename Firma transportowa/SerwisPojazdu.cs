namespace FirmaTransportowa;

public class SerwisPojazdu
{
    public string opis { get; set; }
    public DateTime dataZgloszenia { get; set; }
    public bool czyKrytyczna { get; set; }
    public bool czyRozwiazana { get; set; }

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