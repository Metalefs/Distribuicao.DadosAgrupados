using System;
using System.Collections.Generic;

namespace Distribuicao.DadosAgrupados
{
    class Program
    {
        static void Main(string[] args)
        {
            List<float> Notas = new List<float>()
            {
               18,18,19,19,20,20,21,21,21,21,
               22,23,23,23,24,24,24,26,26,26,
               27,27,28,29,31,31,31,31,31,31,
               31,32,32,32,33,33,34,35,35,35,
               37,37,38,39,41,42,43,44,47,50
            };
            TabelaDistribuicao tabela = new TabelaDistribuicao(Notas);
            Console.ReadKey();
        }
    }

    public static class StringExtention
    {
        public static string Normalized(this string value)
        {
            return value.PadLeft(15, ' ');
        }
    }
}
