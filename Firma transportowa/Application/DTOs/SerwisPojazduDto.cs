namespace FirmaTransportowa.Application.DTOs;

public sealed record SerwisPojazduDto(
    string Opis,
    DateTime DataZgloszenia,
    bool CzyKrytyczna,
    bool CzyRozwiazana
);

