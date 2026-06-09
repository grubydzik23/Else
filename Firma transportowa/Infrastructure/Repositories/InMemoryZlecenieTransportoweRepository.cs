using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Infrastructure.Repositories;

public sealed class InMemoryZlecenieTransportoweRepository : IZlecenieTransportoweRepository
{
    private readonly List<ZlecenieTransportowe> _zlecenia = new();

    public void Add(ZlecenieTransportowe zlecenie) => _zlecenia.Add(zlecenie);

    public IReadOnlyList<ZlecenieTransportowe> GetAll() => _zlecenia;

    public ZlecenieTransportowe? GetById(Guid id) => _zlecenia.FirstOrDefault(z => z.Id == id);
}

