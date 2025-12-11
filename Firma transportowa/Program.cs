using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace FirmaTransportowa
{
    enum StatusPojazdu
    {
        Dostepny,
        WTrasie,
        Serwis
    }
    class Pojazd
    {
        public string Marka { get; set; }
        public int RokProdukcji { get; set; }
        public DateTime WaznoscBadaniaTechnicznego { get; set; } 
        public StatusPojazdu Status { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            private void ZarzadzaniePojazdami()
                while(true)
                    Console.WriteLine("1. Rejestracja pojazdu");
            {
                Console.WriteLine("1. Zarzadanie pojazdami");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ZarzadaniePojazdami();
                        break;
                }
            }
        }
    }
}