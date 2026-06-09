using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Domain.Entities;
using FirmaTransportowa.Domain.Enums;

namespace FirmaTransportowa.Application.Services;

public sealed class TrasaAppService
{
    private readonly ITrasaRepository _trasaRepository;
    private readonly IPojazdRepository _pojazdRepository;
    private readonly IZlecenieTransportoweRepository _zlecenieRepository;

    public TrasaAppService(
        ITrasaRepository trasaRepository,
        IPojazdRepository pojazdRepository,
        IZlecenieTransportoweRepository zlecenieRepository)
    {
        _trasaRepository = trasaRepository;
        _pojazdRepository = pojazdRepository;
        _zlecenieRepository = zlecenieRepository;
    }

    public IReadOnlyList<Trasa> GetActiveAll() => _trasaRepository.GetActiveAll();

    public IReadOnlyList<Trasa> GetHistoryForDriver(string imieNazwisko) =>
        _trasaRepository.GetHistoryForDriver(imieNazwisko);

    public bool DriverHasActiveRoute(Kierowca kierowca) =>
        _trasaRepository.GetActiveForDriver($"{kierowca.Imie} {kierowca.Nazwisko}").Count > 0;

    public bool VehicleHasActiveRoute(Pojazd pojazd) =>
        _trasaRepository.GetActiveAll().Any(t => t.VinPojazdu == pojazd.Vin);

    public bool DriverHasActiveRouteWithVehicle(Kierowca kierowca, Pojazd pojazd)
    {
        var imieNazw = $"{kierowca.Imie} {kierowca.Nazwisko}";
        return _trasaRepository.GetActiveAll().Any(t => t.Kierowca == imieNazw && t.VinPojazdu == pojazd.Vin);
    }

    public OperationResult TryStartRoute(Kierowca kierowca, Pojazd pojazd, ZlecenieTransportowe zlecenie)
    {
        if (zlecenie.Status != StatusZleceniaTransportowego.Nowe)
            return OperationResult.Fail("Zlecenie nie jest w statusie 'Nowe' i nie moze byc rozpoczęte.");

        if (pojazd.PrzypisanyKierowca == null || pojazd.PrzypisanyKierowca != kierowca)
        {
            if (!pojazd.CzyKierowcaMozeProwadzic(kierowca, out string powodKat))
                return OperationResult.Fail(powodKat);
            pojazd.PrzypiszKierowce(kierowca);
        }

        if (!pojazd.czyZdatnyDoJazdy(out string powod))
            return OperationResult.Fail(powod);

        try
        {
            zlecenie.OznaczJakoWTrakcie();
        }
        catch (InvalidOperationException ex)
        {
            return OperationResult.Fail(ex.Message);
        }

        var trasa = new Trasa(
            zlecenie.Id,
            pojazd.Vin,
            $"{kierowca.Imie} {kierowca.Nazwisko}",
            zlecenie.StartMiejsce,
            zlecenie.CelMiejsce,
            zlecenie.Towar);
        _trasaRepository.Add(trasa);
        return OperationResult.Ok();
    }

    public void EndRoute(Trasa trasa, int km)
    {
        trasa.Zakoncz(km);
        var pojazd = _pojazdRepository.GetByVin(trasa.VinPojazdu);
        if (pojazd != null && km > 0)
            pojazd.UstawPrzebieg(pojazd.AktualnyPrzebieg + km);

        var zlecenie = _zlecenieRepository.GetById(trasa.ZlecenieId);
        if (zlecenie != null && zlecenie.Status != StatusZleceniaTransportowego.Anulowane)
        {
            try
            {
                zlecenie.OznaczJakoZakonczone();
            }
            catch
            {
                // ignore
            }
        }
    }
    
}