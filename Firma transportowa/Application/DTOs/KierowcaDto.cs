namespace FirmaTransportowa.Application.DTOs;

public sealed record KierowcaDto(
    string Imie,
    string Nazwisko,
    string? PrzypisanyPojazdVin,
    IReadOnlyList<UprawnienieDto> KategoriaPrawaJazdy
);

