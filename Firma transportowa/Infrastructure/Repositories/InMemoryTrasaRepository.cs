using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Infrastructure.Repositories;

public sealed class InMemoryTrasaRepository : ITrasaRepository
{
    private readonly List<Trasa> _trasy = new();

    public void Add(Trasa trasa) => _trasy.Add(trasa);

    public IReadOnlyList<Trasa> GetAll() => _trasy;

    public IReadOnlyList<Trasa> GetActiveForDriver(string imieNazwisko) =>
        _trasy.Where(t => t.Kierowca == imieNazwisko && t.Koniec == null).ToList();

    public IReadOnlyList<Trasa> GetActiveAll() =>
        _trasy.Where(t => t.Koniec == null).ToList();

    public IReadOnlyList<Trasa> GetHistoryForDriver(string imieNazwisko) =>
        _trasy.Where(t => t.Kierowca == imieNazwisko).OrderByDescending(t => t.Start).ToList();
}
