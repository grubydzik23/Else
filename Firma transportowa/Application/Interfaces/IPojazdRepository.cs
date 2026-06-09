using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Application.Interfaces;

public interface IPojazdRepository
{
    IReadOnlyList<Pojazd> GetAll();
    void Add(Pojazd pojazd);
    Pojazd? GetByVin(string vin);
    Pojazd? FindAssignedTo(Kierowca kierowca);
}
