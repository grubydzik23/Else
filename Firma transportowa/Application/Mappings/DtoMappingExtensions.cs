using System;
using System.Linq;
using FirmaTransportowa.Application.DTOs;
using FirmaTransportowa.Domain.Entities;
using FirmaTransportowa.Domain.Enums;
using FirmaTransportowa.Domain.ValueObjects;

namespace FirmaTransportowa.Application.Mappings;

public static class DtoMappingExtensions
{
    public static StatusPojazduDto ToDto(this StatusPojazdu status)
        => status switch
        {
            StatusPojazdu.Dostepny => StatusPojazduDto.Dostepny,
            StatusPojazdu.WTrasie => StatusPojazduDto.WTrasie,
            StatusPojazdu.Serwis => StatusPojazduDto.Serwis,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Nieznany status pojazdu.")
        };

    public static TypWpisuSerwisowegoDto ToDto(this TypWpisuSerwisowego typ)
        => typ switch
        {
            TypWpisuSerwisowego.Usterka => TypWpisuSerwisowegoDto.Usterka,
            TypWpisuSerwisowego.Naprawa => TypWpisuSerwisowegoDto.Naprawa,
            TypWpisuSerwisowego.Przeglad => TypWpisuSerwisowegoDto.Przeglad,
            TypWpisuSerwisowego.WymianaCzesci => TypWpisuSerwisowegoDto.WymianaCzesci,
            _ => throw new ArgumentOutOfRangeException(nameof(typ), typ, "Nieznany typ wpisu serwisowego.")
        };

    public static UprawnienieDto ToDto(this Uprawnienie u)
        => new(u.Kategoria, u.dataWaznosci);

    public static SerwisPojazduDto ToDto(this SerwisPojazdu s)
        => new(s.opis, s.dataZgloszenia, s.TypWpisu.ToDto(), s.czyKrytyczna, s.czyRozwiazana);

    public static TrasaDto ToDto(this Trasa t)
        => new(t.VinPojazdu, t.Kierowca, t.Opis, t.Towar, t.Start, t.Koniec, t.PrzejechaneKm, t.SrednieSpalanie, t.ZatankowaneLitry, t.CenaZaLitr, t.KosztyDodatkowe, t.KosztyCalkowite);

    public static KierowcaDto ToDto(this Kierowca k, string? przypisanyPojazdVin = null)
        => new(
            k.Imie,
            k.Nazwisko,
            przypisanyPojazdVin,
            k.KategoriaPrawaJazdy.Select(ToDto).ToList()
        );

    public static PojazdDto ToDto(this Pojazd p)
        => new(
            p.Marka,
            p.Model,
            p.RokProdukcji,
            p.Status.ToDto(),
            p.Vin,
            p.WymaganaKategoriaPrawaJazdy,
            p.PrzypisanyKierowca == null ? null : $"{p.PrzypisanyKierowca.Imie} {p.PrzypisanyKierowca.Nazwisko}",
            p.AktualnyPrzebieg,
            p.PrzebiegOstatniegoPrzegladu,
            p.CoIlePrzeglad,
            p.waznoscPolisyOC,
            p.WaznoscBadaniaTechnicznego,
            p.HistoriaSerwisow.Select(ToDto).ToList()
        );
}
