using FirmaTransportowa.Domain.Enums;

namespace FirmaTransportowa.Domain.Entities;

public class SerwisPojazdu
{
    public string opis { get; private set; }
    public DateTime dataZgloszenia { get; private set; }
    public bool czyKrytyczna { get; private set; }
    public bool czyRozwiazana { get; private set; }
    public TypWpisuSerwisowego TypWpisu { get; private set; }

    public SerwisPojazdu(string Opis, bool CzyKrytyczna, TypWpisuSerwisowego typWpisu = TypWpisuSerwisowego.Usterka)
    {
        opis = Opis;
        czyKrytyczna = CzyKrytyczna;
        TypWpisu = typWpisu;
        dataZgloszenia = DateTime.Now;
        czyRozwiazana = typWpisu != TypWpisuSerwisowego.Usterka;
    }

    public void napraw()
    {
        czyRozwiazana = true;
    }
}
