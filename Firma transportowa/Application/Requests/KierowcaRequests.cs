namespace FirmaTransportowa.Application.Requests;

public sealed record CreateKierowcaRequest(
    string Imie,
    string Nazwisko
);

public sealed record AddUprawnienieRequest(
    string Kategoria,
    DateTime DataWaznosci
);

public sealed record UpdateUprawnienieRequest(
    string Kategoria,
    DateTime NowaDataWaznosci
);

