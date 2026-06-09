using FirmaTransportowa.Application.Interfaces;
using FirmaTransportowa.Application.Services;
using FirmaTransportowa.Infrastructure.Repositories;
using FirmaTransportowa.Presentation.ConsoleUI;

namespace FirmaTransportowa;

/// <summary>
/// Punkt wejścia: kompozycja zależności (DI ręczne) i uruchomienie warstwy prezentacji.
/// </summary>
internal class Program
{
    public static void Main(string[] args)
    {
        IPojazdRepository pojazdRepository = new InMemoryPojazdRepository();
        IKierowcaRepository kierowcaRepository = new InMemoryKierowcaRepository();
        ITrasaRepository trasaRepository = new InMemoryTrasaRepository();
        IZlecenieTransportoweRepository zlecenieRepository = new InMemoryZlecenieTransportoweRepository();

        var pojazdAppService = new PojazdAppService(pojazdRepository);
        var kierowcaAppService = new KierowcaAppService(kierowcaRepository, pojazdRepository, trasaRepository);
        var zlecenieAppService = new ZlecenieTransportoweAppService(zlecenieRepository);
        var trasaAppService = new TrasaAppService(trasaRepository, pojazdRepository, zlecenieRepository);

        var app = new AppConsole(pojazdAppService, kierowcaAppService, trasaAppService, zlecenieAppService);
        app.Run();
        
    }
}
