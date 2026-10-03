using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    static void Main()
    {
        string json = @"{ 
          ""vendas"": [ 
            { ""vendedor"": ""João Silva"", ""valor"": 1200.50 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 950.75 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 1800.00 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 1400.30 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 1100.90 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 1550.00 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 1700.80 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 250.30 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 480.75 }, 
            { ""vendedor"": ""João Silva"", ""valor"": 320.40 }, 
             
            { ""vendedor"": ""Maria Souza"", ""valor"": 2100.40 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 1350.60 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 950.20 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 1600.75 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 1750.00 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 1450.90 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 400.50 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 180.20 }, 
            { ""vendedor"": ""Maria Souza"", ""valor"": 90.75 }, 
             
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 800.50 }, 
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1200.00 }, 
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1950.30 }, 
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1750.80 }, 
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1300.60 }, 
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 300.40 }, 
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 500.00 }, 
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 125.75 }, 
             
            { ""vendedor"": ""Ana Lima"", ""valor"": 1000.00 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 1100.50 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 1250.75 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 1400.20 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 1550.90 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 1650.00 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 75.30 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 420.90 }, 
            { ""vendedor"": ""Ana Lima"", ""valor"": 315.40 } 
          ] 
        }";

        var relatorio = JsonSerializer.Deserialize<relatorio>(json);

        if (relatorio?.Vendas == null) return;

        // Agrupa por vendedor e calcula o total vendido e o total de comissão
        var resultado = relatorio.Vendas
            .GroupBy(v => v.Vendedor)
            .Select(g => new
            {
                Vendedor = g.Key,
                TotalVendido = g.Sum(v => v.Valor),
                TotalComissao = g.Sum(v => CalcularComissao(v.Valor))
            });

        // configura exibicao de valor
        var valorBR = new CultureInfo("pt-BR");

        Console.WriteLine("--- RELATÓRIO DE COMISSÕES ---");
        foreach (var item in resultado)
        {
            Console.WriteLine($"Vendedor: {item.Vendedor}");
            Console.WriteLine($"Total Vendido:  {item.TotalVendido.ToString("C", valorBR)}");
            Console.WriteLine($"Total Comissão: {item.TotalComissao.ToString("C", valorBR)}");
            Console.WriteLine("------------------------------");
        }
    }

    // Regra de comissão
    static decimal CalcularComissao(decimal valor)
    {
        if (valor >= 500.00m)
            return valor * 0.05m;
        
        if (valor >= 100.00m)
            return valor * 0.01m; 

        return 0m;
    }
}

public class relatorio
{
    [JsonPropertyName("vendas")]
    public List<Venda> Vendas { get; set; } = new();
}

public class Venda
{
    [JsonPropertyName("vendedor")]
    public string Vendedor { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }
}