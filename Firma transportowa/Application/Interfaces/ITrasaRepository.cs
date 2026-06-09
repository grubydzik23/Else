using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Application.Interfaces;

public interface ITrasaRepository
{
    void Add(Trasa trasa);
    IReadOnlyList<Trasa> GetAll();
    IReadOnlyList<Trasa> GetActiveForDriver(string imieNazwisko);
    IReadOnlyList<Trasa> GetActiveAll();
    IReadOnlyList<Trasa> GetHistoryForDriver(string imieNazwisko);
}
