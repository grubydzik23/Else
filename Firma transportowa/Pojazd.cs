namespace FirmaTransportowa;

public class Pojazd
{
    public string Marka { get; set; }
    public string Model { get; set; }
    public int RokProdukcji { get; set; }
    public DateTime WaznoscBadaniaTechnicznego { get; set; } 
    public StatusPojazdu Status { get; set; }
    public string Vin { get; set; }
    public int Przebieg {get; set;}
    public DateTime waznoscPolisyOC  { get; set; }

    public bool czyZdatnyDoJazdy()
    {
        if (Status != StatusPojazdu.Dostepny)
        {
            return false;
        }
        if (waznoscPolisyOC < DateTime.Now)
        {
            return false;
        }
        if (WaznoscBadaniaTechnicznego < DateTime.Now)
        {
            return false;
        }
        return true;
    }
}