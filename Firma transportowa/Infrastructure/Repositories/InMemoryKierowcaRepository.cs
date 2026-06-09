using System.Data;
using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Infrastructure.Repositories;

public sealed class InMemoryKierowcaRepository : IKierowcaRepository
{
    private readonly List<Kierowca> _kierowcy = new();

    public IReadOnlyList<Kierowca> GetAll() => _kierowcy;

    public void Add(Kierowca kierowca) => _kierowcy.Add(kierowca);

    public void Remove(Kierowca kierowca) => _kierowcy.Remove(kierowca);
}
