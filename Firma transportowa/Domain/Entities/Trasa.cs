namespace FirmaTransportowa.Domain.Entities;

public class Trasa
{
    public Guid ZlecenieId { get; }
    public string VinPojazdu { get; }
    public string Kierowca { get; }
    public string StartMiejsce { get; }
    public string CelMiejsce { get; }
    public string Towar { get; }
    public DateTime Start { get; }
    public DateTime? Koniec { get; private set; }
    
    public int? PrzejechaneKm { get; private set; }
    public decimal? SrednieSpalanie { get; private set; }
    public decimal? ZatankowaneLitry { get; private set; }
    public decimal? CenaZaLitr { get; private set; }
    public decimal? KosztyDodatkowe { get; private set; }
    public decimal? KosztyCalkowite {get; private set; }

    public string Opis => $"{StartMiejsce} -> {CelMiejsce}";

    public Trasa(Guid zlecenieId, string vinPojazdu, string kierowca, string startMiejsce, string celMiejsce, string towar)
    {
        ZlecenieId = zlecenieId;
        VinPojazdu = vinPojazdu;
        Kierowca = kierowca;
        StartMiejsce = startMiejsce;
        CelMiejsce = celMiejsce;
        Towar = towar;
        Start = DateTime.Now;
    }

    public void Zakoncz(int przejechaneKm, decimal zatankowaneLitry, decimal cenaZaLitr, decimal kosztyDodatkowe)
    {
        PrzejechaneKm = przejechaneKm;
        Koniec = DateTime.Now;
        
        ZatankowaneLitry = zatankowaneLitry;
        CenaZaLitr = cenaZaLitr;
        KosztyDodatkowe = kosztyDodatkowe;
        
        KosztyCalkowite = (cenaZaLitr*zatankowaneLitry) + kosztyDodatkowe ;
        if (przejechaneKm > 0)
        {
            SrednieSpalanie = (zatankowaneLitry / przejechaneKm) * 100m;
        }
        else
        {
            SrednieSpalanie = 0m;
        }
    }
}
