using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;
using FirmaTransportowa.Domain.Enums;

namespace FirmaTransportowa.Application.Services;

public sealed class ZlecenieTransportoweAppService
{
    private readonly IZlecenieTransportoweRepository _repo;

    public ZlecenieTransportoweAppService(IZlecenieTransportoweRepository repo)
    {
        _repo = repo;
    }

    public IReadOnlyList<ZlecenieTransportowe> GetAll() => _repo.GetAll();

    public IReadOnlyList<ZlecenieTransportowe> GetDoRealizacji() =>
        _repo.GetAll()
            .Where(z => z.Status is StatusZleceniaTransportowego.Nowe)
            .OrderByDescending(z => z.DataUtworzenia)
            .ToList();

    public OperationResult TryCreate(string start, string cel, string towar, out ZlecenieTransportowe? created)
    {
        created = null;
        try
        {
            created = new ZlecenieTransportowe(start, cel, towar);
            _repo.Add(created);
            return OperationResult.Ok();
        }
        catch (ArgumentException ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}

