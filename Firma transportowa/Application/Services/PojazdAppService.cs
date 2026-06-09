using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Application.Services;

public sealed class PojazdAppService
{
    private readonly IPojazdRepository _pojazdRepository;

    public PojazdAppService(IPojazdRepository pojazdRepository)
    {
        _pojazdRepository = pojazdRepository;
    }

    public IReadOnlyList<Pojazd> GetAll() => _pojazdRepository.GetAll();

    public OperationResult RegisterVehicle(
        string marka,
        string model,
        string vin,
        int rok,
        int przebieg,
        int przebiegPrzegladu,
        string wymaganaKategoria,
        DateTime przeglad,
        DateTime oc)
    {
        try
        {
            var pojazd = new Pojazd(
                marka,
                model,
                vin,
                rok,
                przebieg,
                przebiegPrzegladu,
                wymaganaKategoria,
                przeglad,
                oc);
            _pojazdRepository.Add(pojazd);
            return OperationResult.Ok();
        }
        catch (ArgumentException ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public OperationResult TryAssignDriver(Pojazd pojazd, Kierowca kierowca)
    {
        if (!pojazd.CzyKierowcaMozeProwadzic(kierowca, out string powod))
            return OperationResult.Fail(powod);

        pojazd.PrzypiszKierowce(kierowca);
        return OperationResult.Ok();
    }
}
