using System;
using System.Linq;
using FirmaTransportowa;
using FirmaTransportowa.Application.DTOs;

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

    public static UprawnienieDto ToDto(this Uprawnienie u)
        => new(u.Kategoria, u.dataWaznosci);

    public static SerwisPojazduDto ToDto(this SerwisPojazdu s)
        => new(s.opis, s.dataZgloszenia, s.czyKrytyczna, s.czyRozwiazana);

    public static KierowcaDto ToDto(this Kierowca k)
        => new(
            k.Imie,
            k.Nazwisko,
            k.KategoriaPrawaJazdy.Select(ToDto).ToList()
        );

    public static PojazdDto ToDto(this Pojazd p)
        => new(
            p.Marka,
            p.Model,
            p.RokProdukcji,
            p.Status.ToDto(),
            p.Vin,
            p.AktualnyPrzebieg,
            p.PrzebiegOstatniegoPrzegladu,
            p.CoIlePrzeglad,
            p.waznoscPolisyOC,
            p.WaznoscBadaniaTechnicznego,
            p.HistoriaSerwisow.Select(ToDto).ToList()
        );
}

