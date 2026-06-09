using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;

namespace FirmaTransportowa.Application.Services;

public sealed class KierowcaAppService
{
    private readonly IKierowcaRepository _kierowcaRepository;
    private readonly IPojazdRepository _pojazdRepository;
    private readonly ITrasaRepository _trasaRepository;

    public KierowcaAppService(
        IKierowcaRepository kierowcaRepository,
        IPojazdRepository pojazdRepository,
        ITrasaRepository trasaRepository)
    {
        _kierowcaRepository = kierowcaRepository;
        _pojazdRepository = pojazdRepository;
        _trasaRepository = trasaRepository;
    }

    public IReadOnlyList<Kierowca> GetAll() => _kierowcaRepository.GetAll();

    public void RegisterDriver(Kierowca kierowca) => _kierowcaRepository.Add(kierowca);

    public OperationResult TryRemoveDriver(Kierowca kierowca)
    {
        var auto = _pojazdRepository.FindAssignedTo(kierowca);
        if (auto != null)
            return OperationResult.Fail("Auto jest przypisane do kierowcy. Najpierw usun przypisanie w menu.");

        var imieNazwisko = $"{kierowca.Imie} {kierowca.Nazwisko}";
        if (_trasaRepository.GetActiveForDriver(imieNazwisko).Count > 0)
            return OperationResult.Fail("Ten kierowca jest aktualnie w trasie! Zakończ trasę przed usunięciem.");

        _kierowcaRepository.Remove(kierowca);
        return OperationResult.Ok();
    }
    
    public OperationResult TryAssignVehicleToDriver(Kierowca kierowca, Pojazd pojazd)
    {
        if (!pojazd.CzyKierowcaMozeProwadzic(kierowca, out string powod))
            return OperationResult.Fail(powod);

        pojazd.PrzypiszKierowce(kierowca);
        return OperationResult.Ok();
    }
}
