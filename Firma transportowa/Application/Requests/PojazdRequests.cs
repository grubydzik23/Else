namespace FirmaTransportowa.Application.Requests;

public sealed record CreatePojazdRequest(
    string Marka,
    string Model,
    string Vin,
    int RokProdukcji,
    int AktualnyPrzebieg,
    DateTime WaznoscBadaniaTechnicznego,
    DateTime WaznoscPolisyOC
);

public sealed record UpdatePrzebiegRequest(int NowyPrzebieg);

public sealed record UpdateBadanieTechniczneRequest(DateTime NowaWaznoscBadaniaTechnicznego);

public sealed record UpdateOcRequest(DateTime NowaWaznoscPolisyOC);

public sealed record ZglosUsterkeRequest(
    string Opis,
    bool CzyKrytyczna
);

