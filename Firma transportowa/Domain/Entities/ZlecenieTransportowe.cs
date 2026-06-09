using FirmaTransportowa.Domain.Enums;

namespace FirmaTransportowa.Domain.Entities;

public sealed class ZlecenieTransportowe
{
    public Guid Id { get; }
    public string StartMiejsce { get; }
    public string CelMiejsce { get; }
    public string Towar { get; }
    public DateTime DataUtworzenia { get; }
    public StatusZleceniaTransportowego Status { get; private set; }

    public ZlecenieTransportowe(string startMiejsce, string celMiejsce, string towar)
    {
        if (string.IsNullOrWhiteSpace(startMiejsce))
            throw new ArgumentException("Start zlecenia jest wymagany.");
        if (string.IsNullOrWhiteSpace(celMiejsce))
            throw new ArgumentException("Cel zlecenia jest wymagany.");
        if (string.IsNullOrWhiteSpace(towar))
            throw new ArgumentException("Towar jest wymagany.");

        Id = Guid.NewGuid();
        StartMiejsce = startMiejsce.Trim();
        CelMiejsce = celMiejsce.Trim();
        Towar = towar.Trim();
        DataUtworzenia = DateTime.Now;
        Status = StatusZleceniaTransportowego.Nowe;
    }

    public void OznaczJakoWTrakcie()
    {
        if (Status == StatusZleceniaTransportowego.Anulowane)
            throw new InvalidOperationException("Nie można rozpocząć anulowanego zlecenia.");
        if (Status == StatusZleceniaTransportowego.Zakonczone)
            throw new InvalidOperationException("Nie można rozpocząć zakończonego zlecenia.");
        Status = StatusZleceniaTransportowego.WTrakcie;
    }

    public void OznaczJakoZakonczone()
    {
        if (Status == StatusZleceniaTransportowego.Anulowane)
            throw new InvalidOperationException("Nie można zakończyć anulowanego zlecenia.");
        Status = StatusZleceniaTransportowego.Zakonczone;
    }

    public void Anuluj()
    {
        if (Status == StatusZleceniaTransportowego.Zakonczone)
            throw new InvalidOperationException("Nie można anulować zakończonego zlecenia.");
        Status = StatusZleceniaTransportowego.Anulowane;
    }
}

