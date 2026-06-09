namespace FirmaTransportowa.Application.DTOs;

public sealed record TrasaDto(
    string VinPojazdu,
    string Kierowca,
    string Opis,
    string Towar,
    DateTime Start,
    DateTime? Koniec,
    int? PrzejechaneKm,
    decimal? SrednieSpalanie,
    decimal? ZatankowaneLitry,
    decimal? CenaZaLitr,
    decimal? KosztyDodatkowe,
    decimal? KosztyCalkowite
);
