namespace FirmaTransportowa.Application.DTOs;

public sealed record PojazdDto(
    string Marka,
    string Model,
    int RokProdukcji,
    StatusPojazduDto Status,
    string Vin,
    string WymaganaKategoriaPrawaJazdy,
    string? PrzypisanyKierowca,
    int AktualnyPrzebieg,
    int PrzebiegOstatniegoPrzegladu,
    int CoIlePrzeglad,
    DateTime WaznoscPolisyOC,
    DateTime WaznoscBadaniaTechnicznego,
    IReadOnlyList<SerwisPojazduDto> HistoriaSerwisow
);

