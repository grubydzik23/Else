using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Infrastructure.Repositories;

public sealed class InMemoryPojazdRepository : IPojazdRepository
{
    private readonly List<Pojazd> _pojazdy = new();

    public IReadOnlyList<Pojazd> GetAll() => _pojazdy;

    public void Add(Pojazd pojazd) => _pojazdy.Add(pojazd);

    public Pojazd? GetByVin(string vin) =>
        _pojazdy.FirstOrDefault(p => string.Equals(p.Vin, vin, StringComparison.OrdinalIgnoreCase));

    public Pojazd? FindAssignedTo(Kierowca kierowca) =>
        _pojazdy.FirstOrDefault(p => p.PrzypisanyKierowca == kierowca);
}
