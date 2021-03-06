﻿using System;
using System.Collections.Generic;

namespace Distribuicao.DadosAgrupados
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] Notas;
            if (args.Length == 0)
               Notas = Console.ReadLine().ToString().Split(',');
            else
               Notas = args[0].ToString().Split(',');
            
			List<float> NotasF = new List<float>();
			foreach(string nota in Notas){
				NotasF.Add(float.Parse(nota));
			}
            
            TabelaDistribuicao tabela = new TabelaDistribuicao(NotasF);
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
