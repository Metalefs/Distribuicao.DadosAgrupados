﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Distribuicao.DadosAgrupados
{
    public class TabelaDistribuicao : ITabelaDistribuicao
    {
        public List<KeyValuePair<string, int>> NomesColunas = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("Dados", 15),
            new KeyValuePair<string, int>("xi", 10),
            new KeyValuePair<string, int>("fi", 10),
            new KeyValuePair<string, int>("Fi", 10),
            new KeyValuePair<string, int>("fr", 10),
            new KeyValuePair<string, int>("Fr", 10),
        };

        private List<string> Linhas = new List<string>();
        private List<float> FrequenciasSimples = new List<float>();
        private List<float> FrequenciasRelativas = new List<float>();

        public List<float> Valores { get; set; }
        public float Amplitude { get; private set; }
        public int NumeroDeElementos { get; private set; }
        public float QuantidadeIntervalos { get; private set; }
        public float Intervalo { get; private set; }
        private float ValorMinimo;
        private float ValorMaximo;

        public TabelaDistribuicao(List<float> Valores)
        {
            this.Valores = Valores;
            NumeroDeElementos = Valores.Count;
            ValorMinimo = CalcularValorMinimo(Valores);
            ValorMaximo = CalcularValorMaximo(Valores);

            Amplitude = CalcularAmplitude(ValorMinimo, ValorMaximo);
            QuantidadeIntervalos = CalcularQuantidadeIntervalos(NumeroDeElementos);
            Intervalo = CalcularTamanhoIntervalo(Amplitude, QuantidadeIntervalos);
            GerarTabela();
        }

        public float CalcularValorMinimo(List<float> Valores)
        {
            ValorMinimo = Valores.Min();
            return ValorMinimo;
        }

        public float CalcularValorMaximo(List<float> Valores)
        {
            ValorMaximo = Valores.Max();
            return ValorMaximo;
        }

        public float CalcularAmplitude(float ValorMinimo, float ValorMaximo)
        {
            Amplitude = ValorMaximo - ValorMinimo;
            return Amplitude;
        }

        public float CalcularQuantidadeIntervalos(int NumeroDeElementos)
        {
            switch (NumeroDeElementos)
            {
                case 5:
                    QuantidadeIntervalos = 2;
                    break;
                case 10:
                    QuantidadeIntervalos = 4;
                    break;
                case 25:
                    QuantidadeIntervalos = 6;
                    break;
                case 50:
                    QuantidadeIntervalos = 8;
                    break;
                case 100:
                    QuantidadeIntervalos = 10;
                    break;
                default:
                    QuantidadeIntervalos = (float)Math.Sqrt(NumeroDeElementos);
                    break;
            }
            return QuantidadeIntervalos;
        }

        public float CalcularTamanhoIntervalo(float Amplitude, float QuantidadeIntervalos)
        {
            Intervalo = Amplitude / QuantidadeIntervalos;
            return Intervalo;
        }

        public void GerarTabela()
        {
            float xi, fi, Fi, fr, Fr = 0;
            float Abertura = ValorMinimo;
            List<string> Calculos = new List<string>();
            float xifi = 0;
            for (int i = 0; i <= QuantidadeIntervalos; i++)
            {
                float Fim = Abertura + Intervalo;
                string variavel = $"{Abertura.ToString("0.00")}|--{Fim.ToString("0.00")}";

                xi = CalcularMediaXI(Abertura, Fim);
                fi = CalcularFrequenciaSimples(Abertura, Fim);
                FrequenciasSimples.Add(fi);

                Fi = CalcularFrequenciaSimplesAcumulada(i, fi);
                fr = CalcularFrequenciaRelativa(i);
                FrequenciasRelativas.Add(fr);
                Calculos.Add($"fr {i} = {FrequenciasSimples[i]} / {NumeroDeElementos} * 100 = {(FrequenciasSimples[i] / NumeroDeElementos) * 100} \n");

                Fr = CalcularFrequenciaRelativaAcumulada(i, fr);

                Linhas.Add(GerarLinha(variavel, xi, fi, Fi, fr, Fr));

                Abertura = Fim;

                xifi += xi +fi;
            }
            Linhas.Add(GerarLinha("", 0f, FrequenciasSimples.Sum(), 0f, FrequenciasRelativas.Sum(), 0f));
            Linhas.Add($"Moda: {CalcularModa()}");
            Linhas.AddRange(Calculos);

            SalvarResultado(Linhas);
        }

        

        private float CalcularModa()
        {
            float Resultado = Valores.GroupBy(i => i).OrderByDescending(grp => grp.Count())
            .Select(grp => grp.Key).First();

            //if (Resultado >= 0 && Resultado <= 4)
            //{
            //    switch (Resultado)
            //    {
            //        case 0:
            //            Classificacao = ClassificacaoModa.Amodal;
            //            break;
            //        case 1:
            //            Classificacao = ClassificacaoModa.Unimodal;
            //            break;
            //        case 2:
            //            Classificacao = ClassificacaoModa.Bimodal;
            //            break;
            //        case 3:
            //            Classificacao = ClassificacaoModa.Trimodal;
            //            break;
            //        case 4:
            //            Classificacao = ClassificacaoModa.Polimodal;
            //            break;
            //    }
            //}
            //else if (Resultado >= 4)
            //{
            //    Classificacao = ClassificacaoModa.Polimodal;
            //}
            return Resultado;
        }


        private void SalvarResultado(List<string> Linhas)
        {
            using (TextWriter tw = new StreamWriter("Resultado.txt", true))
            {
                tw.WriteLine(GerarCabecalho());
                foreach (string linha in Linhas)
                {
                    tw.Write(linha);
                }
                tw.WriteLine($"\n Amplitude: {ValorMaximo} - {ValorMinimo} = {Amplitude}");
                tw.WriteLine($" Quantidade Intervalos: sqr.root {NumeroDeElementos} = {QuantidadeIntervalos}");
                tw.WriteLine($" Intervalo: {Amplitude} / {QuantidadeIntervalos} = {Intervalo}");
                tw.WriteLine($" Média: {Valores.Sum()} / {NumeroDeElementos} = {Valores.Average()}");
                tw.WriteLine($" Mediana = {Valores[(int)Valores.Count / 2]}");               
            }
        }

        // private void SalvarResultadoHTML(List<string> Linha)
        //{
        //
        //}

        private string GerarCabecalho()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, int> KV in NomesColunas)
            {
                if (KV.Key == "Dados")
                {
                    sb.Append($"{KV.Key.Normalized(),15}");
                }
                else
                {
                    sb.Append($"{KV.Key.Normalized(),10}");
                }
            }
            return sb.ToString();
        }

        private string GerarLinha(string variavel, float xi, float fi, float Fi, float fr, float Fr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(
                $"{variavel,15}" +
                $"{PadronizarLinha(xi),10}" +
                $"{PadronizarLinha(fi),10}" +
                $"{PadronizarLinha(Fi),10}" +
                $"{PadronizarLinha(fr),10}" +
                $"{PadronizarLinha(Fr),10}"
            );
            return sb.ToString();
        }

        private string PadronizarLinha(float value)
        {
            return value.ToString("0.00");
        }

        private float CalcularMediaXI(float Abertura, float Fim)
        {
            return (Abertura + Fim) / 2;
        }

        public float CalcularFrequenciaSimples(float Abertura, float Fim)
        {
            return Valores.Where(x => x >= Abertura && x < Fim).Count();
        }

        public float CalcularFrequenciaSimplesAcumulada(int pos, float fi)
        {
            if (pos > 0)
                return FrequenciasSimples.Sum();
            else
                return fi;
        }

        public float CalcularFrequenciaRelativa(int pos)
        {
            float Fr = FrequenciasSimples[pos] / NumeroDeElementos * 100;
            return Fr;
        }

        public float CalcularFrequenciaRelativaAcumulada(int pos, float fr)
        {
            if (pos > 0)
                return FrequenciasRelativas.Sum();
            else
                return fr;
        }

    }
}
