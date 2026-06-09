namespace FirmaTransportowa.Application.DTOs;

public sealed record SerwisPojazduDto(
    string Opis,
    DateTime DataZgloszenia,
    TypWpisuSerwisowegoDto TypWpisu,
    bool CzyKrytyczna,
    bool CzyRozwiazana
);

