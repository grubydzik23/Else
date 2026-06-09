using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Application.Interfaces;

public interface IZlecenieTransportoweRepository
{
    void Add(ZlecenieTransportowe zlecenie);
    IReadOnlyList<ZlecenieTransportowe> GetAll();
    ZlecenieTransportowe? GetById(Guid id);
}

