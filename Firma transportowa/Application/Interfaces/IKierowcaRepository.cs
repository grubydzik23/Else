using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Application.Interfaces;

public interface IKierowcaRepository
{
    IReadOnlyList<Kierowca> GetAll();
    void Add(Kierowca kierowca);
    void Remove(Kierowca kierowca);
}
