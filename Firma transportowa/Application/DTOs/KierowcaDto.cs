namespace FirmaTransportowa.Application.DTOs;

public sealed record KierowcaDto(
    string Imie,
    string Nazwisko,
    IReadOnlyList<UprawnienieDto> KategoriaPrawaJazdy
);

